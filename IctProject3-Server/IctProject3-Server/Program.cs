using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IctProject3_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 11; i++)
            {
                Console.Beep();
                Thread.Sleep(180);
            }
            Console.ReadKey();

        }
    }
}
