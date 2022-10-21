using System.Linq;

namespace Calculator;


/// <summary>
/// Calculates passed in expressions
/// </summary>
public static class ExpressionCalculator
{
    public static double Calculate(ProcessedInput input)
    {
        //Some error checking, return if we've not sent in a valid input
        if (input.ValidityStatus != ProcessedInput.Validity.Valid)
            return double.NaN;

        //Create a local copy of the input
        var workedInput = input.ProcessedComponents;
        /*Go through the input based on the order of operations
         ------------------------------------
        Originally I did this by using the SupportedOperators array
        Now I use a list of arrays, since it can be ambiguous if * or /, + or - should be done first.
        Doing it with the SupportedOperators array would have meant that we'd be forced to do division first, since otherwise multiplication might have left
        us with a risk to divide by 0
        Going left to right and checking multiple operators at once evades that while keeping the ambiguity
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

                //Same if we're on the first or last value, although this should not happen with current parser
                if (inputIndex == 0 || inputIndex == workedInput.Count - 1)
                    continue;

                //Otherwise, create a new expression based on the value before and after our parse
                //Since one of our validity-checks is that all values are parseable, we don't need to worry about parse errors here
                double firstValue = double.Parse(workedInput[inputIndex - 1].Value);
                double secondValue = double.Parse(workedInput[inputIndex + 1].Value);
                double calculatedValue = Calculate(firstValue, secondValue, workedInput[inputIndex].Value);
                ExpressionComponent calculatedComponent = new(calculatedValue.ToString(), ExpressionType.Value);

                //Now that we have a new component, it's time to swap the 3 components that 
                workedInput.Insert(inputIndex - 1, calculatedComponent);
                workedInput.RemoveRange(inputIndex, 3);

                //This is a lot slower, but just to be safe I reset the inputIndex to 0. 
                inputIndex = 0;
            }
        }

        return double.Parse(workedInput[0].Value);
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
        //Uncomment to show the steps of the calculation
        //Console.WriteLine($"Calculating {firstValue} {operatorSign} {secondValue}");

        //Not a fan of these if-checks but prefer Contains over Equals
        if (operatorSign.Contains(Program.MultiplicationOperator))
            return firstValue * secondValue;
        if (operatorSign.Contains(Program.DivisionOperator))
            return firstValue / secondValue;
        if (operatorSign.Contains(Program.PlusOperator))
            return firstValue + secondValue;
        if (operatorSign.Contains(Program.MinusOperator))
            return firstValue - secondValue;

        return double.NaN;
    }

}