using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// Used to read XML files (pretty much non-serially)
    /// </summary>
    class XMLReader
    {
        private StreamReader _reader;       // Used to read in data

        /// <summary>
        /// Simply returns a string of the entire file contents... Could be hazardous, may need to revise...
        /// </summary>
        /// <param name="file">The file to pull in</param>
        /// <returns>A string of the entire file's contents</returns>
        public String GetEntireFile(String file)
        {
            try
            {
                _reader = new StreamReader(file);       // Open the file
            }
            catch (Exception)
            {
                throw new GBaseGeneralException("Could not open \"" + file + "\"");
            }
            String returnMe = _reader.ReadToEnd();  // Read the line into a variable
            _reader.Close();                        // Close the file
            return returnMe;                        // Return the text from the file (I would have done this in one line but I needed to close the file)
        }

        /// <summary>
        /// After .Open(), Gets the value of a specific attribute, in the passed tag
        /// </summary>
        /// <param name="tagName">The tag to find the attribute in</param>
        /// <param name="attributeName">The attribute that the value corrisponds to</param>
        /// <returns>The the value at the first occurance of the specified tag</returns>
        public String GetValueFromUniqueTag(String tagName, String attributeName)
        {
            String value = "$#@!@#$???MIKEYSenTEnIAlYo";        // Declare a "snazzy sentenial"

            foreach (String line in _reader.ReadToEnd().Split('\n'))        // Loop through each line in the file
            {
                if (line.IndexOf("<" + tagName) != -1)          // If we find the tag that we are looking for
                {
                    String[] crucialStrings = line.Substring(line.IndexOf(' '),     // Take from the line an array of everything between <tagname and > as separated by would-be spaces
                                                             line.IndexOf('>') - line.IndexOf(' ')).Split(' '); 

                    foreach(String attrValuePair in crucialStrings)                 // Search through each attribute value pair
                        if (attrValuePair.IndexOf(attributeName+"=\"") != -1)       // If we find the one we are looking for
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
        /// After .Open(), makes sure that a tag exists in the opened file with the given tag, attributes and values exists
        /// </summary>
        /// <param name="tagName">The tag that is searched for</param>
        /// <param name="attrs">Attributes in the string, parallel to the values</param>
        /// <param name="values">Values in the string, parallel to the attributes</param>
        /// <returns>True if the tag was found, else otherwise</returns>
        public bool TagExistsWithAttributes(String tagName, String[] attrs, String[] values)
        {
            for (int i = 0; i < Math.Min(attrs.Length, values.Length); i++)     // Make attrs into what they should look like: attr="value"
                attrs[i] = attrs[i] + "=\"" + values[i] + "\"";

            foreach(String line in _reader.ReadToEnd().Split('\n'))             // Loop through each line of the file
                if (line.IndexOf("<" + tagName) != -1)                          // If we found the tag that we are looking for
                    if (!line.Substring(line.IndexOf(' '), line.IndexOf('>')).Trim().Split().Except(attrs).Except(new String[] { "<" + tagName}).Any()) // If the attribute value pairs are the same
                        return true;        // Return true

            return false;                   // If none were found, return false
        }

        /// <summary>
        /// After .Open(), Looks for a specific tag and returns any XML that is within it
        /// </summary>
        /// <param name="tagName">Tag to look for</param>
        /// <returns>Any XML that lies in between the given tag</returns>
        public String GetXMLBetweenTags(String tagName)
        {
            String fileContents = _reader.ReadToEnd();      // Read the file
            return fileContents.Substring((fileContents.IndexOf("<" + tagName + ">") + 2 + tagName.Length),     // return a substring of the text within the tags
                                          (fileContents.IndexOf("</" + tagName + ">")) - (fileContents.IndexOf("<" + tagName + ">") + 2 + tagName.Length));
        }
        
        /// <summary>
        /// Open an XML file to be read
        /// </summary>
        /// <param name="file">The name of the file to be read</param>
        public void Open(String file)
        {
            try
            {
                _reader = new StreamReader(file);
            }
            catch (Exception)
            {
                throw new GBaseGeneralException("Could not open \"" + file + "\"");
            }
        }

        /// <summary>
        /// Close the current XML file
        /// </summary>
        public void Close()
        {
            try
            {
                _reader.Close();
            }
            catch
            {
            }
        }
    }
}
