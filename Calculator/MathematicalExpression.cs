﻿using System.Text;

namespace Calculator;

/// <summary>
/// 
/// </summary>
public class MathematicalExpression
{
    public enum Validity
    {
        Valid, ParseError, OrderError, DivisionByZeroError, TooShortError
    }
    private readonly List<ExpressionComponent> _processedComponents = new List<ExpressionComponent>();
    private Validity _validity = Validity.TooShortError; //By default we're too short to be valid
    private bool nextNegative = false; //Used to handle negative numbers (example: 5*-2) 

    public Validity ValidityStatus => _validity;

    /// <summary>
    /// Adds a provided component to the expression and validates the current state.
    /// </summary>
    /// <param name="component">The component that should be added to the expression. If the previous component was an operator and the current component is a minus it will instead negate the next component if possible.</param>
    public void Append(ExpressionComponent component)
    {
        //Handle negative numbers
        // Two cases to consider: component  '-' and we have an empty component list or the previous component is an operator
        if (component.Type == ExpressionType.Operator && 
            (_processedComponents.Count == 0 || _processedComponents.Last().Type == ExpressionType.Operator))
        {
            //If we have a minus, negate the next value instead of saving the component
            if (component.Value.Contains(Program.MinusOperator))
            {
                nextNegative = true;
                return;
            }
            
            //Small design note here:
            //I could remove other extra operators via either a general return in here, or more specific extra if-checks (eg 5*/2 could be set to ignore the / resulting in 5*2)
            //I chose not to do that because I don't want this class to modify the passed in input, doing so would remove the need for the Validate()-design
            //Instead it should fully represent what's passed in and provide information on why a faulty expression is faulty
            //This is also why I re-add the '-' below if the next component is not a value since then we properly represent the invalid expression (example: 5/-*2 should not produce 5/*-2 when printed)
        }

        //Negate a value if we should and can
        if (nextNegative)
        {
            nextNegative = false;
            //If we're supposed to negate but the next component is not a value, re-add our minus component
            if(component.Type == ExpressionType.Operator)
            {
                _processedComponents.Add(new ExpressionComponent(Program.MinusOperator.ToString(), ExpressionType.Operator));
            }
            else if (double.TryParse(component.Value, out double value))
            {
                value *= -1;
                component = new ExpressionComponent(value.ToString(), ExpressionType.Value);
            }
        }


        _processedComponents.Add(component);
        Validate();
    }


    /// <summary>
    /// Returns a copy of the component-list
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

        //Second is if we're not alternating between values and operators OR if our first component is an operator
        for (int index = 0; index < _processedComponents.Count - 1; index++)
        {
            if (index == 0 && _processedComponents[index].Type == ExpressionType.Operator)
                _validity = Validity.OrderError;

            //If the next input is different, skip forward
            else if (_processedComponents[index].Type != _processedComponents[index + 1].Type) continue;
            
            //Otherwise, order error
            _validity = Validity.OrderError;
            return;
        }

        //Third is a bit of a special case - checking for division by 0
        for (int index = 0; index < _processedComponents.Count - 1; index++)
        {
            //If we're not dealing with a division, skip current
            if (!_processedComponents[index].Value.Contains(Program.DivisionOperator))
                continue;

            //Otherwise, if the preceding value is 0, we're invalid
            double.TryParse(_processedComponents[index + 1].Value, out double value);
            if (value.Equals(0))
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
            returnString.Append(' ');
        }

        return returnString.ToString();
    }
}

public record ExpressionComponent(string Value, ExpressionType Type) { }

public enum ExpressionType
{
    Operator, Value
}