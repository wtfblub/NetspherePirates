using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Netsphere.Common.Configuration;
using Netsphere.Common.Configuration.Hjson;
using Netsphere.Common.Converters;
using Netsphere.Common.Converters.Json;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using Serilog.Sinks.SystemConsole.Themes;
using TimeSpanConverter = Netsphere.Common.Converters.Json.TimeSpanConverter;

namespace Netsphere.Common
{
    public static class Startup
    {
        public static IConfiguration Initialize(string baseDirectory,
            string configFile, Func<IConfiguration, LoggerOptions> loggerOptionsFactory)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new IPAddressConverter(),
                    new IPEndPointConverter(),
                    new DnsEndPointConverter(),
                    new TimeSpanConverter(),
                    new VersionConverter(),
                    new PeerIdConverter()
                }
            };

            TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(IPAddressTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(IPEndPointTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(DnsEndPoint), new TypeConverterAttribute(typeof(DnsEndPointTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(TimeSpan), new TypeConverterAttribute(typeof(TimeSpanTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Version), new TypeConverterAttribute(typeof(VersionTypeConverter)));
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            configFile = Path.Combine(baseDirectory, configFile);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddHjsonFile(configFile, false, true)
                .Build();

            var loggerOptions = loggerOptionsFactory(configuration);
            InitializeSerilog(baseDirectory, loggerOptions);
            return configuration;
        }

        private static void InitializeSerilog(string baseDirectory, LoggerOptions options)
        {
            var logDir = Path.Combine(baseDirectory, options.Directory);
            logDir = logDir.Replace("$(BASE)", AppDomain.CurrentDomain.BaseDirectory);

            if (!Enum.TryParse<LogEventLevel>(options.Level, out var logLevel))
            {
                Console.Error.WriteLine(
                    $"Invalid log level {options.Level}. Valid values are {string.Join(",", Enum.GetNames(typeof(LogEventLevel)))}");
                Environment.Exit(1);
            }

            var jsonlog = Path.Combine(logDir, $"{options.Name}.log.json");
            var logfile = Path.Combine(logDir, $"{options.Name}.log");
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransforming<IPEndPoint>(endPoint => endPoint.ToString())
                .Destructure.ByTransforming<EndPoint>(endPoint => endPoint.ToString())
                .WriteTo.File(new JsonFormatter(), jsonlog, rollingInterval: RollingInterval.Day)
                .WriteTo.File(logfile, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(new ConditionalTemplateRenderer())
                .MinimumLevel.Is(logLevel)
                .CreateLogger().ForContext(Serilog.Core.Constants.SourceContextPropertyName, "Main");
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Log.Error(e.Exception, "UnobservedTaskException");
        }

        private static void OnUnhandledException(object s, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception)e.ExceptionObject, "UnhandledException");
        }

        private class ConditionalTemplateRenderer : ITextFormatter
        {
            private readonly ITextFormatter _consoleFormatter;
            private readonly ITextFormatter _consoleFormatterWithProperties;
            private readonly Func<MessageTemplate, PropertyToken[]> _namedProperties;
            private readonly Func<MessageTemplate, PropertyToken[]> _positionalProperties;

            public ConditionalTemplateRenderer()
            {
                var assembly = Assembly.GetAssembly(typeof(ConsoleLoggerConfigurationExtensions));
                var theme = Console.IsOutputRedirected || Console.IsErrorRedirected
                    ? ConsoleTheme.None
                    : AnsiConsoleTheme.Literate;
                var type = assembly.GetType("Serilog.Sinks.SystemConsole.Output.OutputTemplateRenderer");

                _consoleFormatter = (ITextFormatter)Activator.CreateInstance(type, theme,
                    "[{Level} {SourceContext}] {Message:lj}{NewLine}{Exception}", default(IFormatProvider));

                _consoleFormatterWithProperties = (ITextFormatter)Activator.CreateInstance(type, theme,
                    "[{Level} {SourceContext}] {Message:lj}{NewLine}    {Properties:l} {NewLine}{Exception}",
                    default(IFormatProvider));

                var namedPropertiesProp = typeof(MessageTemplate)
                    .GetProperty("NamedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

                var positionalPropertiesProp = typeof(MessageTemplate)
                    .GetProperty("PositionalProperties", BindingFlags.NonPublic | BindingFlags.Instance);

                var parameter = Expression.Parameter(typeof(MessageTemplate));
                _namedProperties = Expression.Lambda<Func<MessageTemplate, PropertyToken[]>>(
                        Expression.Property(parameter, namedPropertiesProp), parameter)
                    .Compile();

                _positionalProperties = Expression.Lambda<Func<MessageTemplate, PropertyToken[]>>(
                        Expression.Property(parameter, positionalPropertiesProp), parameter)
                    .Compile();
            }

            public void Format(LogEvent logEvent, TextWriter output)
            {
                var propCount = logEvent.Properties.Count -
                                _namedProperties(logEvent.MessageTemplate)?.Length ?? 0 +
                                _positionalProperties(logEvent.MessageTemplate)?.Length ?? 0;

                if (propCount > 1)
                    _consoleFormatterWithProperties.Format(logEvent, output);
                else
                    _consoleFormatter.Format(logEvent, output);
            }
        }
    }
}
