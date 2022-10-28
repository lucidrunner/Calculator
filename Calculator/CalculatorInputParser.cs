namespace Calculator;

/// <summary>
/// Supports operations to convert a mathematical string expression (ie 2*5+2) to a 
/// </summary>
public static class CalculatorInputParser
{
    /// <summary>
    /// Parses a provided string into operators and values and builds an expression based on them
    /// </summary>
    /// <param name="input">The string that should represent a mathematical expression (eg. 5*2+1).</param>
    /// <returns>The MathematicalExpression built from the parts, can be marked as invalid for several reasons.</returns>
    public static MathematicalExpression ParseInput(string input)
    {
        MathematicalExpression inputExpression = new MathematicalExpression();

        //Trim any operator signs that are at the end of our input string since these will be discarded anyway
        input = input.TrimEnd(Program.SupportedOperators);

        //Go through the string, taking chunks off it until we don't have any operators left
        while (input.Length > 0)
        {
            //Trim white spaces
            input = input.Trim();

            //Find the next operator
            int nextIndex = FindNextOperator(input);
            //If we have no operators left, we're gonna add the rest of the input as a value
            if (nextIndex < 0)
                nextIndex = input.Length;

            //Add the value part (aka, the input up to the next operator) to the processed input
            string value = input.Substring(0, nextIndex);
            if (value.Length > 0)
            {
                inputExpression.Append(new ExpressionComponent(value, ExpressionType.Value));
                input = input.Remove(0, nextIndex);
            }

            //If we still have input left, that means we added value before an operator, and need to also add the operator as a separate component
            if (input.Length > 0)
            {
                inputExpression.Append(new ExpressionComponent(input[..1], ExpressionType.Operator));
                input = input.Remove(0, 1);
            }
        }


        return inputExpression;
    }

    /// <summary>
    /// Reports the index of the next operator in the provided expression.
    /// </summary>
    /// <returns>The index of the next operator, based on the Program.SupportedOperators. -1 if no operator is present.</returns>
    private static int FindNextOperator(string expression)
    {
        int nextIndex = - 1;
        foreach (var supportedOperator in Program.SupportedOperators)
        {
            int index = expression.IndexOf(supportedOperator);
            if (index >= 0 && (index < nextIndex || nextIndex < 0))
                nextIndex = index;
        }
        return nextIndex;
    }
}
