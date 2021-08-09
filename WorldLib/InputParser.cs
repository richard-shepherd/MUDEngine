using System;
using System.Collections.Generic;
using Utility;

namespace WorldLib
{
    /// <summary>
    /// Parses user input.
    /// </summary>
    public class InputParser
    {
        #region Public types

        /// <summary>
        /// Enum for parsed actions.
        /// </summary>
        public enum ActionEnum
        {
            NO_ACTION,
            LOOK,
            GO_TO_DIRECTION
        }

        /// <summary>
        /// User input parsed into an action enum and the associated objects and directions.
        /// </summary>
        public class ParsedInput
        {
            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            public ActionEnum Action { get; set; } = ActionEnum.NO_ACTION;

            /// <summary>
            /// Gets or sets the direction.
            /// Used with the GO_TO_DIRECTION action.
            /// </summary>
            public string Direction { get; set; } = "";
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputParser()
        {
        }

        /// <summary>
        /// Parses user input.
        /// </summary>
        public ParsedInput parseInput(string input)
        {
            // We convert the input to uppercase before parsing...
            input = input.ToUpper();

            // We check if the input is a simple compass direction...
            var parsedInput = parse_CompassDirection(input);
            if (parsedInput != null)
            {
                return parsedInput;
            }

            // We check if the input is LOOK...
            parsedInput = parse_Look(input);
            if (parsedInput != null)
            {
                return parsedInput;
            }

            return null;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Checks if the input is a simple compass direction.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_CompassDirection(string input)
        {
            if (!m_directions.Contains(input))
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.GO_TO_DIRECTION;
            parsedInput.Direction = input;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is a look command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Look(string input)
        {
            if (input != "LOOK")
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.LOOK;
            return parsedInput;
        }

        #endregion

        #region Private data

        // Compass directions...
        private readonly HashSet<string> m_directions = new HashSet<string> { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        
        #endregion
    }
}
