using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBase_Console_MGilliland
{
    /// <summary>
    /// An exception thrown when a command is not followed by the correct specifier 
    /// </summary>
    class GBaseCommandMustBeFollowedByException : Exception
    {
        public int Location { get; set; }

        public GBaseCommandMustBeFollowedByException(String commandName, String followedBy, int location) 
            : base("The command: \"" + commandName.ToUpper() + "\" must be followed by " + followedBy)
        {
            Location = location;    // An estimate of the location of the error, since characters are by pixel, this doesn't really work
        }
    }

    /// <summary>
    /// An exception thrown when a command is unrecognized
    /// </summary>
    class GBaseDoesNotRecognizeCommandException : Exception
    {
        public GBaseDoesNotRecognizeCommandException(String commandName)
            : base("\""+commandName + "\" is not recognized as a GBase command")
        {
        }
    }

    /// <summary>
    /// An exception thrown when a ROW command does not have attribute value pairs in a "with"..."in" clause
    /// </summary>
    class GBaseRowCommandMustHaveWithClauseException : Exception
    {
        public GBaseRowCommandMustHaveWithClauseException()
            : base("GBase ROW command must have a WITH clause specifying attr:value pairs")
        {
        }
    }

    /// <summary>
    /// An exception thrown when a ROW command does not have attribute value pairs in a "that" clause
    /// </summary>
    class GBaseModCommandMustHaveThatClauseException : Exception
    {
       public GBaseModCommandMustHaveThatClauseException()
            : base("GBase MOD command must have a THAT clause specifying attr:value pairs")
        {
        }
    }

    /// <summary>
    /// An excpetion thrown when attribute value pairs are incorrectly given
    /// </summary>
    class GBaseAttrValuePairsMustBeOfFormException : Exception
    {
        public GBaseAttrValuePairsMustBeOfFormException()
            : base("GBase attr value pairs must be specified as [attr]:[value]")
        {
        }
    }

    /// <summary>
    /// A general GBase specific exception with just a message
    /// </summary>
    class GBaseGeneralException : Exception
    {
        public GBaseGeneralException(String message)
            : base(message)
        {
        }
    }
}
