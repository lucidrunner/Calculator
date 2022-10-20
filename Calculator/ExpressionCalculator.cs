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
        //Go through the input based on the order of operations
        //Originally I did this by using the SupportedOperators array
        //Now I use a [,] array and 


        for (int orderIndex = 0; orderIndex < Program.OrderOfOperations.Count; orderIndex++)
        {
            var operators = Program.OrderOfOperations[orderIndex];
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
                ExpressionComponent calculatedComponent =
                    new ExpressionComponent(calculatedValue.ToString(), ExpressionType.Value);


                //Now that we have a new component, it's time to swap the 3 components that 
                workedInput.Insert(inputIndex - 1, calculatedComponent);
                workedInput.RemoveRange(inputIndex, 3);

                //This is a lot slower, but just to be safe I reset the inputIndex to 0. 
                inputIndex = 0;
            }
        }

        Console.WriteLine($"After calculating, {workedInput.Count} remains.");

        return double.Parse(workedInput[0].Value);

    }

    private static double Calculate(double firstValue, double secondValue, string operatorSign)
    {
        Console.WriteLine($"Calculating {firstValue} {operatorSign} {secondValue}");
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