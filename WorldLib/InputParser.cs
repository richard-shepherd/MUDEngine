using System;
using System.Collections.Generic;
using System.Linq;
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
            GO_TO_DIRECTION,
            TAKE
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
            /// </summary><remarks>
            /// Used with the GO_TO_DIRECTION action.
            /// </remarks>
            public string Direction { get; set; } = "";

            /// <summary>
            /// Gets or sets the primary target object.
            /// </summary><remarks>
            /// Used with the TAKE action, eg TAKE [target-1].
            /// </remarks>
            public string Target1 { get; set; } = "";
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
            var uppercaseInput = input.ToUpper();

            // We check the parsing functions...
            var parsingFunctions = new List<Func<string, string, ParsedInput>>
            {
                parse_CompassDirection,
                parse_Look,
                parse_Take
            };
            foreach(var parsingFunction in parsingFunctions)
            {
                var parsedInput = parsingFunction(uppercaseInput, input);
                if (parsedInput != null)
                {
                    return parsedInput;
                }
            }

            return null;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Checks if the input is a simple compass direction.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_CompassDirection(string uppercaseInput, string input)
        {
            if (!m_directions.Contains(uppercaseInput))
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.GO_TO_DIRECTION;
            parsedInput.Direction = uppercaseInput;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is a look command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Look(string uppercaseInput, string input)
        {
            if (uppercaseInput != "LOOK")
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.LOOK;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is a take command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Take(string uppercaseInput, string input)
        {
            // We check if the input starts with a TAKE synonym...
            var synonyms = new List<string> { "TAKE", "PICK UP" };
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if(matchingSynonym == null)
            {
                return null;
            }

            // We have a take command, so we find the target...
            var target = input.Substring(matchingSynonym.Length).Trim();

            // If the target starts with "the " we remove this...
            if(target.ToUpper().StartsWith("THE "))
            {
                target = target.Substring(4);
            }

            // We return the parsed input...
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.TAKE;
            parsedInput.Target1 = target;
            return parsedInput;
        }

        #endregion

        #region Private data

        // Compass directions...
        private readonly HashSet<string> m_directions = new HashSet<string> { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        
        #endregion
    }
}
