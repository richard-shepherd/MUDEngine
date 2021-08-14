using System;

namespace Utility
{
    /// <summary>
    /// Not so much a logger as a log-broker.
    /// 
    /// Allows you to log via a static method. Logs are forwarded to clients
    /// connected to the onMessageLogged event.
    /// </summary>
    public class Logger
    {
        #region Events

        /// <summary>
        /// Data passed with the onMessageLogged event. 
        /// </summary>
        public class Args : EventArgs
        {
            public string Message { get; set; } = "";
        }

        /// <summary>
        /// Raised when messages are logged.
        /// </summary>
        public static event EventHandler<Args> onMessageLogged;

        #endregion

        #region Public methods

        /// <summary>
        /// Logs an exeption.
        /// </summary>
        public static void log(Exception ex)
        {
            log($"{ex.Message}: {ex.StackTrace}");
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        public static void log(string message)
        {
            var args = new Args { Message = message };
            Utils.raiseEvent(onMessageLogged, null, args);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void error(string message)
        {
            var args = new Args { Message = $"ERROR: {message}" };
            Utils.raiseEvent(onMessageLogged, null, args);
        }

        #endregion
    }
}