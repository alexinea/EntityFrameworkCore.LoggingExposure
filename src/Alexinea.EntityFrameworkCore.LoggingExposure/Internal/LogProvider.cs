using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming

namespace Alexinea.EntityFrameworkCore.LoggingExposure.Internal {
    internal class LogProvider : ILoggerProvider {

        //volatile to allow the configuration to be switched without locking
        private volatile SelfLoggingConfiguration _configuration;
        private static readonly ConcurrentDictionary<Type, LogProvider> _loggerProviders = new ConcurrentDictionary<Type, LogProvider>();
        private static readonly Func<string, LogLevel, bool> _trueFilter = (categoryName, level) => true;

        private LogProvider(Action<string> logger, Func<string, LogLevel, bool> filter) => _configuration = new SelfLoggingConfiguration(logger, filter);

        public static void CreateOrModifyLoggerForDbContext(Type dbContextType, ILoggerFactory loggerFactory, Action<string> logger, Func<string, LogLevel, bool> filter = null) {
            var isNew = false;
            var provider = _loggerProviders.GetOrAdd(dbContextType, t => {
                    var __provider = new LogProvider(logger, filter ?? _trueFilter);
                    loggerFactory.AddProvider(__provider);
                    isNew = true;
                    return __provider;
                }
            );
            if (!isNew) {
                provider._configuration = new SelfLoggingConfiguration(logger, filter ?? _trueFilter);
            }

        }

        private class SelfLoggingConfiguration {
            public readonly Action<string> Logger;
            public readonly Func<string, LogLevel, bool> Filter;

            public SelfLoggingConfiguration(Action<string> logger, Func<string, LogLevel, bool> filter) {
                Logger = logger;
                Filter = filter;
            }
        }

        public ILogger CreateLogger(string categoryName) => new Logger(categoryName, this);

        private class Logger : ILogger {

            readonly string _categoryName;
            readonly LogProvider _provider;

            public Logger(string categoryName, LogProvider provider) {
                _provider = provider;
                _categoryName = categoryName;
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
                var config = _provider._configuration;
                if (config.Filter(_categoryName, logLevel)) {
                    config.Logger(formatter(state, exception));
                }
            }

            public IDisposable BeginScope<TState>(TState state) => null;
        }

        public void Dispose() { }
    }
}