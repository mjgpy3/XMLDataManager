using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// Acts as a brain to the GBaseHandler and probably others.
    /// </summary>
    class GBaseBrain
    {
        private XMLReader _xmlIn;           // Used to read XML
        private String _location = ".\\";   // Where the file is stored, for now, next to the .exe file
        private String _separator = "-";    // What to put between table and model in the table filename
        private String _extension = ".gbs"; // The extension to use

        /// <summary>
        /// The default constructor... It's got some skills.
        /// </summary>
        public GBaseBrain()
        {
            ////xmlOut = new XMLWriter();          // Instantiate stuff
            _xmlIn = new XMLReader();
            //_evalXML = new GBaseXMLEvaluator();
        }

        /// <summary>
        /// Check if the passed file is a GBase model
        /// </summary>
        /// <param name="file">The name of the file to inspect</param>
        /// <returns></returns>
        public bool FileIsModel(String file)
        {
            return FileIsTable(file, "model");   // Spoof the input to "FileIsTable()"... Sneaky...
        }

        /// <summary>
        /// Tells whether a file bears the table headers of GBase
        /// </summary>
        /// <param name="file">The name of the file to inspect</param>
        /// <param name="attr">You probably shouldn't change this...</param>
        /// <returns>True if the file bears a table header for GBase</returns>
        public bool FileIsTable(String file, String attr = "table")
        {
            _xmlIn.Open(file);  // Open the file

            if (attr.CompareTo(_xmlIn.GetValueFromUniqueTag("GBase", "Type")) == 0)     // See if the Type attribute has the value attr
            {
                _xmlIn.Close();     // Close the file
                return true;        // Yes it is
            }
                    
            _xmlIn.Close();         // Otherwise... Close the file
            return false;           // No it's not
        }

        /// <summary>
        /// Checks if a tuple is a subset of another, only considering certain locations
        /// </summary>
        /// <param name="allValues">The Set to check against</param>
        /// <param name="valuesToCheck">The subset, whose values should be equal to the parent at the specified indicies</param>
        /// <param name="indicies">Where to check for said equality</param>
        /// <returns></returns>
        public bool ValuesMatchAtDesiredIndicies(String[] allValues, String[] valuesToCheck, int[] indicies)
        {
            for (int i = 0; i < valuesToCheck.Length; i++)          // For all values in the subset
                if (allValues[indicies[i]] != valuesToCheck[i])     // If an element doesn't match up to the "parent" set
                    return false;                                   // Then return false

            return true;                                            // Else return true
        }

        /// <summary>
        /// Ensure that the passed attributes match the passed value pairs
        /// </summary>
        /// <param name="attr">The attributes to check</param>
        /// <param name="elements">The elements to check</param>
        /// <returns>True if they match, false otherwise</returns>
        public bool AttrsMatchFieldNames(String[] attr, GBaseElement[] elements)
        {
            return !GBaseElementsToStrings(elements).Except(attr).Any();    // If the set difference doesn't have any elements (they are same) return true, else fail
        }

        /// <summary>
        /// Makes sure that the passed attributes are a subset of the passed elements
        /// </summary>
        /// <param name="attr">What will be checked as a subset</param>
        /// <param name="elements">What will be checked as a parent set</param>
        /// <returns>True if the attributes are a subset, false elsewise</returns>
        public bool AttrsSubsetOfFieldNames(String[] attr, GBaseElement[] elements)
        {
            return !attr.Except(GBaseElementsToStrings(elements)).Any();    // If the set difference doesn't have any elements (they are same) return true, else fails
        }

        /// <summary>
        /// Returns an array of the indicies of the locations of desired attributes
        /// </summary>
        /// <param name="attr">The attributes for which we are finding the indicies</param>
        /// <param name="elements">The GBase elements that show the structure of data</param>
        /// <returns>An array of indicies parallel to the desired attributes, specifying where they are in the structure</returns>
        public int[] GetIndiciesOfAttributesFromGBaseElements(String[] attr, GBaseElement[] elements)
        {
            int[] result = new int[attr.Length];            // Make a new array that will store the indicies

            for (int k = 0; k < attr.Length; k++)           // For all elements in attributes
                for (int i = 0; i < elements.Length; i++)   // For all elements in elements
                    if (elements[i].Name == attr[k])        // If we find the location of the corrisponding element in the structure
                        result[k] = i;                      // Then add it to the array

            return result;                                  // Return the array
        }

        /// <summary>
        /// Converts an array of GBaseElements into a string (by element name)
        /// </summary>
        /// <param name="elements">What to convert</param>
        /// <returns>An array of strings converted from the GBase elements</returns>
        private String[] GBaseElementsToStrings(GBaseElement[] elements)
        {
            String[] temp = new String[elements.Length];        // Make a new string array to hold the values
            for (int i = 0; i < elements.Length; i++)           // For each thing in elements
                temp[i] = elements[i].Name;                     // Add that thing to the temp string array

            return temp;                                        // Return the temp string array
        }

        /// <summary>
        /// Sorts a given array of values in the order of the matching elements
        /// </summary>
        /// <param name="attrs">The attributes that corrispond to the values</param>
        /// <param name="values">The values to be sorted</param>
        /// <param name="elements">What is being sorted with dues to</param>
        public void SortValuesDueToFieldNames(String[] attrs, ref String[] values, GBaseElement[] elements)
        {
            String[] temp = new String[elements.Length];        // Will store the values
            for (int i = 0; i < elements.Length; i++)           // Loop each element
            {
                for (int k = 0; k < attrs.Length; k++)      // Foreach element find where it ought to go
                {
                    if (attrs[k] == elements[i].Name)       
                        temp[i] = values[k];                // Put it there (in temp)
                }
            }

            values = temp;                                  // Values becomes temp
        }

        /// <summary>
        /// Returns a GBase table name as specified by the parameters
        /// </summary>
        /// <param name="table">The name of the table</param>
        /// <param name="model">The name fo the model (GBase)</param>
        /// <returns></returns>
        public String GetTableFileName(String table, String model)
        {
            return _location + table.ToLower() + _separator + model.ToLower() + _extension;
        }

        /// <summary>
        /// Gets the file name of the specified model (GBase)
        /// </summary>
        /// <param name="model">The name of the GBase</param>
        /// <returns></returns>
        public String GetModelFileName(String model)
        {
            return _location + model.ToLower() + _extension;
        }
    }
}
