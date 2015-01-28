using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestWeirdReference
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("calling Message...");
            Console.WriteLine(OriginalName.Message.GetMessage());
            Thread.Sleep(90);
            Console.WriteLine(OriginalName.Message.GetMessage());
            Thread.Sleep(90);
            Console.WriteLine(OriginalName.Message.GetMessage());
            Console.ReadLine();
        }
    }
}
