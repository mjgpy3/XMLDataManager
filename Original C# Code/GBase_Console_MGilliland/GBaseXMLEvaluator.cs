using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// Evaluate non-fileIO XML
    /// </summary>
    class GBaseXMLEvaluator
    {
        private GBaseElement _GBaseElement;     // Stores "Name-Value Pairs" for later use and abuse

        /// <summary>
        /// Converts an XMLstring to an array of GBaseElement (Name and Type) structures
        /// </summary>
        /// <param name="XMLString">An XML string of the form: field..type..field..type...</param>
        /// <returns>An array of GBaseElement that is the parsed, passed XMLstring</returns>
        public GBaseElement[] FieldsToGBaseElementArray(String XMLString)
        {
            Stack<GBaseElement> _elements = new Stack<GBaseElement>();                      // Initially use a stack to hold the elements
            XMLString = XMLString.Replace("\t", "").Replace("\r", "").Replace("\n",  "");   // Replace the tabs, newllines, and returns in the string

            // Loop until break is hit
            while (true)
            {
                _GBaseElement.Name = XMLString.Substring(XMLString.IndexOf("Name=\"") + 6,      // Set the name to be the string between "Name=\"" and the next "
                                                         XMLString.IndexOf("\"", XMLString.IndexOf("Name=\"") + 6) - (XMLString.IndexOf("Name=\"") + 6)).ToLower();
                    
                                                                                                // Set the Type to be the string between <Type> and </Type>
                _GBaseElement.Type = GetXMLBetweenTags("Type", XMLString).ToLower();

                _elements.Push(_GBaseElement);  // Push the newly generated object into _elements

                XMLString = XMLString.Substring(XMLString.IndexOf("</Field>") + 8,              // Get rid of the portion of the string that has been evaluated
                                                XMLString.Length - (XMLString.IndexOf("</Field>") + 8));

                if (XMLString.Replace(" ", "").Length == 0)     // Get rid of any left over spaces (sometimes they presist) and see if there is no more to look over
                    break;
            }

            return _elements.ToArray();     // Return the stack as an array
        }
        
        /// <summary>
        /// Coverts the passed XML code (should be between two entries) to an ArrayList of String arrays (I call these tuples FAIAP)
        /// </summary>
        /// <param name="XMLCode">The XML between the TableData tags</param>
        /// <param name="numberOfFields">The number of fields we are looking for</param>
        /// <returns>An ArrayList of tuples (string arrays of each entry)</returns>
        public ArrayList EntriesTo2DArrayList(String XMLCode, int numberOfFields)
        {
            ArrayList returnMe = new ArrayList();                           // Will store the tuples to return
            String tempString = "";                                         // Will be used to store each entry (in string form)
            int LengthToCut;                                                // Will be used to later how much from XMLCode to cut away

            while (XMLCode.Trim().CompareTo(String.Empty) != 0)             // Keep Looping until the XML string is empty
            {
                tempString = GetXMLBetweenTags("Entry", XMLCode);           // Set tempString to the data between the first Entry tags
                String[] tempTuple = new String[numberOfFields];            // Make a new tuple of the length of the number of fields in the table

                LengthToCut = tempString.Length;                            // Get how much to cut from XMLCode (the entries)

                for (int i = 0; i < numberOfFields; i++)                    // Foreach field specified by numberOfFields
                {
                    tempTuple[i] = GetXMLBetweenTags("D", tempString);                  // Set each location in tempTuple to what we get between the data tags 
                    tempString = tempString.Substring(tempString.IndexOf("</D>") + 4);  // Cut off the data tags
                }

                returnMe.Add(tempTuple);                                        // Add the tuple to returnMe

                XMLCode = XMLCode.Substring(LengthToCut);                       // Cut of the desired amount from XMLCode
                XMLCode = XMLCode.Substring(XMLCode.IndexOf("</Entry>") + 8);   // Also get rid of the ending Entry tag
            }

            return returnMe;    // Return it!
        }

        /// <summary>
        /// Gets an array of strings of the locations of tables from code in the model
        /// </summary>
        /// <param name="XMLCode">The code between the TableData tags from the model file</param>
        /// <returns>A string array of all the locations of the tables in the GBase</returns>
        public String[] GetLocationsFromTableInstances(String XMLCode)
        {
            String[] locations = new String[] { };      // Holds the result

            for (int i = 0; XMLCode.Trim().CompareTo(String.Empty) != 0; i++)   // Keep looping until the XML code is nothing
            {
                Array.Resize(ref locations, locations.Length + 1);                              // Make the string array 1 element bigger
                locations[i] = GetXMLBetweenTags("Location", XMLCode);                          // Get the location
                XMLCode = XMLCode.Substring(XMLCode.IndexOf("</TableInstance>") + 16).Trim();   // Make XMLCode 1 location tag/endtag smaller
            }

            return locations;   // Return it!
        }

        /// <summary>
        /// Gets the value of a specific attribute, in the passed tag from the passed XML code
        /// </summary>
        /// <param name="tagName">The tag to find the attribute in</param>
        /// <param name="attributeName">The attribute that the value corrisponds to</param>
        /// <returns></returns>
        public String GetValueFromUniqueTag(String tagName, String attributeName, String XMLCode)
        {
            String value = "$#@!@#$???MIKEYSenTEnIAlYo";        // Declare a "snazzy sentenial"

            foreach (String line in XMLCode.Split('\n'))        // Loop through each line in the file
            {
                if (line.IndexOf("<" + tagName) != -1)          // If we find the tag that we are looking for
                {
                    String[] crucialStrings = line.Substring(line.IndexOf(' '),     // Take from the line an array of everything between <tagname and > as separated by would-be spaces
                                                             line.IndexOf('>') - line.IndexOf(' ')).Split(' ');

                    foreach (String attrValuePair in crucialStrings)                 // Search through each attribute value pair
                        if (attrValuePair.IndexOf(attributeName + "=\"") != -1)       // If we find the one we are looking for
                        {
                            value = attrValuePair.Substring(attrValuePair.IndexOf("\"") + 1,    // Break it out of its string and get outta here! To the next if...
                                                            attrValuePair.LastIndexOf("\"") - attrValuePair.IndexOf("\"") - 1);
                            break;
                        }

                    if (value != "$#@!@#$???MIKEYSenTEnIAlYo")      // If we found the value (sentenial is gone...) then break
                        break;
                }
            }

            return value;   // Return value (or does it?)... May return our beloved "smelly sentenial"
        }

        /// <summary>
        /// Gets a string of anything between the first occurance of the passed tag from the passed XML code
        /// </summary>
        /// <param name="tagName">The tag whose "inners" are desired</param>
        /// <param name="XMLCode">The XML in which the tag resides</param>
        /// <returns>The text between the passed tags in the passed XML code</returns>
        public String GetXMLBetweenTags(String tagName, String XMLCode)
        {
            return XMLCode.Substring((XMLCode.IndexOf("<" + tagName + ">") + 2 + tagName.Length),   // Returns a substring between <tagName> and </tagname>
                                          (XMLCode.IndexOf("</" + tagName + ">")) - (XMLCode.IndexOf("<" + tagName + ">") + 2 + tagName.Length));
        }
    }
}
