using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    class ConsoleRunner
    {
        public static string Prompt(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public static string GetHost()
        {
            return Prompt("Host: ");
        }

        public static string GetPort()
        {
            return Prompt("Port: ");
        }

        public static string GetUsername()
        {
            return Prompt("Username: ");
        }

        public static string GetPassword()
        {
            return Prompt("Password: ");
        }

        
    }
}
