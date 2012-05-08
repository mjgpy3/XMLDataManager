using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBase_Console_MGilliland
{
    class GBaseCommandString
    {
        public String Command { set; get; }             // The command used: mod, gen, get, use, help or del
        public String ActsUpon { set; get; }            // What it acts upon: table, gbase, row, col
        public String TableName { set; get; }           // The table's name
        public String GBaseName { set; get; }           // The Gbase name
        public String ColName { set; get; }             // The col's name
        public String[] WithAttributes { set; get; }    // The attributes in a with clause
        public String[] WithValues { set; get; }        // The parallel values in a with clause
        public String[] ThatAttributes { set; get; }    // The attributes in a that clause
        public String[] ThatValues { set; get; }        // The parallel values in a that clause
        public bool ErrorOccured {get; set;}            // Specifies whether an error occured in the command

        /// <summary>
        /// Converts the the passed string to the calling GBaseCommandString object
        /// </summary>
        /// <param name="convertMe">The string to convert</param>
        public void StringToCommandString(String convertMe)
        {
            String[] partsOfCommand = convertMe.Split(' ');     // Get each part of the command separated by spaces

            ResetCommand();                                     // Make the properties blank

      
            switch (partsOfCommand[0].ToLower())                // Switch on the first thing (in lower), I want this to be case insensitive
            {
                case "use":     // If the command is a use
                    GBaseName = GetPropertyName(partsOfCommand, 1, convertMe);      // Set the GBase name from the second word
                    break;
                case "gen":     // If the command is gen
                    SetDelOrGen(partsOfCommand, convertMe);                         // Set the command from the rest of the string
                    break;
                case "get":     // If the command is get
                    SetActsUpon(partsOfCommand, convertMe);                         // Set acts upon
                    SetWithAttributesAndValues(partsOfCommand, convertMe);          // Set the with attributes and values
                    TableName = GetTableNameAfterIn(partsOfCommand, convertMe);     // Get the table name
                    break;
                case "del":     // If the command is del
                    SetDelOrGen(partsOfCommand, convertMe);                         // Set the rest of the command from the string
                    break;
                case "mod":     // If the command is mod
                    SetActsUpon(partsOfCommand, convertMe);                         // Set what the command acts upon
                    SetWithAttributesAndValues(partsOfCommand, convertMe);          // Set the with attributes and values
                    TableName = GetTableNameAfterIn(partsOfCommand, convertMe);     // Get the table name
                    SetThatAttributesAndValues(partsOfCommand, convertMe);          // Set the that attributes and values
                    break;
                case "help":    // If the command is help... Don't do anything
                    break;
                default:        // If the command is unrecognized throw an exception
                    throw new GBaseDoesNotRecognizeCommandException(partsOfCommand[0]);
            }

            Command = partsOfCommand[0].ToLower();                                  // If no exception is thrown before me, set
        }                                                                           // the comand to the first word

        /// <summary>
        /// Used to set the rest of a command that has been found to be del or gen, since they have the same syntax
        /// </summary>
        /// <param name="partsOfCommand">The words of the command in a string array</param>
        /// <param name="commandString">The actuall command as a string</param>
        private void SetDelOrGen(String[] partsOfCommand, String commandString)
        {
            SetActsUpon(partsOfCommand, commandString);

            switch (ActsUpon)       // It should now be safe to use ActsUpon to figure out what we are generating
            {
                case "gbase":       // If it's a gbase we're acting on, set its name
                    GBaseName = GetPropertyName(partsOfCommand, 2, commandString);
                    break;
                case "table":       // If it's a table we're acting on, set its name
                    TableName = GetPropertyName(partsOfCommand, 2, commandString);
                    break;
                case "col":       // If it's a col we're acting on, set its name, and get the table name
                    ColName = GetPropertyName(partsOfCommand, 2, commandString);
                    TableName = GetTableNameAfterIn(partsOfCommand, commandString);
                    break;
                case "row":       // If it's a row we're acting on, set the attribute/value pairs and get the table name
                    SetWithAttributesAndValues(partsOfCommand, commandString);
                    TableName = GetTableNameAfterIn(partsOfCommand, commandString);
                    break;
            }
        }

        /// <summary>
        /// Gets a specific property's name, using the expected location
        /// </summary>
        /// <param name="partsOfCommand"></param>
        /// <param name="wherePropertyNameShouldBe"></param>
        /// <param name="commandString"></param>
        /// <returns></returns>
        private String GetPropertyName(String[] partsOfCommand, int wherePropertyNameShouldBe, String commandString)
        {
            try
            {
                return partsOfCommand[wherePropertyNameShouldBe].ToLower();     // Hazardous since there may be no other word at the desired location

            }
            catch (IndexOutOfRangeException)        // If we find that there is no second word, throw a new 
            {                                       // GBase Exception to be caught somewhere else
                throw new GBaseCommandMustBeFollowedByException(partsOfCommand[wherePropertyNameShouldBe - 1],
                                                                "a GBase table name", commandString.Length);
            }
        }

        /// <summary>
        /// Sets ActsUpon which specifies what kind of thing our command is dealing with
        /// </summary>
        /// <param name="partsOfCommand">A string array of the words of the command</param>
        /// <param name="commandString">The command itself</param>
        private void SetActsUpon(string[] partsOfCommand, String commandString)
        {
            try
            {
                if (partsOfCommand[1].ToLower() == "table" || partsOfCommand[1].ToLower() == "gbase" ||     // Make sure the second word is right, hazardous
                    partsOfCommand[1].ToLower() == "row" || partsOfCommand[1].ToLower() == "col")           // since it may not exist
                    ActsUpon = partsOfCommand[1].ToLower();
                else
                    throw new GBaseCommandMustBeFollowedByException(partsOfCommand[0],
                        "one of the following keywords: GBASE, MODEL, COL, ROW", commandString.IndexOf(partsOfCommand[1]));

            }
            catch (IndexOutOfRangeException)
            {
                throw new GBaseCommandMustBeFollowedByException(partsOfCommand[0],
                    "one of the following keywords: GBASE, MODEL, COL, ROW", commandString.IndexOf(partsOfCommand[0]));
            }
        }

        /// <summary>
        /// Loops through the parts of the command to set the attributes and values in a with clause
        /// </summary>
        /// <param name="partsOfCommand">A string array of the words in the command</param>
        /// <param name="commandString">The command itself</param>
        private void SetWithAttributesAndValues(String[] partsOfCommand, String commandString)
        {
            String[] pairs = GetPairsBetweenWords(partsOfCommand, "with", "in");        // Set pairs to an array of "attr:value"

            if (pairs.Length == 0)  // If there are no attributes and values
                throw new GBaseRowCommandMustHaveWithClauseException();

            String[] tempAttr = new String[pairs.Length], tempValues = new String[pairs.Length];    // New arrays, temps

            if (pairs[0] == "*")                    // If we just have a star then we should 
                tempAttr = new String[] { "*" };
            else                                    // Otherwise set the temps and if they are wrongly formatted the throw
            {
                for (int i = 0; i < pairs.Length; i++)
                {
                    try
                    {
                        tempAttr[i] = pairs[i].Split(':')[0];
                        tempValues[i] = pairs[i].Split(':')[1];
                    }
                    catch
                    {
                        throw new GBaseAttrValuePairsMustBeOfFormException();
                    }
                }
            }
            // Set the attributes and values from the temps
            WithAttributes = tempAttr;
            WithValues = tempValues;
        }

        private void SetThatAttributesAndValues(String[] partsOfCommand, String commandString)
        {
            String[] pairs = GetPairsBetweenWords(partsOfCommand, "that", "SOMETHINGOBSCURE712");   // Get pairs from that to end

            if (pairs.Length == 0)                                          // If there are none then throw an exception
                throw new GBaseModCommandMustHaveThatClauseException();

            String[] tempAttr = new String[pairs.Length], tempValues = new String[pairs.Length];    // Temps

            if (pairs[0] == "*")                    // If the first thing is a star then we should pass it on (it means get all)
                tempAttr = new String[] { "*" };
            else                                    // Otherwise loop through each pair and if they are wrong throw
            {
                for (int i = 0; i < pairs.Length; i++)
                {
                    try
                    {
                        tempAttr[i] = pairs[i].Split(':')[0].ToLower();
                        tempValues[i] = pairs[i].Split(':')[1];
                    }
                    catch
                    {
                        throw new GBaseAttrValuePairsMustBeOfFormException();
                    }
                }
            }
            // Set the attributes and values from the temps
            ThatAttributes = tempAttr;
            ThatValues = tempValues;
        }

        private String[] GetPairsBetweenWords(String[] partsOfCommand, String startWord, String endWord)
        {
            String[] pairs = new String[] { };
            bool addElements = false;
            int index = 0;

            foreach (String part in partsOfCommand)                         // Go through each part of the command
            {
                if (part.ToLower().CompareTo(endWord.ToLower()) == 0)       // If we find the end word the get out
                    break;

                if (addElements)                                            // If we are supposed to be adding elements then add it
                {
                    Array.Resize(ref pairs, pairs.Length + 1);
                    pairs[index++] = part;
                }

                if (part.ToLower().CompareTo(startWord) == 0)               // If we find the start word then begin adding elements on the next go
                    addElements = true;
            }

            return pairs;
        }

        private String GetTableNameAfterIn(String[] partsOfCommand, String commandString)
        {
            int i;
            for (i = 0; i < partsOfCommand.Length; i++)                 // Loop through the command
                if (partsOfCommand[i].ToLower().CompareTo("in") == 0)   // Until we find "IN"
                    return partsOfCommand[i + 1].ToLower();             // Since we've found it return the next thing

            // If it were never found, throw an exception
            throw new GBaseCommandMustBeFollowedByException(partsOfCommand[0], "an IN clause specifying the tablename", commandString.LastIndexOf(partsOfCommand[i - 1]));
        }

        /// <summary>
        /// Resets all the properties
        /// </summary>
        private void ResetCommand()
        {
            Command = ActsUpon = TableName = ColName = GBaseName = "";
            ThatAttributes = WithAttributes = WithValues = ThatValues = new String[] { };
            ErrorOccured = false;
        }
    }
}
