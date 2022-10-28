namespace Calculator;

//TODO Same note as for the InputParser, mostly just using a static class here to group some methods together
//One note of improvement here - the class makes some assumptions about the values it gets and just parses them directly. A more futureproof way would be TryParse and return NaN if
//they fail
//TODO - Second note of improvement, I originally used doubles for my calculations. At the end I ran into rounding errors due to this. I'm not too familiar with decimals but might
//have been worth experimenting with at least.


/// <summary>
/// Provides methods for calculating different levels of mathematical expressions.
/// </summary>
public static class ExpressionCalculator
{
    //TODO Set to True to show the steps taken by the calculator
    private const bool ShowSteps = false;

    /// <summary>
    /// Goes through and calculates the value of the passed in Expression based on the order of operations defined by Program.OrderOfOperations
    /// </summary>
    /// <returns>The calculated value of all the expression components, or double.NaN if the expression is invalid.</returns>
    public static double Calculate(MathematicalExpression input)
    {
        //If we know we have an invalid expression, return immediately
        if (input.ValidityStatus != MathematicalExpression.Validity.Valid)
            return double.NaN;

        //Create a local copy of the input
        var workedInput = input.ProcessedComponents;

        /*Go through the input based on the order of operations and solve it in parts
         ------------------------------------
        Originally I did this by using the SupportedOperators array
        Now I use a list of arrays, since it can be ambiguous if * or /, + or - should be done first.
        Doing it with the SupportedOperators array would have meant that we'd be forced to do division first, since otherwise multiplication might have left
        us with a risk to divide by 0 (example: 2 / 5 * 0 would result in 2 / 0 if multiplication is done first)
        Going left to right and checking multiple operators at once evades that while keeping the ambiguity present in the order of operations
         ------------------------------------
        */
        foreach (char[] operators in Program.OrderOfOperations)
        {
            for (int inputIndex = 0; inputIndex < workedInput.Count; inputIndex++)
            {
                //If we're not looking at an operator, or it's not one of our current operators, skip it
                if (!(workedInput[inputIndex].Type == ExpressionType.Operator &&
                      operators.Any(n => workedInput[inputIndex].Value.Contains(n))))
                    continue;

                //Same if we're on the first or last value, although this should not happen with the current parser
                if (inputIndex == 0 || inputIndex == workedInput.Count - 1)
                    continue;

                //Otherwise, create a new expression based on the value before and after our current component
                //Since one of our validity-checks is that all values are parseable, we don't need to worry about parse errors here
                double firstValue = double.Parse(workedInput[inputIndex - 1].Value);
                double secondValue = double.Parse(workedInput[inputIndex + 1].Value);
                double calculatedValue = Calculate(firstValue, secondValue, workedInput[inputIndex].Value);
                ExpressionComponent calculatedComponent = new(calculatedValue.ToString(), ExpressionType.Value);

                if(ShowSteps)
                    Console.Write($"({calculatedValue})\n");

                //Now that we have a new component, it's time to swap the 3 components that created it (first value, second value and 
                workedInput.Insert(inputIndex - 1, calculatedComponent);
                workedInput.RemoveRange(inputIndex, 3);

                //This is slower but just to be safe I reset the inputIndex to 0 rather than just doing -- on it
                inputIndex = 0;
            }
        }

        //Parse the sole remaining value to our result
        //Catch all error handling at the end, if we make some changes to MathematicalExpression down the line this returns NaN rather than crashes
        if (workedInput.Count > 0 && double.TryParse(workedInput[0].Value, out double result))
            return result;

        return double.NaN;
    }

    /// <summary>
    /// Calculates two values based on the passed in operator.
    /// </summary>
    /// <param name="firstValue">The first value to be calculated</param>
    /// <param name="secondValue">The second value to be calculated</param>
    /// <param name="operatorSign">The operator sign, will be checked against the defined Program operators</param>
    /// <returns>The calculated value if the operator is valid, otherwise NaN</returns>
    public static double Calculate(double firstValue, double secondValue, string operatorSign)
    {
        if(ShowSteps)
            Console.Write($"Calculating {firstValue} {operatorSign} {secondValue} ");
        

        //Not a fan of these if-checks but prefer Contains over Equals
        //Since doubles aren't perfect we also need to round a bit
        if (operatorSign.Contains(Program.MultiplicationOperator))
            return Math.Round(firstValue * secondValue, 7);
        if (operatorSign.Contains(Program.DivisionOperator))
            return Math.Round(firstValue / secondValue, 7);
        if (operatorSign.Contains(Program.PlusOperator))
            return Math.Round(firstValue + secondValue, 7);
        if (operatorSign.Contains(Program.MinusOperator))
            return Math.Round(firstValue - secondValue, 7);


        return double.NaN;
    }

}