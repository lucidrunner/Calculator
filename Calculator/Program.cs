//Main reason not to use top level statements: Can't file-scope namespaces with them :(

using System.Diagnostics;

namespace Calculator;

internal class Program
{
    //TODO Check safer way to do this since these are currently editable in the program
    private static readonly char[] SupportedOperators = new[] { '*', '+', '-', '/'};

    static void Main(string[] args)
    {
        //Since we're not gonna do operations on our historical list we can just save it as a series of strings
        //(if we were gonna do operations, since we do them on strings this would still work as mostly futureproof though)
        List<string> history = new List<string>();

        //Display our welcome message
        PrintSign("Welcome to the Calculator!");
        bool calculatorRunning = true;
        while (calculatorRunning)
        {
            bool correctInput = true;
            //Using a do-while since we always wanna run this a minimum of one time
            do
            {
                //Input numbers - Basic version = we only deal with 2. I've decided to ask for it as a single string since it's easier to expand later
                Console.Write("Input your two numbered operation (eg x*y): ");
                correctInput = ReadInput(out ProcessedInput input);
                
                //If we did not read the input in a correct format, go to the next cycle of the while
                if(!correctInput)
                {
                    Console.WriteLine("Please provide the input in correct format.");
                    continue;
                }

                //Otherwise, start handling the input
                Console.WriteLine(input);

            } while (!correctInput);
            
            calculatorRunning = PromptForRestart();
        }
        PrintSign("You are now leaving the Calculator. Goodbye!");
    }

    private static bool ReadInput(out ProcessedInput processedInput)
    {
        //One negative part about the pattern I did in this method
        //We have to assign a default record to the out argument since we might need to leave early
        //A risk of doing this is that we later miss assigning parts to the record
        processedInput = new ProcessedInput(0, "*", 0);
        
        //Read the input
        string input = Console.ReadLine() ?? "";
        
        //Start by splitting it
        var inputParts = input.Split(SupportedOperators, StringSplitOptions.TrimEntries);
        
        //First fail state - If any parts are empty, or we don't have two parts in our expression we return false
        if (inputParts.Length != 2 && inputParts.Any(string.IsNullOrEmpty))
            return false;

        
        //Two hickups from splitting occur - the first is that we eliminate the operator from the parts
        //We use a method to get around this
        string mathOperator = GetOperator(input);
        //If the operator was incorrect, return false
        if (string.IsNullOrEmpty(mathOperator))
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
        processedInput = new ProcessedInput(firstNumber, mathOperator, secondNumber);

        //Finally, return true if we passed all our input checking
        return true;
    }


    private static string GetOperator(string input)
    {
        foreach (char supportedOperator in SupportedOperators)
        {
            if (input.Contains(supportedOperator))
                return supportedOperator.ToString();
        }

        return "";
    }


    /// <summary>
    /// Prompts the Player on if they want to restart the calculator
    /// </summary>
    /// <returns>True if the calculator should restart</returns>
    private static bool PromptForRestart()
    {
        //Nothing too fancy, we ask for the response and returns true if it's yes
        string playerResponse = "";
        //ToLower so we don't have to deal with Yy and Nn differences
        while (playerResponse.ToLower() != "y" && playerResponse.ToLower() != "n")
        {
            Console.Write("Restart the Calculator? [y/n]: ");
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
internal record ProcessedInput (double FirstInput, string OperatorSymbol, double SecondInput) { }