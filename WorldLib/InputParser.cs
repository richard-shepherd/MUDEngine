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
            DROP,
            EXAMINE,
            GIVE,
            GO_TO_DIRECTION,
            INVENTORY,
            KILL,
            LOOK,
            SMOKE_POT,
            STATS,
            TAKE,
            TALK
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
            /// For example: TAKE [target1]
            /// </remarks>
            public string Target1 { get; set; } = "";

            /// <summary>
            /// Gets or sets the secondary target object.
            /// </summary><remarks>
            /// For example: GIVE [target1] TO [target2]
            /// </remarks>
            public string Target2 { get; set; } = "";
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
                parse_Drop,
                parse_Examine,
                parse_Give,
                parse_Inventory,
                parse_Kill,
                parse_Look,
                parse_SmokePot,
                parse_Stats,
                parse_Take,
                parse_Talk
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
        /// Checks if the input is a STATS command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Stats(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a DROP synonym...
            var synonyms = new List<string> { "STATS", "SHOW STATS", "SHOW STATS FOR" };
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.STATS, synonyms);
        }

        /// <summary>
        /// Checks if the input is a DROP command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Drop(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a DROP synonym...
            var synonyms = new List<string> { "DROP" };
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.DROP, synonyms);
        }

        /// <summary>
        /// Checks if the input is a GIVE command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Give(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a GIVE synonym...
            var synonyms = new List<string> { "GIVE" };
            return parse_WithTarget1Target2(uppercaseInput, originalInput, ActionEnum.GIVE, synonyms);
        }

        /// <summary>
        /// Checks if the input is a TALK command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Talk(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a TALK synonym...
            var synonyms = new List<string> { "TALK TO", "TALK" };
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.TALK, synonyms);
        }

        /// <summary>
        /// Checks if the input is a KILL command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Kill(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a KILL synonym...
            var synonyms = new List<string> { "KILL", "FIGHT" };
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.KILL, synonyms);
        }

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
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.TAKE, synonyms);
        }


        /// <summary>
        /// Checks if the input is a examine command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Examine(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with an EXAMINE synonym...
            var synonyms = new List<string> { "EXAMINE", "LOOK AT" };
            return parse_WithTarget(uppercaseInput, originalInput, ActionEnum.EXAMINE, synonyms);
        }

        /// <summary>
        /// Returns the target from inputs like "TAKE [target]" or "EXAMINE THE [target]".
        /// </summary>
        private string getTarget(string command, string originalInput)
        {
            var inputWithoutCommand = Utils.removeInitial(originalInput, command);
            var target = Utils.removeInitial(inputWithoutCommand, "THE ");
            return target;
        }

        /// <summary>
        /// Returns the targets from inputs like "GIVE [target1] TO [target2]" or 
        /// "KILL THE [target1] WITH THE [target2]".
        /// </summary>
        private (string target1, string target2) getTarget1Target2(string command, string originalInput)
        {
            // We remove the command...
            var inputWithoutCommand = Utils.removeInitial(originalInput, command);

            // We find the preposition...
            var prepositions = new List<string> { "WITH", "TO", "AT" };
            var uppercaseInputWithoutCommand = inputWithoutCommand.ToUpper();
            var target1 = "";
            var target2 = "";
            foreach (var preposition in prepositions)
            {
                var prepositionWithSpaces = $" {preposition} ";
                var index_Preposition = uppercaseInputWithoutCommand.IndexOf(prepositionWithSpaces);
                if(index_Preposition != -1)
                {
                    // We have found the preposition so we can find target1 and target2
                    // from the string either side of it...
                    target1 = inputWithoutCommand.Substring(0, index_Preposition);
                    target2 = inputWithoutCommand.Substring(index_Preposition + prepositionWithSpaces.Length);
                    break;
                }
            }

            // We make sure the targets do not start with "the"...
            target1 = Utils.removeInitial(target1, "THE ");
            target2 = Utils.removeInitial(target2, "THE ");

            return (target1, target2);
        }

        /// <summary>
        /// Returns ParsedInput for a command with a target: "[command] [target]", eg, "TAKE apple".
        /// Returns null if the input does not match the command.
        /// </summary>
        private ParsedInput parse_WithTarget(string uppercaseInput, string originalInput, ActionEnum action, List<string> synonyms)
        {
            // We check if the input starts with a command synonym...
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if (matchingSynonym == null)
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = action;
            parsedInput.Target1 = getTarget(matchingSynonym, originalInput);
            return parsedInput;
        }

        /// <summary>
        /// Returns ParsedInput for a command with two targets: "[command] [target1] [preposition] [target2]", eg, "GIVE apple to dragon".
        /// Returns null if the input does not match the command.
        /// </summary>
        private ParsedInput parse_WithTarget1Target2(string uppercaseInput, string originalInput, ActionEnum action, List<string> synonyms)
        {
            // We check if the input starts with a command synonym...
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if (matchingSynonym == null)
            {
                return null;
            }

            // We find the targets...
            var targets = getTarget1Target2(matchingSynonym, originalInput);

            // We return the parsed input...
            var parsedInput = new ParsedInput();
            parsedInput.Action = action;
            parsedInput.Target1 = targets.target1;
            parsedInput.Target2 = targets.target2;
            return parsedInput;
        }

        #endregion

        #region Private data

        // Compass directions...
        private readonly HashSet<string> m_directions = new HashSet<string> { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        
        #endregion
    }
}
