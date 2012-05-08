using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace GBase_Console_MGilliland
{
    public partial class GBaseConsole : Form
    {
        String _currentInput;
        GBaseHandler _gbHandler;
        GBaseCommandString _command;

        public GBaseConsole()
        {
            InitializeComponent();

            _gbHandler = new GBaseHandler();
            _command = new GBaseCommandString();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            _currentInput = txtCommand.Text;    // Get the current string
            rtbOutput.Text = "";                // Set the Output box blank

            // Tries to format the passed string into a command if an exception is thrown then it is caught
            try
            {
                _command.StringToCommandString(_currentInput);
            }
            catch (GBaseCommandMustBeFollowedByException mustFollow)    // Caught if X is not followed by Y
            {
                rtbOutput.Text += mustFollow.Message + "\n";
                _command.ErrorOccured = true;
            }
            catch (GBaseDoesNotRecognizeCommandException unrecognizedCommand)   // Caught if the command is unrecognized
            {
                rtbOutput.Text += unrecognizedCommand.Message + "\n";
                _command.ErrorOccured = true;
            }
            catch (GBaseRowCommandMustHaveWithClauseException mustHaveWith)     // Caught if a row command does not have a with
            {
                rtbOutput.Text += mustHaveWith.Message + "\n";
                _command.ErrorOccured = true;
            }
            catch (GBaseModCommandMustHaveThatClauseException mustHaveThat)     // Caught if a row command does not have a that
            {
                rtbOutput.Text += mustHaveThat.Message + "\n";
                _command.ErrorOccured = true;
            }
            catch (GBaseAttrValuePairsMustBeOfFormException attrValInWrongForm) // Caught if the attributes were set up wrong
            {
                rtbOutput.Text += attrValInWrongForm.Message + "\n";
                _command.ErrorOccured = true;
            }
            catch (Exception)                                                   // Caught if any other exception is thrown
            {
                _command.ErrorOccured = true;
                rtbOutput.Text += "An unknown error was found in your command\n";
            }

            txtCommand.Text = "";


            if (!_command.ErrorOccured)         // If no error occured
            {
                // Try to evaulate the command
                try
                {
                    switch (_command.Command)
                    {
                        case "gen":
                            switch (_command.ActsUpon)
                            {
                                case "gbase":
                                    _gbHandler.GenGBase(_command.GBaseName);
                                    break;
                                case "table":
                                    _gbHandler.GenTable(_command.TableName);
                                    break;
                                case "col":
                                    _gbHandler.GenColIn(_command.ColName, "string", _command.TableName);
                                    break;
                                case "row":
                                    _gbHandler.GenRowIn(_command.WithAttributes, _command.WithValues, _command.TableName);
                                    break;
                            }
                            break;
                        case "use":
                            _gbHandler.Use(_command.GBaseName);
                            break;
                        case "get":
                            SetRTBToOutput(_gbHandler.GetRowWith(_command.WithAttributes, _command.WithValues, _command.TableName));
                            break;
                        case "del":
                            switch (_command.ActsUpon)
                            {
                                case "gbase":
                                    _gbHandler.DelGBase(_command.GBaseName);
                                    break;
                                case "table":
                                    _gbHandler.DelTable(_command.TableName);
                                    break;
                                case "col":
                                    _gbHandler.DelColIn(_command.ColName, _command.TableName);
                                    break;
                                case "row":
                                    _gbHandler.DelRowWith(_command.WithAttributes, _command.WithValues, _command.TableName);
                                    break;
                            }
                            break;
                        case "mod":
                            _gbHandler.ModRowWith(_command.WithAttributes, _command.WithValues, _command.TableName, _command.ThatAttributes, _command.ThatValues);
                            break;
                        case "help":
                            // Not yet implemented
                            break;
                    }
                }   
                catch (Exception exception)         // If the command throws an exception then print the error message
                {
                    rtbOutput.Text += "Unaccounted for exception Occured:\n" + exception.Message + "\n";
                    rtbOutput.Text += "Console output:\n";
                }
                finally
                {
                    rtbOutput.Text += _gbHandler.GetOutput();
                }
            }
        } 

        /// <summary>
        /// Loops through the output of a Get command an prints its output
        /// </summary>
        /// <param name="output"></param>
        private void SetRTBToOutput(ArrayList output)
        {
            rtbOutput.Text += "\n";
            foreach (String[] tuple in output)
            {
                foreach (String item in tuple)
                    rtbOutput.Text += item + "  ";

                rtbOutput.Text += "\n";
            }

            rtbOutput.Text += "\n\n";
        }

        /// <summary>
        /// Makes a GBase and seeds it with Data
        /// </summary>
        private void Seed()
        {
            // Generate model
            _gbHandler.GenGBase("TestGBase");

            // Generate tables
            _gbHandler.GenTable("Drinks");
            _gbHandler.GenTable("Foods");

            // Generate columns
            _gbHandler.GenColIn("DrinkName", "string", "Drinks");
            _gbHandler.GenColIn("DrinkType", "string", "Drinks");
            _gbHandler.GenColIn("Flavors", "int", "Drinks");
            _gbHandler.GenColIn("FoodName", "string", "Foods");
            _gbHandler.GenColIn("CanBeBoughtAt", "string", "Foods");
            _gbHandler.GenColIn("TastesLike", "string", "Foods");

            // Generate rows
            _gbHandler.GenRowIn(new String[] { "DrinkName", "DrinkType", "Flavors" }, new String[] { "Monster Coffee", "Coffee", "3" }, "Drinks");
            _gbHandler.GenRowIn(new String[] { "DrinkName", "DrinkType", "Flavors" }, new String[] { "Monster", "Energy Drink", "5" }, "Drinks");
            _gbHandler.GenRowIn(new String[] { "DrinkName", "DrinkType", "Flavors" }, new String[] { "Mountain Dew", "Soda", "6" }, "Drinks");
            _gbHandler.GenRowIn(new String[] { "DrinkName", "DrinkType", "Flavors" }, new String[] { "Milk", "Natural", "2" }, "Drinks");
            _gbHandler.GenRowIn(new String[] { "FoodName", "CanBeBoughtAt", "TastesLike" }, new String[] { "Pizza", "Papa Johns", "Pepers" }, "Foods");
            _gbHandler.GenRowIn(new String[] { "FoodName", "CanBeBoughtAt", "TastesLike" }, new String[] { "Taco", "Taco Bell", "Chicken" }, "Foods");
            _gbHandler.GenRowIn(new String[] { "FoodName", "CanBeBoughtAt", "TastesLike" }, new String[] { "Ice Cream", "DQ", "Sugar" }, "Foods");
            _gbHandler.GenRowIn(new String[] { "FoodName", "CanBeBoughtAt", "TastesLike" }, new String[] { "Steak", "Ponderosa", "Chicken" }, "Foods");
            _gbHandler.GenRowIn(new String[] { "FoodName", "CanBeBoughtAt", "TastesLike" }, new String[] { "Cookies", "Subway", "Chicken" }, "Foods");

        }
    }
}
