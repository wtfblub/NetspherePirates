using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProudNet.DotNetty.Handlers
{
    internal class MessageHandler : ChannelHandlerAdapter
    {
        private delegate Task<bool> HandlerDelegate(object handler, MessageContext context, object messsage);

        private readonly ILogger _logger;
        private readonly IMessageHandlerResolver _messageHandlerResolver;
        private readonly IServiceProvider _serviceProvider;
        private IReadOnlyDictionary<Type, HandlerInfo[]> _handlerMap;

        public event Action<MessageContext> UnhandledMessage;

        protected virtual void OnUnhandledMessage(MessageContext context)
        {
            UnhandledMessage?.Invoke(context);
        }

        public MessageHandler(IServiceProvider serviceProvider, IMessageHandlerResolver messageHandlerResolver)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MessageHandler>>();
            _messageHandlerResolver = messageHandlerResolver;
            _serviceProvider = serviceProvider;
        }

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            var messageContext = (MessageContext)message;
            messageContext.ChannelHandlerContext = context;

            if (_handlerMap == null)
                InitializeHandlers();

            try
            {
                if (_handlerMap.TryGetValue(messageContext.Message.GetType(), out var handlerInfos))
                {
                    foreach (var handlerInfo in handlerInfos)
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
                            _logger.LogDebug("Message {Message} blocked by rule {Rule}(Invert={Invert}) for handler={Handler}",
                                messageContext.Message.GetType().FullName,
                                ruleThatBlocked.Rule.GetType().FullName, ruleThatBlocked.Invert,
                                handlerInfo.Type.FullName);
                            continue;
                        }

                        if (!await handlerInfo.Func(handlerInfo.Instance, messageContext, messageContext.Message))
                        {
                            _logger.LogDebug("Execution cancelled by {HandlerType}", handlerInfo.Type.FullName);
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
                ReferenceCountUtil.Release(messageContext.Message);
            }
        }

        private void InitializeHandlers()
        {
            _logger.LogInformation("Initializing handlers...");

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
                    methodInfo = handleInterface.GetMethods().Single();

                    var priorityAttribute = methodInfo.GetCustomAttribute<PriorityAttribute>();
                    var priority = priorityAttribute?.Priority ?? 10;

                    var handlerFunc = CreateHandlerFunc(handlerType, methodInfo, messageType);
                    handlerList.Add(new HandlerInfo(handler, handlerFunc, rules, priority));
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

            public HandlerInfo(object instance, HandlerDelegate func, RuleInfo[] rules, byte priority)
            {
                Type = instance.GetType();
                Instance = instance;
                Func = func;
                Rules = rules;
                Priority = priority;
            }
        }

        private struct RuleInfo
        {
            public IFirewallRule Rule;
            public bool Invert;
        }
    }
}
