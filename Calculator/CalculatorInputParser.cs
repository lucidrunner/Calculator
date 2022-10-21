using System.Text;

namespace Calculator;

/// <summary>
/// Handles converting provided input 
/// </summary>
public static class CalculatorInputParser
{
    
    public static ProcessedInput ParseInput(string input)
    {
        ProcessedInput processedInput = new ProcessedInput();

        //Trim any operator signs that are at the start or end of the input string, since these will be disregarded anyway
        input = input.Trim(Program.SupportedOperators);



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
                processedInput.AddComponent(new ExpressionComponent(value, ExpressionType.Value));
                //Remove the value part
                input = input.Remove(0, nextIndex);
            }

            //If we still have input left we need to get our operator too 
            if (input.Length > 0)
            {
                processedInput.AddComponent(new ExpressionComponent(input[..1], ExpressionType.Operator));
                input = input.Remove(0, 1);
            }
        }


        return processedInput;
    }

    private static int FindNextOperator(string input)
    {
        int nextIndex = - 1;
        foreach (var supportedOperator in Program.SupportedOperators)
        {
            int index = input.IndexOf(supportedOperator);
            if (index >= 0 && (index < nextIndex || nextIndex < 0))
                nextIndex = index;
        }
        return nextIndex;
    }
}


//Record for our input, split into parts and the operator
//Slightly breaking my Part 1 rule here by using a record, but prefer them over tuples
public class ProcessedInput
{
    public enum Validity
    {
        Valid, ParseError, OrderError, DivisionByZeroError, TooShortError
    }
    private readonly List<ExpressionComponent> _processedComponents = new List<ExpressionComponent>();
    private Validity _validity = Validity.TooShortError; //By default we're too short to be valid
    public Validity ValidityStatus => _validity;

    public void AddComponent(ExpressionComponent component)
    {
        _processedComponents.Add(component);
        Validate();
    }


    /// <summary>
    /// Returns a copy of the component
    /// </summary>
    public List<ExpressionComponent> ProcessedComponents => new(_processedComponents);

    public void Validate()
    {

        //3 main cases of invalid input
        //First is if any of our value tagged components aren't parseable
        //Could do this as a lambda but keeping it simpler
        foreach (ExpressionComponent component in _processedComponents)
        {
            if (component.Type == ExpressionType.Value && !double.TryParse(component.Value, out _))
            {
                _validity = Validity.ParseError;
                return;
            }
        }

        //Second is if we're not alternating between values and operators
        for (int index = 0; index < _processedComponents.Count - 1; index++)
        {
            //If the next input is different, skip forward
            if (_processedComponents[index].Type != _processedComponents[index + 1].Type) continue;
            
            //Otherwise, order error
            _validity = Validity.OrderError;
            return;
        }

        //Third is a bit of a special case - checking for division by 0
        for (int index = 0; index < _processedComponents.Count - 1; index++)
        {
            //If we're not dealing with a division, skip current
            if (!_processedComponents[index].Value.Equals(Program.DivisionOperator.ToString()))
                continue;

            //Otherwise, if the preceding value is 0, we're invalid
            double.TryParse(_processedComponents[index + 1].Value, out double value);
            if (value == 0)
            {
                _validity = Validity.DivisionByZeroError;
                return;
            }
        }

        //Fourth is pretty simple, we're just too short to be valid
        if (_processedComponents.Count < 3)
        {
            _validity = Validity.TooShortError;
            return;
        }

        //Otherwise, we're valid
        _validity = Validity.Valid;
    }

    public override string ToString()
    {
        StringBuilder returnString = new StringBuilder();
        foreach (var component in _processedComponents)
        {
            returnString.Append(component.Value);
            returnString.Append(" ");
        }

        return returnString.ToString();
    }
}

public record ExpressionComponent(string Value, ExpressionType Type) { }

public enum ExpressionType
{
    Operator, Value
}