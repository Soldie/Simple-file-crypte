using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCrypter
{
    public static class ConsoleManager
    {
        public enum Colors { Default = ConsoleColor.Green, Error = ConsoleColor.Red, Important = ConsoleColor.White, Password = ConsoleColor.Yellow, File = ConsoleColor.Cyan }
        
        
        public static void Init()
        {
            Console.ForegroundColor = (ConsoleColor)Colors.Default;
        }

        public static void WriteLine(string message, Colors color)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine(message);
            Console.ForegroundColor = (ConsoleColor)Colors.Default;
        }

        public static void Write(string message, Colors color)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.Write(message);
            Console.ForegroundColor = (ConsoleColor)Colors.Default;
        }
    }
}
