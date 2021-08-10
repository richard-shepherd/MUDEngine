using System.Collections.Generic;

namespace UI_WinForms
{
    /// <summary>
    /// Parsed version of config.json.
    /// </summary>
    internal class ParsedConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets the root folders where object definitions are found.
        /// </summary>
        public List<string> ObjectFolders { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the ID of the location where players are initially placed.
        /// </summary>
        public string StartingLocationID { get; set; } = "";

        #endregion
    }
}
