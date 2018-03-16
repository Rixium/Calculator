using System;
using System.Collections.Generic;
using System.Windows.Forms;

/* Calculator can compute based on BODMAS with precedence.
 * Includes Brackets, Indices, Multiplication and Divison, Addition and Subtraction.
 */

namespace Calculator {
    public partial class Form1 : Form
    {

        // Stores all the inputted digits and operators.
        private List<string> commandList = new List<string>();

        // Stores the current displayed value for the display.
        private string currentLabelString = "";

        // An array of precedences ordered in an array.
        private string[] precendenceList = new string[] {"+-", "*/", "^", "("};

        // Holds the starting milliseconds before the calculation takes place.
        private double startTime;

        public Form1() {
            InitializeComponent();
        }

        // Recursive statement that allows calculation of segments of the list.
        private float evaluate(List<string> commandList)
        {
            // Set our current precedence to the final value in the list.
            int currentPrecedence = precendenceList.Length - 1;

            // If the commandList contains more than 1 value, then calculations must take place.
            while (commandList.Count > 1 && currentPrecedence >= 0)
            {
                // Sets the starting point of our brackets.
                int startIndex = 0;
                // Iterating through the entire command list.
                for (int i = 0; i < commandList.Count; i++)
                {
                    // A flag, for when we need to start from the beginning of the command list again.
                    bool startOver = false;

                    // If we're currently calculating brackets.
                    if (precendenceList[currentPrecedence] == "(")
                    {
                        // If we find an opening bracket.
                        if (commandList[i][0] == '(')
                        {
                            // Set the startIndex as the index of the opening bracket.
                            startIndex = i;
                            // If we find a closing bracket.
                        } else if (commandList[i][0] == ')')
                        {
                            // We create a new sublist.
                            List<string> subList = new List<string>();
                            // Iterate from the startIndex bracket + 1, and go until our current iteration (closing bracket).
                            for (int start = startIndex + 1; start < i; start++)
                            {
                                // Add the command to the new command list.
                                subList.Add(commandList[start]);
                            }

                            // Recursively call our evaluate function with the new sublist.
                            float val = evaluate(subList);
                            // Remove the range of values from the starting bracket, to the closing.
                            commandList.RemoveRange(startIndex, i - startIndex + 1);
                            // Insert our new value at the startIndex.
                            commandList.Insert(startIndex, val.ToString());
                            // Set our flag to start the loop again.
                            startOver = true;
                        }
                    } else { // If we're no longer checking for brackets.

                        // Iterate through the current precedence string, since some precedences are paired.
                        foreach (var c in precendenceList[currentPrecedence])
                        {
                            // If we don't find the precedence operator at the current command position, then continue.
                            if (commandList[i][0] != c) continue;

                            /* Otherwise, calculate the value based on the operator
                             * Here, we pass the digit before the operator, and the digit after, to our evaluationSection function. */
                            var newVal = EvaluateSection(float.Parse(commandList[i - 1]),
                                float.Parse(commandList[i + 1]), c.ToString());

                            // We remove the operator and the 2 numbers.
                            commandList.RemoveRange(i - 1, 3);
                            // Insert the new value at the index given.
                            commandList.Insert(i - 1, newVal.ToString());
                            // We need to decrement i, since we have operated and removed values from the command list.
                            i--;
                            // We've found and operated on the found precedence, so we don't need to search anymore.
                            break;
                        }
                    }
                    // If startOver is true, then we reset the loop, incase of missed and altered values.
                    if (startOver) i = 0;
                }

                // Finally, after we've checked the currentPrecedence, we decrement it, and restart the loops.
                currentPrecedence--;
            }

            // Once all the commands have been calculated, and we're left with a single value in the command list, pass it back.
            return float.Parse(commandList[0]);
        }

        private static float EvaluateSection(float n1, float n2, string op)
        {
            // Compute based on the passed operator.
            switch (op)
            {
                case "+":
                    return n1 + n2;
                case "-":
                    return n1 - n2;
                case "*":
                    return n1 * n2;
                case "/":
                    return n1 / n2;
                case "^":
                    return (float)Math.Pow(n1, n2);
            }

            return 0;
        }

        private void ButtonManager(object sender, EventArgs e)
        {
            var b = (Button) sender;
            var c = b.Text;

            // If equals has been pressed, we need to evaluate the command list the we have.
            if (c == "=")
            {
                startTime = DateTime.Now.Millisecond; // This is to measure how long the calculation takes.
                float result = evaluate(commandList); // We get the final recursive result from our evaluation function.
                label1.Text = result.ToString(); // We set the display to our calculated value.
                // Final measurement in seconds of the time taken.
                label2.Text = ($@"Calculated in: {((DateTime.Now.Millisecond - startTime) / 1000)} s");
                return; // We don't need to go any further.
            }

            // Add the inputted value to our commandList.
            commandList.Add(c);
            // Add the current input to our displayed string.
            currentLabelString += c + "";
            // Set the displayed string.
            label1.Text = currentLabelString;
        }

        public void Button16_Click(object sender, EventArgs e)
        {
            // This is the reset button, we just need to reset the displayed string, and clear the commandList.
            currentLabelString = "";
            label2.Text = "";
            label1.Text = currentLabelString;
            commandList.Clear();
        }

    }
}
