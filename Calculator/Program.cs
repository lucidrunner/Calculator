namespace Calculator;

internal class Program
{
    //Slightly ugly design, but allows us to access the Operators array in switches
    //As well as iterate over them without being able to change them
    public const char PlusOperator = '+';
    public const char MinusOperator = '-';
    public const char MultiplicationOperator = '*';
    public const char DivisionOperator = '/';

    //Want to be able to iterate through the operators so need to have them in some sort of collection at least
    public static char[] SupportedOperators =>
        new[] { MultiplicationOperator, DivisionOperator, PlusOperator, MinusOperator };
    
    //Since multiplication and division, and plus and minus have the same priority we also create a multi-dimensional
    //array defining the steps of the order of operations
    public static List<char[]> OrderOfOperations =>
        new() { new []{ MultiplicationOperator, DivisionOperator }, new []{ PlusOperator, MinusOperator } };


    private static void Main(string[] args)
    {
        //Since we're not gonna do operations on our historical list we can just save it as a series of strings
        List<string> history = new List<string>();

        //Display our welcome message
        PrintSign("Welcome to the Calculator!");
        bool calculatorRunning = true;
        while (calculatorRunning)
        {
            bool correctInput = true;
            ProcessedInput parsedInput;

            //Using a do-while since we always wanna run this a minimum of one time
            do
            {
                //Ask and record the input
                Console.Write("Input your expression: ");
                string input = Console.ReadLine() ?? "";
                //Send it to our parser and get a parsed version back
                parsedInput = CalculatorInputParser.ParseInput(input);

                //If the parsed version is not in a valid state, repeat the while and print why it failed
                if (parsedInput.ValidityStatus != ProcessedInput.Validity.Valid)
                {
                    correctInput = false;
                    PrintValidityError(parsedInput);
                }
                
            } while (!correctInput);
            

            //Now that we have a correct expression split up, it's time to actually calculate the results
            double answer = ExpressionCalculator.Calculate(parsedInput);
            Console.WriteLine(answer);
            
            string fullAnswer = $"{parsedInput} = {answer}";
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

    private static void PrintValidityError(ProcessedInput correctInput)
    {
        Console.WriteLine("Input error:");
        switch (correctInput.ValidityStatus)
        {
            case ProcessedInput.Validity.ParseError:
                Console.WriteLine("Incorrect format of the entered values.");
                break;
            case ProcessedInput.Validity.OrderError:
                Console.WriteLine("Confusing operator order.");
                break;
            case ProcessedInput.Validity.DivisionByZeroError:
                Console.WriteLine("No dividing by zero.");
                break;
            case ProcessedInput.Validity.TooShortError:
                Console.WriteLine("The entered expression was too short.");
                break;
            default:
                Console.WriteLine("Unknown Input Error.");
                break;
        }
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