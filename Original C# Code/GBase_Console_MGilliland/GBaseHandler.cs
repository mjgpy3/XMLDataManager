using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// In charge of handling operations with GBase Files
    /// </summary>
    class GBaseHandler
    {
        private XMLWriter _xmlOut;          // Used to write XML
        private XMLReader _xmlIn;           // Used to read XML
        private GBaseXMLEvaluator _evalXML; // Used to evaluate non-file IO XML
        private String _use;                // Tells which MBase is currently being used
        private GBaseBrain _brain;          // Performs crucial operations for the GBase
        private GBaseOutputHandler _output; // Keeps track of output for the GBase

        /// <summary>
        /// Default constructor, it's pretty good with a bowstaff...
        /// </summary>
        public GBaseHandler()
        {       
            _xmlOut = new XMLWriter();          // Instantiate stuff
            _xmlIn = new XMLReader();
            _evalXML = new GBaseXMLEvaluator();
            _brain = new GBaseBrain();
            _output = new GBaseOutputHandler();
        }

        /// <summary>
        /// Gets the output buffer associated with the current GBase object
        /// </summary>
        /// <returns>The output from the GBase</returns>
        public String GetOutput()
        {
            return _output.GetOutputBuffer();
        }

        /// <summary>
        /// Switch the current database in use
        /// </summary>
        /// <param name="toMe">The name of the GBase to switch to</param>
        /// <returns></returns>
        public void Use(String toMe)
        {
            if (_brain.FileIsModel(_brain.GetModelFileName(toMe)))
            {
                _use = toMe.ToLower();  // Set the current GBase
                _output.EndBufferWithText("Changed current MBase to \"" + toMe + "\"");
            }
            else
                _output.EndBufferWithText("Unable to use \"" + toMe + "\"\nAre you sure it is an existing GBase?");
        }

        /// <summary>
        /// Make a GBase
        /// </summary>
        /// <param name="name">The name of the GBase that you are creating</param>
        /// <returns>The output of the attempted create</returns>
        public void GenGBase(String name)
        {
            _xmlOut.Open(_brain.GetModelFileName(name));      // Open a "new" GBase file for writing

            // Write out the beginning header stuff

            _xmlOut.BeginTagWithAttributes("GBase", new String[] {"Type"}, new String[] {"model"}); 
            _xmlOut.BeginTag("HeaderInfo");
            _xmlOut.WriteTagAndContents("Name", name.ToLower());
            // Try to get the current user's profile name but if it fails make the field empty
            try
            {
                _xmlOut.WriteTagAndContents("Creator", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
            catch
            {
                _xmlOut.WriteTagAndContents("Creator");
                _output.EndBufferWithText("Failed to find creator");       // Update feedback upon fail
            }

            // Write out the rest of the bare file, updating feedback as you go
            _xmlOut.WriteTagAndContents("CreationDate", DateTime.Today.ToString());
            _xmlOut.WriteTagAndContents("Access", "A");
            _xmlOut.EndTag("HeaderInfo");

            _output.AddOutput("Wrote MBaseHeaders for \"" + name + "\"");

            _xmlOut.BeginTag("TableData");
            _xmlOut.EndTag("TableData");
            _xmlOut.EndTag("GBase");

            _output.AddOutput("Wrote Empty TableData for \"" + name + "\"");
            _output.EndBufferWithText("***Successfully created \"" + name + "\"");

            _xmlOut.Close();        // Close the file

            Use(name);            // Switch use to the new database
        }

        /// <summary>
        /// Make a table in the current MBase in use
        /// </summary>
        /// <param name="name">The name of the table to be created</param>
        /// <returns>The output of the attempted create</returns>
        public void GenTable(string name)
        {
            // Write out the new table's headers into the model file and update the feedback
            _xmlOut.AddBeginTagWithAttributesAtFootOfSection(_brain.GetModelFileName(_use), "TableData", "TableInstance", new String[] {"Name"}, new String[] { name.ToLower() });
            _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetModelFileName(_use), "TableData", "Location", _brain.GetTableFileName(name, _use), 2);
            _xmlOut.AddEndTagAtFootOfSection(_brain.GetModelFileName(_use), "TableData", "TableInstance");

            _output.AddOutput("Added new table data to MBase file for \"" + name + "\"");

            _xmlOut.Open(_brain.GetTableFileName(name, _use));     // Open the newly created file

            // Write out some header info to the new table file
            _xmlOut.BeginTagWithAttributes("GBase", new String[] { "Type" }, new String[] { "table" });
            _xmlOut.BeginTag("TableHeaderInfo");
            _xmlOut.WriteTagAndContents("Name", name.ToLower());

            // Try to write out the creator, if it fails write an empty tag and keep going
            try
            {
                _xmlOut.WriteTagAndContents("Creator", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
            catch
            {
                _xmlOut.WriteTagAndContents("Creator");
                _output.AddOutput("Failed to find creator");
            }

            // Write the rest of the blank table data
            _xmlOut.EndTag("TableHeaderInfo");
            _xmlOut.BeginTag("TableFields");
            _xmlOut.EndTag("TableFields");
            _xmlOut.BeginTag("TableData");
            _xmlOut.EndTag("TableData");

            _xmlOut.EndTag("GBase");

            _output.EndBufferWithText("***Created the table \"" + name + "\"");

            _xmlOut.Close();       // Close the file
        }

        /// <summary>
        /// Generates a new attribute (col) in the specified table in the current GBase
        /// </summary>
        /// <param name="colName">The name of the new column</param>
        /// <param name="colType">The type that the column will be</param>
        /// <param name="inThisTable">The name of the table to write it to</param>
        public void GenColIn(String colName, String colType, String inThisTable)
        {
            try
            {
                _xmlIn.Open(_brain.GetModelFileName(_use));
            }
            catch
            {
                _output.EndBufferWithText("Failed to open Gbase file.\nCommand: use [mbaseFileName]");
            }

            // Make sure that the model has record of the table before proceding
            if (_xmlIn.TagExistsWithAttributes("TableInstance", new String[] { "Name" }, new String[] { inThisTable.ToLower() }))
            {
                _output.AddOutput("Found the file in GBase Model");

                // Make sure that the table file has the appropriate header and is not a spoof
                if (_brain.FileIsTable(_brain.GetTableFileName(inThisTable, _use)))
                {
                    // Write out the field thingy at the bottom of the tablefields section
                    _xmlOut.AddBeginTagWithAttributesAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableFields", "Field", new String[] { "Name" }, new String[] { colName.ToLower() });
                    _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableFields", "Type", colType, 2);
                    _xmlOut.AddEndTagAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableFields", "Field");
                    _output.AddOutput("Wrote header information");
                }
                else
                    _output.EndBufferWithText("Table not in proper GBase format");

                _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "Entry", "D", "");
                _output.EndBufferWithText("***Successfully created new col");
            }
            else
                _output.EndBufferWithText("Could not find the file in GBase Model");
        }

        /// <summary>
        /// Generates a new row (entity) with the specified parallel attributes and values in the specified table in the GBase in use
        /// </summary>
        /// <param name="attrs">A list of attributes which must be in the table</param>
        /// <param name="values">A parallel list of values for each of the attributes</param>
        /// <param name="inThisTable">The name of the table to generate the row in</param>
        public void GenRowIn(String[] attrs, String[] values, String inThisTable)
        {
            String tempString;
            GBaseElement[] tempElements;

            // May Be Deletable
            for (int i = 0; i < attrs.Length; i++)
                attrs[i] = attrs[i].ToLower();

            try
            {
                _xmlIn.Open(_brain.GetModelFileName(_use));
            }
            catch
            {
                _output.EndBufferWithText("Failed to open GBase file.\nCommand: use [mbaseFileName]");
            }

            // Make sure that the model has record of the table before proceding
            if (_xmlIn.TagExistsWithAttributes("TableInstance", new String[] { "Name" }, new String[] { inThisTable.ToLower() }))
            {
                _output.AddOutput("Found the file in GBase Model");

                // Make sure that the table file has the appropriate header and is not a spoof
                if (_brain.FileIsTable(_brain.GetTableFileName(inThisTable, _use)))
                {
                    _xmlIn.Open(_brain.GetTableFileName(inThisTable, _use));

                    _output.AddOutput("Found the correct GBase table file");

                    tempString = _xmlIn.GetXMLBetweenTags("TableFields");           //  Get the fields from the table
                    tempElements = _evalXML.FieldsToGBaseElementArray(tempString);  //  Get the attributes from the fields

                    Array.Reverse(tempElements);    // Reverse the tempelements, my stack made me do this!

                    _xmlIn.Close();

                    // Make sure that the passed attributes are really in the table
                    if (_brain.AttrsMatchFieldNames(attrs, tempElements))
                    {
                        _output.AddOutput("Verified the field names");

                        try
                        {
                            _brain.SortValuesDueToFieldNames(attrs, ref values, tempElements);      // Sort the values based on what we read from the table
                        }
                        catch
                        {
                            _output.EndBufferWithText("Failed to sort values correctly");
                        }
                        
                        _output.AddOutput("Sorted the values correctly");


                        try
                        {
                            _xmlOut.AddBeginTagWithAttributesAtFootOfSection(_brain.GetTableFileName(inThisTable, _use),    // Write entry tag
                                "TableData", "Entry", new String[] { }, new String[] { });

                            foreach (String v in values)        // Write out the new data
                                _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "D", v, 2);

                            _xmlOut.AddEndTagAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "Entry"); // Write entry end tag
                        }
                        catch
                        {
                            _output.EndBufferWithText("Data was unsuccessfully written\nEnsure that format of table is proper");
                        }

                        _output.EndBufferWithText("***Wrote data to GBase successfully");

                    }
                    else
                        _output.EndBufferWithText("At least one specified attribute does not exist in the table");

                }
                else
                    _output.EndBufferWithText("Could not find a suitable table file");
            }
            else
                _output.EndBufferWithText("Could not find file in GBase Model");

            _xmlIn.Close();
        }

        /// <summary>
        /// Gets an array list of tuples meeting the requirements passed from the specified table, a * for the first attribute gets all
        /// </summary>
        /// <param name="attrs">A list of the attributes to get, a * as the first one gets all</param>
        /// <param name="values">A parallel list of the values that go with each attribute</param>
        /// <param name="inThisTable"></param>
        /// <returns>An ArrayList of string arrays meeting the requirements met by the query</returns>
        public ArrayList GetRowWith(String[] attrs, String[] values, String inThisTable)
        {
            String tempString;                          // Used to store XML temporarily
            GBaseElement[] tempElements;                // Will store attributes in table 
            int[] indiciesOfDesiredAttrs;               // Will store the indicies of the attributes we want to check
            ArrayList allData;                          // Will store all data from the table
            ArrayList result = new ArrayList();         // Will store the result if a * is not given for the first attribute
            String[] tempTuple = new String[] {};       // Will store a tuple temporarily

            for (int i = 0; i < attrs.Length; i++)      // Make all attributes lowercase
                attrs[i] = attrs[i].ToLower();

                                                        // Try to open the file
            try
            {
                _xmlIn.Open(_brain.GetModelFileName(_use));
            }
            catch
            {
                _output.EndBufferWithText("Failed to open Gbase file.\nCommand: use [mbaseFileName]\n");
            }

            // Make sure that the model has record of the table before proceding
            if (_xmlIn.TagExistsWithAttributes("TableInstance", new String[] { "Name" }, new String[] { inThisTable.ToLower() }))
            {
                _output.AddOutput("Found the file in GBase Model");

                // Make sure that the table file has the appropriate header and is not a spoof
                if (_brain.FileIsTable(_brain.GetTableFileName(inThisTable, _use)))
                {
                    _xmlIn.Open(_brain.GetTableFileName(inThisTable, _use));

                    _output.AddOutput("Found the correct GBase table file");

                    tempString = _xmlIn.GetXMLBetweenTags("TableFields");           // Get the TableFields
                    tempElements = _evalXML.FieldsToGBaseElementArray(tempString);  // Use them to get the temp elements

                   _xmlIn.Close();


                    // Make sure that all of the passed attributes are actual attributes
                    if (_brain.AttrsSubsetOfFieldNames(attrs, tempElements) || attrs[0] == "*")
                    {
                        _xmlIn.Open(_brain.GetTableFileName(inThisTable, _use));

                        _output.AddOutput("Verified that all attributes are in table");
                        Array.Reverse(tempElements);        // Reverse the temp elements due to stack use

                        tempString = _xmlIn.GetXMLBetweenTags("TableData");     // Get the data

                        allData = _evalXML.EntriesTo2DArrayList(tempString, tempElements.Length);   // Convert the data to a 2D arrayList

                        if (attrs[0] == "*")    // If the first attribute is a * then we stop right here and RETURN ALL THE DATA!!!!
                        {
                            _xmlIn.Close();
                            return allData;
                        }
                        
                        _output.EndBufferWithText("***Successfully queried GBase!");

                        indiciesOfDesiredAttrs = _brain.GetIndiciesOfAttributesFromGBaseElements(attrs, tempElements);  // Find the places the user wants to check

                        for (int i = 0; i < allData.Count; i++)         // Loop through each one and if it matches the place that the user wants, add it to be outputted
                            if (_brain.ValuesMatchAtDesiredIndicies((String[]) allData[i], values, indiciesOfDesiredAttrs))
                                result.Add(allData[i]);

                        _xmlIn.Close();
                    }
                }
                else
                    _output.EndBufferWithText("Table file not in correct format\n");
            }
            else
                _output.EndBufferWithText("Could not find the table \"" + inThisTable + "\"");

            return result;      // Return the data!
        }

        /// <summary>
        /// Deletes a GBase
        /// </summary>
        /// <param name="GBaseName">The name of the GBase to delete</param>
        public void DelGBase(String GBaseName)
        {
            String XMLOfInterest;

            if (_brain.FileIsModel(_brain.GetModelFileName(GBaseName)))       // Make sure that the gbase were are trying to delete is truly a GBase
            {
                _xmlIn.Open(_brain.GetModelFileName(GBaseName));
                XMLOfInterest = _xmlIn.GetXMLBetweenTags("TableData");  // Get the table instances
                _xmlIn.Close();

                foreach (String location in _evalXML.GetLocationsFromTableInstances(XMLOfInterest))     // Delete each of the files found in the model header file
                    File.Delete(location);

                File.Delete(_brain.GetModelFileName(GBaseName));      // Delete the model header file
            }
            else
                _output.EndBufferWithText("Could not find GBase: \"" + GBaseName + "\"");
        }

        /// <summary>
        /// Deletes a table in the current GBase
        /// </summary>
        /// <param name="tableName">The name of the table to delete</param>
        public void DelTable(String tableName)
        {
            String XMLOfInterest;

            if (_brain.FileIsTable(_brain.GetTableFileName(tableName, _use)))      // Make sure that the table exists
            {
                _xmlIn.Open(_brain.GetTableFileName(tableName, _use));
                XMLOfInterest = _xmlIn.GetXMLBetweenTags("TableData");      // Get the locations
                _xmlIn.Close();

                XMLOfInterest = XMLOfInterest.Substring(XMLOfInterest.IndexOf("Name=\"" + tableName.ToLower() + "\""));     // Cut from the beginning the the correct place

                XMLOfInterest = _evalXML.GetXMLBetweenTags("Location", XMLOfInterest);  // Get the location

                File.Delete(XMLOfInterest);                                             // Delete the file

                _xmlOut.WriteWithoutTagWithName(_brain.GetModelFileName(_use), "TableInstance", tableName.ToLower());

            }
            else
                _output.EndBufferWithText("Could not find table: \"" + tableName + "\"");
        }

        /// <summary>
        /// Deletes a row that matches the given parameters in the specified table
        /// </summary>
        /// <param name="attrs">The attributes to filter by</param>
        /// <param name="values">The values that are parallel to the attributes passed</param>
        /// <param name="inThisTable">The table to delete the row in</param>
        public void DelRowWith(String[] attrs, String[] values, String inThisTable)
        {
            ArrayList toWrite = GetRowWith(new String[] { "*" }, new String[] { }, inThisTable);                        // Gets all elements and verifies the table
            ArrayList minusMe = new ArrayList();                                                                        // Stores the elements to get rid of
            String fileContents = _xmlIn.GetEntireFile(_brain.GetTableFileName(inThisTable, _use));                     // Gets the file contents (I know it's aweful)
            GBaseElement[] tempElements = _evalXML.FieldsToGBaseElementArray(_evalXML.GetXMLBetweenTags("TableFields",  // Gets and sets the Attributes
                                                                             fileContents));
            StreamWriter writer;                                                                                        // Will be used later to write

            for (int i = 0; i < attrs.Length; i++)      // Make all attributes lowercase, GBase is case insensitive!
                attrs[i] = attrs[i].ToLower();

            Array.Reverse(tempElements);                // Reverse the attributes, stacks tend to mess with order

            int[] indiciesOfDesiredAttrs = _brain.GetIndiciesOfAttributesFromGBaseElements(attrs, tempElements);    // Find the indicies that we want to check for removal

            for (int i = 0; i < toWrite.Count; i++)                                                                 // Loop through each entry adding ones that match the 
                if (_brain.ValuesMatchAtDesiredIndicies((String[])toWrite[i], values, indiciesOfDesiredAttrs))      // user's requirements to the minusMe array
                    minusMe.Add(toWrite[i]);

            foreach (String[] tuple in minusMe)     // Remove everything specified by minusMe
                toWrite.Remove(tuple);

            writer = new StreamWriter(_brain.GetTableFileName(inThisTable, _use));                      // Rewrite everything except for the actual tabledata
            writer.Write(fileContents.Substring(0, fileContents.IndexOf("<TableData>") + 11) + "\n");
            writer.Write("  " + fileContents.Substring(fileContents.IndexOf("</TableData>")));
            writer.Close();

            foreach (String[] tuple in toWrite)     // Write each tuple back that hasn't been subtracted
            {
                _xmlOut.AddBeginTagWithAttributesAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "Entry", new String[] { }, new String[] { });
                foreach (String element in tuple)
                    _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "D", element, 2);

                _xmlOut.AddEndTagAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "Entry");
            }
        }

        /// <summary>
        /// Deletes a col in requested table in the current GBase
        /// </summary>
        /// <param name="colName">The name of the col to be deleted</param>
        /// <param name="inThisTable">The name of the table to delete the col from</param>
        public void DelColIn(String colName, String inThisTable)
        {
            int desiredIndex,                                       // Will store the location of the data within the entry tag
                runningIndex = 0;                                   // Will be used to loop

            ArrayList allData = GetRowWith(new String[] { "*" },    // Get all data, also verifies that the file is a table
                new String[] { "" },
                inThisTable.ToLower());

            GBaseElement[] tempElements;                            // Will store the fields

            tempElements = _evalXML.FieldsToGBaseElementArray(_evalXML.GetXMLBetweenTags("TableFields", // Set the temp elements
                _xmlIn.GetEntireFile(_brain.GetTableFileName(inThisTable, _use))));

            Array.Reverse(tempElements);    // Reverse them, since a stack is used they come out in the wrong order

            desiredIndex = _brain.GetIndiciesOfAttributesFromGBaseElements(new String[] { colName.ToLower() }, tempElements)[0];    // Get the index (location) of the column we are deleting

            _xmlOut.WriteWithoutTagWithName(_brain.GetTableFileName(inThisTable, _use), "Field", colName.ToLower());    // Writes back to the table header without the specified field

            _xmlOut.WriteWithoutTag(_brain.GetTableFileName(inThisTable, _use), "Entry");   // Writes back to the model with no entries

            if (tempElements.Length != 1)   // If there is only one element left, if it made it though the rest of the above code, it is the last one and therefore there are no entries to write back
            {
                foreach (String[] tuple in allData)
                {
                    _xmlOut.AddBeginTagWithAttributesAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "Entry", new String[] { }, new String[] { });     // Write a new entry begin tag
                    foreach (String element in tuple)                       // Go through each object in the tuple
                        if (runningIndex++ != desiredIndex)                 // If the element is not the one we want to delete, then write it back to the GBase
                            _xmlOut.AddTagAndContentsAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "D", element, 2);

                    _xmlOut.AddEndTagAtFootOfSection(_brain.GetTableFileName(inThisTable, _use), "TableData", "Entry");     // Add an end tag

                    runningIndex = 0;   // Restart running index
                }
            }
        }

        /// <summary>
        /// Modifies an entry with attributes having certain values in a table so that attributes will have values
        /// </summary>
        /// <param name="withAttrs">(Query data) The attributes parallel to the values to find before changing</param>
        /// <param name="withValues">(Query data) The values that corrispond to the above attributes to find before making changes</param>
        /// <param name="inThisTable">The table to modify rows in</param>
        /// <param name="thatAttr">The attributes parallel to the values that are to be changed</param>
        /// <param name="thatValues">The values parallel to the attributes that are to be changed</param>
        public void ModRowWith(String[] withAttrs, String[] withValues, String inThisTable, String[] thatAttr, String[] thatValues)
        {
            ArrayList toChange = GetRowWith(withAttrs, withValues, inThisTable);    // Queries the GBase for all entries matching the ones that the user wants to mod, and in doing so verifies the tablefile
            GBaseElement[] tempElements;                                            // Will store the fields
            int[] desiredIndicies;                                                  // Will store the indicies of the fields that will be modded

            for (int i = 0; i < withAttrs.Length; i++)  // Make all attributes lower, GBase is case insensitive!
                withAttrs[i] = withAttrs[i].ToLower();

            for (int i = 0; i < thatAttr.Length; i++)   // Make all attributes lower, GBase is case insensitive!
                thatAttr[i] = thatAttr[i].ToLower();

            tempElements = _evalXML.FieldsToGBaseElementArray(_evalXML.GetXMLBetweenTags("TableFields",     // Fills tempElements with the fields in the table
                _xmlIn.GetEntireFile(_brain.GetTableFileName(inThisTable, _use))));

            Array.Reverse(tempElements);        // Reverse the elements (since a stack is used they come out backwards)

            desiredIndicies = _brain.GetIndiciesOfAttributesFromGBaseElements(thatAttr, tempElements);  // Get the indicies of the attributes we want to change

            DelRowWith(withAttrs, withValues, inThisTable);         // Deletes any rows that have the attr:val pairs that the user specified

            String[] allAttrs = new String[tempElements.Length];    // Will store all attributes, is the length of the number of cols

            for (int i = 0; i < tempElements.Length; i++)           // Set all attributes to be the attributes
                allAttrs[i] = tempElements[i].Name;

            foreach (String[] tuple in toChange)                    // Loop through each tuple in to change
            {
                for (int i = 0; i < desiredIndicies.Length; i++)    // Loop through each desired index to change
                    tuple[desiredIndicies[i]] = thatValues[i];

                GenRowIn(allAttrs, tuple, inThisTable);             // Write the col out to the bottom of the file
            }
        }
    }
}
