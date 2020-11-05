using System;
using System.IO;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Please enter file name");

        string fileName = Console.ReadLine();

        if (fileName == "factorial.txt" || fileName == "sort.txt" || fileName == "fibonacci.txt")
        {
            var fileReader = new StreamReader(fileName);
            var scriptExecutor = new BaseParser(fileReader);
            scriptExecutor.addStatements();
            scriptExecutor.executeScript();
        }
        else
        {
            Console.WriteLine("Incorrect file name");
            Console.ReadKey();
            Program.Main(args);
        }
    }
}
