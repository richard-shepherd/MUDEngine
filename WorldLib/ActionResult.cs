namespace WorldLib
{
    /// <summary>
    /// Info returned when actions are performed.
    /// </summary>
    public class ActionResult
    {
        #region Public types

        /// <summary>
        /// Enum for success / failure.
        /// </summary>
        public enum StatusEnum
        {
            SUCCEEDED,
            FAILED
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status of the result.
        /// </summary>
        public StatusEnum Status { get; set; } = StatusEnum.FAILED;

        /// <summary>
        /// Gets or sets an optional message associated with the result.
        /// </summary>
        public string Message { get; set; } = "";

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionResult()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionResult(StatusEnum status, string message="")
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// Static helper to create a SUCCEEDED result.
        /// </summary>
        public static ActionResult succeeded()
        {
            return new ActionResult(StatusEnum.SUCCEEDED);
        }

        /// <summary>
        /// Static helper to create a FAILED result.
        /// </summary>
        public static ActionResult failed(string message)
        {
            return new ActionResult(StatusEnum.FAILED, message);
        }

        #endregion
    }
}
