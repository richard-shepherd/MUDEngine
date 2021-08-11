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
            SMOKE_POT,
            LOOK,
            GO_TO_DIRECTION,
            TAKE,
            EXAMINE,
            INVENTORY
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
            /// Used with the EXAMINE action, eg EXAMINE [target-1].
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
            var uppercaseInput = input.ToUpper().Trim();

            // We check the parsing functions...
            var parsingFunctions = new List<Func<string, string, ParsedInput>>
            {
                parse_CompassDirection,
                parse_Look,
                parse_Take,
                parse_Examine,
                parse_Inventory,
                parse_SmokePot
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
        /// Checks if the input is requesting the inventory.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Inventory(string uppercaseInput, string originalInput)
        {
            // We check if the input is one of the synonyms for the inventory...
            var synonyms = new List<string> { "I", "INVENTORY" };
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput == x);
            if (matchingSynonym == null)
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.INVENTORY;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is SMOKE POT.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_SmokePot(string uppercaseInput, string originalInput)
        {
            if (uppercaseInput != "SMOKE POT")
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.SMOKE_POT;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is a simple compass direction.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_CompassDirection(string uppercaseInput, string originalInput)
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
        private ParsedInput parse_Look(string uppercaseInput, string originalInput)
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
        private ParsedInput parse_Take(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a TAKE synonym...
            var synonyms = new List<string> { "TAKE", "PICK UP", "GET" };
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if(matchingSynonym == null)
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.TAKE;
            parsedInput.Target1 = getTarget(matchingSynonym, originalInput);
            return parsedInput;
        }


        /// <summary>
        /// Checks if the input is a examine command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Examine(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with an TAKE synonym...
            var synonyms = new List<string> { "EXAMINE", "LOOK AT" };
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if (matchingSynonym == null)
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.EXAMINE;
            parsedInput.Target1 = getTarget(matchingSynonym, originalInput);
            return parsedInput;
        }

        /// <summary>
        /// Returns the target from inputs like "TAKE [target]", "EXAMINE [target]" etc.
        /// </summary>
        private string getTarget(string command, string originalInput)
        {
            var target = originalInput.Substring(command.Length).Trim();

            // If the target starts with "the " we remove this...
            if (target.ToUpper().StartsWith("THE "))
            {
                target = target.Substring(4);
            }

            return target;
        }

        #endregion

        #region Private data

        // Compass directions...
        private readonly HashSet<string> m_directions = new HashSet<string> { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        
        #endregion
    }
}
