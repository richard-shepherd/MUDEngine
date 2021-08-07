using System;

namespace Utility
{
    /// <summary>
    /// Attaches to the Logger and writes output to the console.
    /// </summary>
    public class ConsoleLogger : IDisposable
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConsoleLogger()
        {
            // We attach to the logger...
            Logger.onMessageLogged += onMessageLogged;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// The "destructor".
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed) return;

            Logger.onMessageLogged -= onMessageLogged;

            IsDisposed = true;
        }

        // True if Dispose has already been called.
        protected bool IsDisposed { get; private set; }

        #endregion

        #region Private functions

        /// <summary>
        /// Called when a message has been logged.
        /// </summary>
        private void onMessageLogged(object sender, Logger.Args args)
        {
            try
            {
                Console.WriteLine(args.Message);
            }
            catch(Exception)
            {
                // We don't do anything here. In particular, we do not log
                // as we are the logger!
            }
        }

        #endregion
    }
}
