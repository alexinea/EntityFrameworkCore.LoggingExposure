using System;
using System.Collections.Generic;
using Alexinea.EntityFrameworkCore.LoggingExposure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Alexinea.EntityFrameworkCore.LoggingExposure {
    public static class DbContextExtensions {
        public static void ConfigureLogging(this DbContext db, Action<string> logger, Func<string, LogLevel, bool> filter) {
            var serviceProvider = db.GetInfrastructure<IServiceProvider>();
            var loggerFactory = (ILoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));

            LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(), loggerFactory, logger, filter);
        }

        public static void ConfigureLogging(this DbContext db, Action<string> logger, LoggingCategories categories = LoggingCategories.All) {
            var serviceProvider = db.GetInfrastructure<IServiceProvider>();
            var loggerFactory = (LoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));

            if (categories == LoggingCategories.Sql) {
                var sqlCategories = new List<string> {
                    DbLoggerCategory.Database.Command.Name,
                    DbLoggerCategory.Query.Name,
                    DbLoggerCategory.Update.Name
                };
                LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(),
                    loggerFactory,
                    logger,
                    (c, l) => sqlCategories.Contains(c));
            } else if (categories == LoggingCategories.All) {
                LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(),
                    loggerFactory, logger,
                    (c, l) => true);
            }
        }
    }
}