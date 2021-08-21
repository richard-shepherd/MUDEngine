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
            EAT,
            EXAMINE,
            GIVE,
            GO_TO_DIRECTION,
            HELP,
            INVENTORY,
            KILL,
            LOOK,
            PUT,
            REPAIR,
            SMOKE_POT,
            STATS,
            TAKE,
            TALK,
            UNLOCK,
            WEAR
        }

        /// <summary>
        /// Gets help for commands.
        /// </summary>
        public List<string> Help { get; } = new List<string>
        {
            "DROP [target],                'drop apple', 'drop all'",
            "EAT [target],                 'eat apple'",
            "EXAMINE [target],             'examine dragon'",
            "GIVE [target1] TO [target2]   'give apple to dragon'",
            "N,NE,E,SE,S,SW,W,NW,UP,DOWN   moves in the direction specified",
            "I / INVENTORY                 shows the inventory",
            "KILL [target]                 'kill dragon'",
            "LOOK                          looks at the current location",
            "PUT [target1] IN [target2]    'put sword in chest'",
            "REPAIR [target]               'repair steel armour'",
            "STATS                         shows stats for the player",
            "STATS [target]                'stats dragon'",
            "TAKE [target]                 'take apple', 'take apples'",
            "TALK (TO) [target]            'talk to shopkeeper', 'talk shopkeeper'",
            "UNLOCK [target]               'unlock trapdoor' (the right key will be chosen if you have it)",
            "WEAR [target]                 'wear leather armour'"
        };

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
                parse_Eat,
                parse_Examine,
                parse_Give,
                parse_Help,
                parse_Inventory,
                parse_Kill,
                parse_Look,
                parse_Put,
                parse_Repair,
                parse_SmokePot,
                parse_Stats,
                parse_Take,
                parse_Talk,
                parse_Unlock,
                parse_Wear
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
        /// Checks if the input is an REPAIR command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Repair(string uppercaseInput, string originalInput)
        {
            // We check if the input includes a REPAIR synonym.
            var synonyms = new List<string> { "REPAIR ", "FIX " };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.REPAIR, synonyms, CommandPosition.ANYWHERE);
        }

        /// <summary>
        /// Checks if the input is an PUT command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Put(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a WEAR synonym...
            var synonyms = new List<string> { "PUT" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.PUT, synonyms);
        }

        /// <summary>
        /// Checks if the input is an WEAR command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Wear(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a WEAR synonym...
            var synonyms = new List<string> { "WEAR ", "PUT ON " };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.WEAR, synonyms);
        }

        /// <summary>
        /// Checks if the input is an UNLOCK command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Unlock(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with an UNLOCK synonym...
            var synonyms = new List<string> { "UNLOCK", "OPEN" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.UNLOCK, synonyms);
        }

        /// <summary>
        /// Checks if the input is an EAT command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Eat(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with an EAT synonym...
            var synonyms = new List<string> { "EAT", "MUNCH", "SCOFF" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.EAT, synonyms);
        }

        /// <summary>
        /// Checks if the input is a HELP command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Help(string uppercaseInput, string originalInput)
        {
            // We check if the input is one of the synonyms for HELP...
            var synonyms = new List<string> { "HELP" };
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput == x);
            if (matchingSynonym == null)
            {
                return null;
            }
            var parsedInput = new ParsedInput();
            parsedInput.Action = ActionEnum.HELP;
            return parsedInput;
        }

        /// <summary>
        /// Checks if the input is a STATS command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Stats(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a STATS synonym...
            var synonyms = new List<string> { "STATS", "SHOW STATS", "SHOW STATS FOR" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.STATS, synonyms);
        }

        /// <summary>
        /// Checks if the input is a DROP command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Drop(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a DROP synonym...
            var synonyms = new List<string> { "DROP" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.DROP, synonyms);
        }

        /// <summary>
        /// Checks if the input is a GIVE command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Give(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a GIVE synonym...
            var synonyms = new List<string> { "GIVE" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.GIVE, synonyms);
        }

        /// <summary>
        /// Checks if the input is a TALK command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Talk(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a TALK synonym...
            var synonyms = new List<string> { "TALK TO", "TALK" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.TALK, synonyms);
        }

        /// <summary>
        /// Checks if the input is a KILL command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Kill(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with a KILL synonym...
            var synonyms = new List<string> { "KILL", "FIGHT", "ATTACK" };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.KILL, synonyms);
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
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.TAKE, synonyms);
        }

        /// <summary>
        /// Checks if the input is a examine command.
        /// Returns a ParsedInput if so, null if not.
        /// </summary>
        private ParsedInput parse_Examine(string uppercaseInput, string originalInput)
        {
            // We check if the input starts with an EXAMINE synonym...
            var synonyms = new List<string> { "EXAMINE ", "LOOK AT ", "LOOK IN " };
            return parse_WithTargets(uppercaseInput, originalInput, ActionEnum.EXAMINE, synonyms);
        }

        /// <summary>
        /// Returns the targets from inputs like "GIVE [target1] TO [target2]" or 
        /// "KILL THE [target1] WITH THE [target2]".
        /// </summary>
        private (string target1, string target2) getTargets(string command, string originalInput)
        {
            // We remove the command...
            var inputWithoutCommand = Utils.removeInitial(originalInput, command);

            // We find the preposition...
            var prepositions = new List<string> { "WITH", "TO", "AT", "IN", "INTO", "ON", "ONTO" };
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

            // If no targets were found, this means that the command did not include a preposition.
            // In this case we do not have a target2...
            if(String.IsNullOrEmpty(target1))
            {
                target1 = inputWithoutCommand;
            }

            // We make sure the targets do not start with "the"...
            target1 = Utils.removeInitial(target1, "THE ");
            target2 = Utils.removeInitial(target2, "THE ");

            return (target1, target2);
        }

        /// <summary>
        /// Returns ParsedInput for a command with two targets: "[command] [target1] [preposition] [target2]", eg, "GIVE apple to dragon".
        /// Returns null if the input does not match the command.
        /// </summary>
        private ParsedInput parse_WithTargets(string uppercaseInput, string originalInput, ActionEnum action, List<string> synonyms, CommandPosition commandPosition = CommandPosition.START)
        {
            // If we are looking for commands anywhere in the inputi, we 
            var preprocessedInput = preprocessInput(uppercaseInput, originalInput, synonyms, commandPosition);

            // We check if the input starts with a command synonym...
            var matchingSynonym = synonyms.FirstOrDefault(x => preprocessedInput.UppercaseInput.StartsWith(x));
            if (matchingSynonym == null)
            {
                return null;
            }

            // We find the targets...
            var targets = getTargets(matchingSynonym, preprocessedInput.OriginalInput);

            // We return the parsed input...
            var parsedInput = new ParsedInput();
            parsedInput.Action = action;
            parsedInput.Target1 = targets.target1;
            parsedInput.Target2 = targets.target2;
            return parsedInput;
        }

        /// <summary>
        /// Preprocesses the input based on the expected command position.
        /// </summary>
        private (string UppercaseInput, string OriginalInput) preprocessInput(string uppercaseInput, string originalInput, List<string> synonyms, CommandPosition commandPosition)
        {
            switch(commandPosition)
            {
                case CommandPosition.START:
                    return (uppercaseInput, originalInput);

                case CommandPosition.ANYWHERE:
                    return preprocessInput_Anywhere(uppercaseInput, originalInput, synonyms);

                default:
                    throw new Exception($"Unhandled command-position: {commandPosition}");
            }
        }

        /// <summary>
        /// Preprocesses input where the command can be anywhere in the text.
        /// </summary><remarks>
        /// For example, a repair command could be "repair armour" or "ask blacksmith to repair armour".
        /// In both cases, we want to parse this as repair armour. So in this case we look for the word
        /// "repair " and discard anything before it.
        /// </remarks>
        private (string UppercaseInput, string OriginalInput) preprocessInput_Anywhere(string uppercaseInput, string originalInput, List<string> synonyms)
        {
            // We see if the input starts with a synonym...
            var matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.StartsWith(x));
            if (matchingSynonym != null)
            {
                return (uppercaseInput, originalInput);
            }

            // The text does not start with any of the synonyms, so we check if the text contains them.
            // Note that in this case we look for them with an extra space before the command - eg, " repair " - 
            // to make sure that we are getting a whole word.
            matchingSynonym = synonyms.FirstOrDefault(x => uppercaseInput.Contains($" {x}"));
            if (matchingSynonym == null)
            {
                return (uppercaseInput, originalInput);
            }

            // We have found the command, so we remove any text before it...
            var index = uppercaseInput.IndexOf(matchingSynonym);
            return (uppercaseInput.Substring(index), originalInput.Substring(index));
        }

        #endregion

        #region Private data

        // Compass directions...
        private readonly HashSet<string> m_directions = new HashSet<string> { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "UP", "DOWN" };

        // Enum used when parsing input to determine where we look for command words.
        private enum CommandPosition
        {
            // We only look for commands at the start of the input...
            START,

            // We look for commands anywhere in the input, and discard anything before the command...
            ANYWHERE
        }
        
        #endregion
    }
}
