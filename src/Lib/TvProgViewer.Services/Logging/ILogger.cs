﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Logging;

namespace TvProgViewer.Services.Logging
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public partial interface ILogger
    {
        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteLogAsync(Log log);

        /// <summary>
        /// Deletes a log items
        /// </summary>
        /// <param name="logs">Log items</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteLogsAsync(IList<Log> logs);

        /// <summary>
        /// Clears a log
        /// </summary>
        /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ClearLogAsync(DateTime? olderThan = null);

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the log item items
        /// </returns>
        Task<IPagedList<Log>> GetAllLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the log item
        /// </returns>
        Task<Log> GetLogByIdAsync(int logId);

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the log items
        /// </returns>
        Task<IList<Log>> GetLogByIdsAsync(int[] logIds);

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="user">The user to associate log record with</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a log item
        /// </returns>
        Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null);

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="user">The user to associate log record with</param>
        /// <returns>
        /// Log item
        /// </returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null);

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InformationAsync(string message, Exception exception = null, User user = null);

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        void Information(string message, Exception exception = null, User user = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task WarningAsync(string message, Exception exception = null, User user = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        void Warning(string message, Exception exception = null, User user = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ErrorAsync(string message, Exception exception = null, User user = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="user">User</param>
        void Error(string message, Exception exception = null, User user = null);
    }
}