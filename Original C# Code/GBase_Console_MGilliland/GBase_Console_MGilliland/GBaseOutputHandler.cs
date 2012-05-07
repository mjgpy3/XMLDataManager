using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// Handles the output produced by the GBase
    /// </summary>
    class GBaseOutputHandler
    {
        private String _feedbackBuffer;

        /// <summary>
        /// Ends the output buffer with a the specified text followed by a nice line!
        /// </summary>
        /// <param name="textToEndWith">Text to go before the ending line</param>
        public void EndBufferWithText(String textToEndWith = "")
        {
            _feedbackBuffer += textToEndWith + "\n-----------------------------------------------------\n";
        }

        /// <summary>
        /// Adds to the output buffer
        /// </summary>
        /// <param name="outputToAdd">What to add</param>
        public void AddOutput(String outputToAdd)
        {
            _feedbackBuffer += outputToAdd + "\n";
        }

        /// <summary>
        /// Gets the output that has been accumulating, restarting the accumulation
        /// </summary>
        /// <returns></returns>
        public String GetOutputBuffer()
        {
            String temp = _feedbackBuffer;      // Gets the feedback
            _feedbackBuffer = "";               // Restarts the feedback
            return temp;                        // Returns the feedback
        }
    }
}
