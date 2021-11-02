using System;
using System.Collections.Generic;
using System.Linq;
using ParsJson;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!  test ");
            string __path = @"E:\MLserver\data\#COMMON\DLL\mlserverNew.json";
//            var _parserJson = new ParserJson(__path).Run();
            new ParserJson(__path).Run();
        }
    }
}