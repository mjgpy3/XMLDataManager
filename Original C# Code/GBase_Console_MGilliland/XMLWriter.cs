using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// Used to handle 2 kinds of XML writing, 1. Open a file and write serially and 2. Jump in and insert
    /// </summary>
    class XMLWriter
    {
        private StreamWriter _writer;       // Will write out to an XML file
        private XMLReader _xmlIn;           // Will read from an XML file
        private int _indentation;           // Specifies the current indentation level (in spaces)

        /// <summary>
        /// Writes out to an XML file... Like an intern.
        /// </summary>
        public XMLWriter()
        {
            _xmlIn = new XMLReader();       // Instantantiate XML reader
            _indentation = 0;               // Indentation starts at 0, dontchya know?
        }

        /// <summary>
        /// After .Open(), writes a beginning tag with attributes and values specified by the parallel array inputs
        /// </summary>
        /// <param name="tagName">The tag that will be written</param>
        /// <param name="attrName">A list of attributes the tag will have, parallel to the values</param>
        /// <param name="attrValue">A list of values the tag will have, parallel to the attributes</param>
        public void BeginTagWithAttributes(String tagName, String[] attrName, String[] attrValue)
        {
            _writer.Write("{1}<{0}", tagName, _padding());                              // Write the beginning of the tag: "_ _ <tagName"

            for (int i = 0; i < Math.Min(attrName.Length, attrValue.Length); i++)       // Write out the attributes: " attrName=\"attrValue\""
                _writer.Write(" {0}=\"{1}\"", attrName[i], attrValue[i]);

            _writer.Write(">\n");                                                       // Write the end tag: ">"

            _indentation += 2;                                                          // Increase the indentation by 2
        }

        /// <summary>
        /// Jumps to the end of a section in an XML file and adds a beginning tag with any attribute/value pairs that may be passed
        /// </summary>
        /// <param name="file">The file to jupm into</param>
        /// <param name="sectionName">The section to insert the tag before the end of</param>
        /// <param name="tagName">The tag to write</param>
        /// <param name="attrName">An array of attributes that the tag will have, parallel to attrValue</param>
        /// <param name="attrValue">An array of values that the tag will have, parallel to attrName</param>
        public void AddBeginTagWithAttributesAtFootOfSection(String file, String sectionName, String tagName, String[] attrName, String[] attrValue)
        {
            String writeMeOut = "";     // What will eventually be written out

            foreach (String line in _xmlIn.GetEntireFile(file).Split('\n'))     // Loop through each line in the file
            {
                if (line.IndexOf("/" + sectionName) != -1)                      // If the section is found
                {
                    writeMeOut += String.Format("{0}<{1}", _padding(line.IndexOf("<") + 2), tagName);    // Append what we want (the tag start)

                    for (int i = 0; i < Math.Min(attrName.Length, attrValue.Length); i++)               // And any attr/value pairs that may have been passed
                        writeMeOut += String.Format(" {0}=\"{1}\"", attrName[i], attrValue[i]); 

                    writeMeOut += ">\n";        // Also add the end tag
                }

                    writeMeOut += line + "\n";      // Add the current line to be written
            }

            StreamWriter _temp_writer = new StreamWriter(file);     // Open the file to jump into

            _temp_writer.Write(writeMeOut.TrimEnd() + "\n");                         // Write

            _temp_writer.Close();                                   // Close
        }

        /// <summary>
        /// Jumps to the end of a section in an XML file and adds an end tag
        /// </summary>
        /// <param name="file">The XMl file to jump into</param>
        /// <param name="section">The section to put the end tag in</param>
        /// <param name="tagName">The name of the end tag to write</param>
        public void AddEndTagAtFootOfSection(String file, String section, String tagName)
        {
            AddBeginTagWithAttributesAtFootOfSection(file, section, "/"+tagName, new String[] { }, new String[] { });   // I cheat!
        }

        /// <summary>
        /// After .Open(), writes a begin tag (with no attributes) to the XML file
        /// </summary>
        /// <param name="tagName">The tag to begin</param>
        public void BeginTag(String tagName)
        {
            _writer.WriteLine("{0}<{1}>", _padding(), tagName);     // Write the tag

            _indentation += 2;      // Increase indentation
        }

        /// <summary>
        /// After .Open(), writes an end of section tag
        /// </summary>
        /// <param name="tagName">The tag to end</param>
        public void EndTag(String tagName)
        {       
            _indentation -= 2;      // Decrease indentation

            _writer.WriteLine("{0}</{1}>", _padding(), tagName);    // Write the tag
        }

        /// <summary>
        /// After .Open(), write out the passed tag and contents within that tag 
        /// </summary>
        /// <param name="tagName">The name of the tag to be passed</param>
        /// <param name="contents">The contents that the tag will store</param>
        public void WriteTagAndContents(String tagName, String contents = "")
        {
            _writer.WriteLine("{2}<{0}>{1}</{0}>", tagName, contents, _padding());      // Write the tag, contents and end tag
        }

        /// <summary>
        /// Jumps (don't use after .Open()) the end of all sections with the passed name and inserts in the tag-innerValue pair, e.g. tag-innerValue-/tag
        /// </summary>
        /// <param name="file">The name of the file to jump into</param>
        /// <param name="sectionName">The section in which you are inserting the tag and content</param>
        /// <param name="tagName">The name of the tag that you are writing</param>
        /// <param name="contents">What the tag will surround</param>
        /// <param name="extraPadding">Any extra padding that you may want</param>
        public void AddTagAndContentsAtFootOfSection(String file, String sectionName, String tagName, String contents, int extraPadding = 0)
        {
            String writeMeOut = "";     // Will store everything that will be writting out to the file

            foreach (String line in _xmlIn.GetEntireFile(file).Split('\n'))     // Loop through each line of the file
            {
                if (line.IndexOf("/"+sectionName) != -1)                        // If we find the end of the desired section
                    writeMeOut += String.Format("{0}<{1}>{2}</{1}>\n", _padding(line.IndexOf("<") + 2 + extraPadding), tagName, contents);  // Before storing the current line, save what is to be inserted

                writeMeOut += line + "\n";      // Add each line (and the extra if we append it) to what will be writting
            }
            StreamWriter _temp_writer = new StreamWriter(file);     // Open the file to be jumped into

            _temp_writer.Write(writeMeOut.TrimEnd() + "\n");        // Write

            _temp_writer.Close();                                   // Close
        }

        /// <summary>
        /// Gets a files data and rewrites it without a specified tag that has a name attribute with the specified name value
        /// </summary>
        /// <param name="file">The file to jump into</param>
        /// <param name="tag">The tag not to write back</param>
        /// <param name="name">The name that the tag has</param>
        public void WriteWithoutTagWithName(String file, String tag, String name)
        {
            String writeMeOut = "";     // Will store everything that will be writting out to the file
            bool keepAppending = true;

            foreach (String line in _xmlIn.GetEntireFile(file).Split('\n'))     // Loop through each line of the file
            {
                if (line.IndexOf("<" + tag + " Name=\"" + name) != -1)   // If we find the end of the desired tableName
                    keepAppending = false;                                      // Then stop appending for a moment   

                if (keepAppending)
                    writeMeOut += line + "\n";      // Add each line unless we are told to stop for a moment...

                if (!keepAppending && line.IndexOf("</" + tag + ">") != -1) // If we find the end of the table instance and we are not supposed to be appendgin
                    keepAppending = true;                                     // Then start appending again
            }
            StreamWriter _temp_writer = new StreamWriter(file);     // Open the file to be jumped into

            _temp_writer.Write(writeMeOut.TrimEnd() + "\n");                         // Write

            _temp_writer.Close();     
        }

        /// <summary>
        /// Reads the contents of a file and rewrites it without a specified tag
        /// </summary>
        /// <param name="file">The file to jump into</param>
        /// <param name="tag">The tag that will not be written back</param>
        public void WriteWithoutTag(String file, String tag)
        {
            String writeMeOut = "";     // Will store everything that will be writting out to the file
            bool keepAppending = true;

            foreach (String line in _xmlIn.GetEntireFile(file).Split('\n'))     // Loop through each line of the file
            {
                if (line.IndexOf("<" + tag + ">") != -1)   // If we find the end of the desired tableName
                    keepAppending = false;                                      // Then stop appending for a moment   

                if (keepAppending)
                    writeMeOut += line + "\n";      // Add each line unless we are told to stop for a moment...

                if (!keepAppending && line.IndexOf("</" + tag + ">") != -1) // If we find the end of the table instance and we are not supposed to be appendgin
                    keepAppending = true;                                     // Then start appending again
            }
            StreamWriter _temp_writer = new StreamWriter(file);     // Open the file to be jumped into

            _temp_writer.Write(writeMeOut.TrimEnd() + "\n");                         // Write

            _temp_writer.Close();
        }

        /// <summary>
        /// Gets padding due to the current indentation of the file you are writing to
        /// </summary>
        /// <returns>Spaces... As many as are needed at the current moment in the file</returns>
        private String _padding()
        {
            return "".PadLeft(_indentation);
        }

        /// <summary>
        /// Get padding due to a certain amount
        /// </summary>
        /// <param name="amount">How many spaces you want</param>
        /// <returns>"amount" number of spaces</returns>
        private String _padding(int amount)
        {
            return "".PadLeft(amount);
        }

        /// <summary>
        /// Close the currently opened file
        /// </summary>
        public void Close()
        {
            _writer.Close();
        }

        /// <summary>
        /// Open a file to write XML to
        /// </summary>
        /// <param name="file">The name of the file to write XML to</param>
        public void Open(String file)
        {
            _writer = new StreamWriter(file);
        }
    }
}