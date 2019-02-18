using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using BlubLib.Threading.Tasks;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using ProudNet.Hosting.Services;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.DotNetty.Handlers
{
    internal class MessageHandler : ChannelHandlerAdapter
    {
        private delegate Task<bool> HandlerDelegate(object handler, MessageContext context, object messsage);

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageHandlerResolver _messageHandlerResolver;
        private readonly ISchedulerService _schedulerService;
        private IReadOnlyDictionary<Type, HandlerInfo[]> _handlerMap;

        public event Action<MessageContext> UnhandledMessage;

        protected virtual void OnUnhandledMessage(MessageContext context)
        {
            UnhandledMessage?.Invoke(context);
        }

        public MessageHandler(IServiceProvider serviceProvider, IMessageHandlerResolver messageHandlerResolver,
            ISchedulerService schedulerService)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MessageHandler>>();
            _serviceProvider = serviceProvider;
            _messageHandlerResolver = messageHandlerResolver;
            _schedulerService = schedulerService;
        }

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            var messageContext = (MessageContext)message;
            messageContext.ChannelHandlerContext = context;
            var messageType = messageContext.Message.GetType();

            if (_handlerMap == null)
                InitializeHandlers();

            try
            {
                if (_handlerMap.TryGetValue(messageType, out var handlerInfos))
                {
                    foreach (var handlerInfo in handlerInfos.OrderByDescending(x => x.Priority))
                    {
                        var isAllowed = true;
                        var ruleThatBlocked = default(RuleInfo);

                        foreach (var ruleInfo in handlerInfo.Rules)
                        {
                            var result = await ruleInfo.Rule.IsMessageAllowed(messageContext, messageContext.Message);
                            if (ruleInfo.Invert)
                                result = !result;

                            if (!result)
                            {
                                isAllowed = false;
                                ruleThatBlocked = ruleInfo;
                                break;
                            }
                        }

                        if (!isAllowed)
                        {
                            _logger.Debug("Message {Message} blocked by rule {Rule}(Invert={Invert}) for handler={Handler}",
                                messageContext.Message.GetType().FullName,
                                ruleThatBlocked.Rule.GetType().FullName, ruleThatBlocked.Invert,
                                handlerInfo.Type.FullName);
                            continue;
                        }

                        bool continueExecution;
                        if (handlerInfo.Inline)
                        {
                            continueExecution = await handlerInfo.Func(
                                handlerInfo.Instance, messageContext, messageContext.Message
                            );
                        }
                        else
                        {
                            var tcs = new TaskCompletionSource<bool>();
                            _schedulerService.Execute(ExecuteHandler, (handlerInfo, messageContext, tcs), null);

                            async void ExecuteHandler(object state, object _)
                            {
                                var tuple =
                                    ((HandlerInfo handlerInfo, MessageContext messageContext, TaskCompletionSource<bool> tcs))
                                    state;
                                var result = await tuple.handlerInfo.Func(
                                    tuple.handlerInfo.Instance,
                                    tuple.messageContext,
                                    tuple.messageContext.Message
                                ).AnyContext();
                                tuple.tcs.TrySetResult(result);
                            }

                            continueExecution = await tcs.Task;
                        }

                        if (!continueExecution)
                        {
                            _logger.Debug("Execution cancelled by {HandlerType}", handlerInfo.Type.FullName);
                            break;
                        }
                    }
                }
                else
                {
                    OnUnhandledMessage(messageContext);
                    // _logger.LogDebug("Unhandled message {Message}", messageContext.Message.GetType());
                }
            }
            catch (Exception ex)
            {
                base.ExceptionCaught(context, ex);
            }
            finally
            {
                if (messageType != typeof(RmiMessage))
                    context.Channel?.Pipeline?.Context(Constants.Pipeline.CoreMessageHandlerName)?.Read();

                ReferenceCountUtil.Release(message);
            }
        }

        private void InitializeHandlers()
        {
            _logger.Information("Initializing handlers...");

            var map = new Dictionary<Type, List<HandlerInfo>>();
            var handlerTypes = _messageHandlerResolver.GetImplementations();
            foreach (var handlerType in handlerTypes)
            {
                // Create an instance of the handler and inject dependencies if needed

                var constructors = handlerType.GetConstructors();
                if (constructors.Length != 1)
                {
                    throw new ProudException(
                        $"IHandle implementation '{handlerType.FullName}' has more than one constructor. Only one constructor is allowed.");
                }

                var parameterInfos = constructors[0].GetParameters();
                var parameters = new object[parameterInfos.Length];
                for (var i = 0; i < parameterInfos.Length; i++)
                {
                    var param = parameterInfos[i];
                    parameters[i] = _serviceProvider.GetRequiredService(param.ParameterType);
                }

                var handler = Activator.CreateInstance(handlerType, parameters);

                // Generate a handler function for every IHandle<> implementation

                var handleInterfaces = handlerType.GetTypeInfo().ImplementedInterfaces.Where(x => x.IsMessageHandlerInterface());
                foreach (var handleInterface in handleInterfaces)
                {
                    var messageType = handleInterface.GenericTypeArguments.Single();

                    // Filter out messages that are not meant to be handled
                    if (!_messageHandlerResolver.BaseTypes.Any(x => x.IsAssignableFrom(messageType)))
                        continue;

                    var handlerList = map.GetValueOrDefault(messageType) ?? new List<HandlerInfo>();

                    // Get the method from the implementing type to scan for FirewallAttributes
                    // then get the method from the interface because the handler will call it
                    // from the interface and not from the type
                    var methodInfo = handlerType.GetInterfaceMap(handleInterface).TargetMethods.Single();
                    var rules = GetRulesFromMethod(methodInfo).ToArray();
                    var priorityAttribute = methodInfo.GetCustomAttribute<PriorityAttribute>();
                    var priority = priorityAttribute?.Priority ?? 10;
                    var scheduleAttribute = methodInfo.GetCustomAttribute<InlineAttribute>();

                    methodInfo = handleInterface.GetMethods().Single();
                    var handlerFunc = CreateHandlerFunc(handlerType, methodInfo, messageType);
                    handlerList.Add(new HandlerInfo(handler, handlerFunc, rules, priority, scheduleAttribute != null));
                    map[messageType] = handlerList;
                }
            }

            _handlerMap = map.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(hi => hi.Priority).ToArray());

            IEnumerable<RuleInfo> GetRulesFromMethod(MethodInfo mi)
            {
                var attributes = mi.GetCustomAttributes<FirewallAttribute>();
                foreach (var attribute in attributes)
                {
                    var rule = GetFirewallRule(attribute.FirewallRuleType, attribute.Parameters);
                    yield return new RuleInfo
                    {
                        Rule = rule,
                        Invert = attribute.Invert
                    };
                }
            }

            HandlerDelegate CreateHandlerFunc(Type handlerType, MethodInfo methodInfo, Type messageType)
            {
                var This = Expression.Parameter(typeof(object));
                var contextParam = Expression.Parameter(typeof(MessageContext));
                var messageParam = Expression.Parameter(typeof(object));
                var body = Expression.Call(Expression.Convert(This, handlerType), methodInfo,
                    contextParam, Expression.Convert(messageParam, messageType));
                var handlerFunc = Expression.Lambda<HandlerDelegate>(body, This, contextParam, messageParam).Compile();
                return handlerFunc;
            }
        }

        private IFirewallRule GetFirewallRule(Type type, object[] parameters)
        {
            var ctors = type.GetConstructors();
            if (ctors.Length != 1)
            {
                throw new ProudException(
                    $"Rule implementation '{type.FullName}' has more than one constructor. Only one constructor is allowed.");
            }

            var ctor = ctors.Single();
            var ctorParams = ctor.GetParameters();
            var paramCount = parameters?.Length ?? 0;
            var diParamCount = ctorParams.Length - paramCount;
            var parametersToPass = parameters;

            // Any left over parameters will be resolved via dependency injection
            if (diParamCount > 0)
            {
                parametersToPass = new object[ctorParams.Length];
                Array.Copy(parameters, parametersToPass, paramCount);

                for (var i = paramCount; i < ctorParams.Length; ++i)
                    parametersToPass[i] = _serviceProvider.GetRequiredService(ctorParams[i].ParameterType);
            }

            return (IFirewallRule)Activator.CreateInstance(type, parametersToPass);
        }

        private struct HandlerInfo
        {
            public readonly Type Type;
            public readonly object Instance;
            public readonly HandlerDelegate Func;
            public readonly RuleInfo[] Rules;
            public readonly byte Priority;
            public readonly bool Inline;

            public HandlerInfo(object instance, HandlerDelegate func, RuleInfo[] rules, byte priority, bool inline)
            {
                Type = instance.GetType();
                Instance = instance;
                Func = func;
                Rules = rules;
                Priority = priority;
                Inline = inline;
            }
        }

        private struct RuleInfo
        {
            public IFirewallRule Rule;
            public bool Invert;
        }
    }
}
