using System;
namespace ORM_Dapper
{
	public static class UserInput
	{
		public static void ErrorMessage(string message)
		{
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void SuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static double ParseNumber(string input)
        {
            double userDouble;
            while (!double.TryParse(input, out userDouble))
            {
                ErrorMessage("ERROR: not readable as a number");
                Console.Write("Please try again: ");
                input = Console.ReadLine();
            }
            return userDouble;
        }

        public static bool YesOrNo(string input)
        {
            var yesOrNo = new string[] { "yes", "y", "no", "n" };
            while (!yesOrNo.Contains(input.ToLower()))
            {
                ErrorMessage("I'm sorry, I don't understand that request.");
                Console.Write("Please try again: ");
                input = Console.ReadLine();
            }

            switch (input.ToLower())
            {
                case "yes" or "y":
                    return true;
                case "no" or "n":
                    return false;
                default:
                    return false;
            }
        }

        public static string ValidInput(IEnumerable<string> valid, string input)
        {
            while (!valid.Contains(input))
            {
                ErrorMessage("I'm sorry, I don't understand that request.");
                Console.Write("Please try again: ");
                input = Console.ReadLine();
            }
            return input;
        }

        public static bool ConfirmInput(string confirmation, string input)
        {
            if (input.ToUpper() == confirmation || input.ToUpper() == $"'{confirmation}'") return true;
            else return false;
        }
    }
}

