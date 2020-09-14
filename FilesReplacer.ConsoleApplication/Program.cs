using System;

namespace FilesReplacer.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            FilesReplacer.BusinessLayer.FilesReplacer fr = new BusinessLayer.FilesReplacer();
            fr.Replace();

            Console.ReadKey();
        }
    }
}
