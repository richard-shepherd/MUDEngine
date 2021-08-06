﻿namespace WorldLib
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
            SUCCEDED,
            FAILED
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status of the result.
        /// </summary>
        public StatusEnum Status { get; set; } = StatusEnum.FAILED;

        /// <summary>
        /// Gets or sets an optional description associated with the result.
        /// </summary>
        public string Description { get; set; } = "";

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
        public ActionResult(StatusEnum status, string description="")
        {
            Status = status;
            Description = description;
        }

        /// <summary>
        /// Static helper to create a SUCCEEDED result.
        /// </summary>
        public static ActionResult succeeded()
        {
            return new ActionResult(StatusEnum.SUCCEDED);
        }

        /// <summary>
        /// Static helper to create a FAILED result.
        /// </summary>
        public static ActionResult failed(string description)
        {
            return new ActionResult(StatusEnum.FAILED, description);
        }

        #endregion
    }
}