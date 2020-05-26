using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorHandling.Config
{
    public class Utils
    {
        public static void WriteLine(string text, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
        }
    }
}
