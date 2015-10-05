using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new ConnectionRunner();
            runner.Run();
            Console.ReadLine();
        }
    }
}
