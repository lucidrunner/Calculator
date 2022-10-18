//Main reason not to use top level statements: Can't file-scope namespaces with them :(

using System.Diagnostics;

namespace Calculator;

internal class Program
{

    //Slightly ugly design, but allows us to access the Operators array in switches
    private const char PlusOperator = '+';
    private const char MinusOperator = '-';
    private const char MultiplicationOperator = '*';
    private const char DivisionOperator = '/';

    //As well as iterate over them without being able to change them
    private static char[] SupportedOperators =>
        new[] { PlusOperator, MinusOperator, MultiplicationOperator, DivisionOperator };

    static void Main(string[] args)
    {
        //Since we're not gonna do operations on our historical list we can just save it as a series of strings
        List<string> history = new List<string>();

        //Display our welcome message
        PrintSign("Welcome to the Calculator!");
        bool calculatorRunning = true;
        while (calculatorRunning)
        {
            bool correctInput = true;
            ProcessedInput input;
            //Using a do-while since we always wanna run this a minimum of one time
            do
            {
                //Input numbers - Basic version = we only deal with 2. I've decided to ask for it as a single string since it's easier to expand later
                Console.Write("Input your two numbered operation (eg x*y): ");
                correctInput = ReadInput(out input);
                
                //If we did not read the input in a correct format, go to the next cycle of the while
                if(!correctInput)
                {
                    Console.WriteLine("Please provide the input in correct format.");
                    continue;
                }

                //There is one final validity check we have to do even if we have correct input - division by zero
                //We could have done this in the reading of the input, but let's keep our responsibilities separate
                if (input.OperatorSymbol == DivisionOperator && input.SecondInput.Equals(0))
                {
                    correctInput = false;
                    Console.WriteLine("We can't divide by zero.");
                }

            } while (!correctInput);

            //Now that we have a correct expression split up, it's time to actually calculate the results
            double answer = Calculate(input);

            //And print it
            string fullAnswer = $"{input} = {answer}";
            Console.WriteLine(fullAnswer);

            //Finally, we save it to our history list
            history.Add(fullAnswer);

            //After that, prompt to display the history list and if yes, do so
            if (PromptForChoice("Display the Calculator history?"))
            {
                foreach (string calculation in history)
                {
                    Console.WriteLine(calculation);
                }
            }


            calculatorRunning = PromptForChoice("Restart the Calculator?");
        }
        PrintSign("You are now leaving the Calculator. Goodbye!");
    }
    

    private static double Calculate(ProcessedInput input)
    {
        //When we're dealing with two numbers this part is actually fairly straight forward
        return input.OperatorSymbol switch
        {
            PlusOperator => input.FirstInput + input.SecondInput,
            MinusOperator => input.SecondInput - input.SecondInput,
            MultiplicationOperator => input.FirstInput * input.SecondInput,
            DivisionOperator => input.FirstInput / input.SecondInput,
            _ => double.NaN
        };
    }

    private static bool ReadInput(out ProcessedInput processedInput)
    {
        //One negative part about the pattern I did in this method
        //We have to assign a default record to the out argument since we might need to leave early
        //A risk of doing this is that we later miss assigning parts to the record
        processedInput = new ProcessedInput(0, '*', 0);
        
        //Read the input
        string input = Console.ReadLine() ?? "";
        
        //Start by splitting it
        var inputParts = input.Split(SupportedOperators, StringSplitOptions.TrimEntries);
        
        //First fail state - If any parts are empty, or we don't have two parts in our expression we return false
        if (inputParts.Length != 2 && inputParts.Any(string.IsNullOrEmpty))
            return false;

        
        //Two hickups from splitting occur - the first is that we eliminate the operator from the parts
        //We use a method to get around this
        char? mathOperator = GetOperator(input);
        //If the operator was incorrect, return false
        if (!mathOperator.HasValue)
            return false;

        //Second hickup is parsing from the parts to our doubles, which can fail for a number of reasons
        //Solving this by using a double.TryParse as a kind of catch-all

        //Have to initialize these before our passedParse check to avoid confusing the compilator 
        double firstNumber = 0, secondNumber = 0;
        bool passedParse = double.TryParse(inputParts[0], out firstNumber) &&
                           double.TryParse(inputParts[1], out secondNumber);
        
        if (!passedParse)
            return false;

        //Before exiting, remember to create our correct record
        processedInput = new ProcessedInput(firstNumber, mathOperator.Value, secondNumber);

        //Finally, return true if we passed all our input checking
        return true;
    }


    private static char? GetOperator(string input)
    {
        //Fairly straight forward since we're only dealing with one operator - go through our allowed operators and see which fits
        foreach (char supportedOperator in SupportedOperators)
        {
            if (input.Contains(supportedOperator))
                return supportedOperator;
        }
        return null;
    }
    
    /// <summary>
    /// Prompts the Player for yes / no on a selected text
    /// </summary>
    /// <param name="promptText">What will be printed, followed by [y/n]:</param>
    /// <returns>True if the player answers y</returns>
    private static bool PromptForChoice(string promptText)
    {
        //Nothing too fancy, we ask for the response and returns true if it's yes
        string playerResponse = "";
        //ToLower so we don't have to deal with Yy and Nn differences
        while (playerResponse.ToLower() != "y" && playerResponse.ToLower() != "n")
        {
            Console.Write($"{promptText} [y/n]: ");
            //?? so we don't have to worry about null
            playerResponse = Console.ReadLine() ?? "";
        }

        return playerResponse.ToLower() == "y";
    }

    /// <summary>
    /// Prints the supplied text surrounded by stars to the console. Will not print if the string is null or empty.
    /// </summary>
    /// <param name="signText">The text that will be displayed</param>
    /// <param name="signCharacter">The character (default *) that will be printed on the sign</param>
    private static void PrintSign(string? signText, char signCharacter = '*')
    {
        if (string.IsNullOrEmpty(signText))
            return;

        //Some magic numbers but 4 due to the edges + space on the middle of the sign
        string signEdge = new string(signCharacter, signText.Length + 4);
        Console.WriteLine();
        Console.WriteLine(signEdge);
        Console.WriteLine($"{signCharacter} {signText} {signCharacter}");
        Console.WriteLine(signEdge);
        Console.WriteLine();
    }
}

//Record for our input, split into parts and the operator
//Slightly breaking my Part 1 rule here by using a record, but prefer them over tuples
internal record ProcessedInput(double FirstInput, char OperatorSymbol, double SecondInput)
{
    public override string ToString()
    {
        return $"{FirstInput}{OperatorSymbol}{SecondInput}";
    }
}