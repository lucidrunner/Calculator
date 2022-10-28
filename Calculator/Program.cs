namespace Calculator;

/*TODO Wrote this in the README too but using these TODO-blocks in the start of each file to add some extra thoughts regarding the current design
 One thing stand out in this - as an entry point Program / Main is doing too much and should really instead instantiate a calculator class that runs the show
The reason I'm not doing that (and also the reason the console "GUI" looks the way it does) is because I kinda made it a goal not to deviate too far from the provided pseudo-code
Some improvements to the UI would be a menu (Do Math, Help, View History, Exit etc) instead of the current math->history->reset/exit flow. 
The problem with the flow is especially apparent during input because I also wanted to add invalidly entered expressions into the history-list, 
and if I wanted to have the possibility to show the list after that I'd have to do some refactoring
*/
internal class Program
{
    //Slightly ugly design, but allows us to access the Operators array in switches
    //As well as iterate over them without being able to accidentally change them
    public const char PlusOperator = '+';
    public const char MinusOperator = '-';
    public const char MultiplicationOperator = '*';
    public const char DivisionOperator = '/';

    //Want to be able to iterate through the operators so need to have them in some sort of collection at least
    public static char[] SupportedOperators =>
        new[] { MultiplicationOperator, DivisionOperator, PlusOperator, MinusOperator };
    
    //Since multiplication and division, and plus and minus have the same priority we also create a multi-dimensional
    //array defining the steps of the order of operations (shoutout to Tau for idea)
    public static List<char[]> OrderOfOperations =>
        new() { new []{ MultiplicationOperator, DivisionOperator }, new []{ PlusOperator, MinusOperator } };


    private static void Main(string[] args)
    {
        //Since we're not gonna do operations on our history list we can just save it as a series of strings
        List<string> history = new List<string>();
        
        //Display our welcome message
        PrintSign("Welcome to the Calculator!");

        bool calculatorRunning = true;
        while (calculatorRunning)
        {

            //Input
            bool correctInput = true;
            MathematicalExpression parsedInput;
            do
            {
                correctInput = true;
                Console.Write("Input your calculation: ");
                string input = Console.ReadLine() ?? "";

                //Send it to our parser and get a parsed version back
                parsedInput = CalculatorInputParser.ParseInput(input);

                //If the parsed version is not in a valid state, print why and repeat input
                if (parsedInput.ValidityStatus != MathematicalExpression.Validity.Valid)
                {
                    correctInput = false;
                    PrintValidityError(parsedInput);
                    //I'm still adding it to the history-list though, which is why I needed to refactor Print to both Print & Get error message
                    history.Add($"{parsedInput} = {GetValidityErrorMessage(parsedInput)}");
                }
            } while (!correctInput);
            

            //Now that we have a correct expression split up, it's time to actually calculate the results
            double answer = ExpressionCalculator.Calculate(parsedInput);
            
            //Then we construct our full answer
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

            //Ask if they want to restart, and if no then set our running to false
            calculatorRunning = PromptForChoice("Restart the Calculator?");
        }

        PrintSign("You are now leaving the Calculator. Goodbye!");
    }

    /// <summary>
    /// Prints different console messages based on the error state of the passed in MathematicalExpression
    /// </summary>
    private static void PrintValidityError(MathematicalExpression input)
    {
        Console.WriteLine($"Input error: {GetValidityErrorMessage(input)}");
    }

    /// <summary>
    /// Returns an error message based on the error state of the passed in MathematicalExpression
    /// </summary>
    private static string GetValidityErrorMessage(MathematicalExpression input)
    {
        switch (input.ValidityStatus)
        {
            case MathematicalExpression.Validity.ParseError:
                return "Incorrect format of the entered values.";
            case MathematicalExpression.Validity.OrderError:
                return "Confusing operator order.";
            case MathematicalExpression.Validity.DivisionByZeroError:
                return "No dividing by zero.";
            case MathematicalExpression.Validity.TooShortError:
                return "The entered expression was too short.";
            default:
                return "Unknown Input Error.";
        }
    }
    
    /// <summary>
    /// Prompts the Player for yes / no on a selected text
    /// </summary>
    /// <param name="promptText">What will be printed, followed by [y/n]:</param>
    /// <returns>True if the player answers y</returns>
    private static bool PromptForChoice(string promptText)
    {
        //Since I converted this from a previous ReadLine I'm still doing string comparisons for input. A clearer version would directly check .Key instead

        //Nothing too fancy, we ask for the response and returns true if it's yes
        string playerResponse = "";
        //ToLower so we don't have to deal with Yy and Nn differences
        while (playerResponse.ToLower() != "y" && playerResponse.ToLower() != "n")
        {
            Console.Write($"{promptText} [y/n]: ");
            //?? so we don't have to worry about null
            playerResponse = Console.ReadKey().KeyChar.ToString().ToLower();
            Console.WriteLine();
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