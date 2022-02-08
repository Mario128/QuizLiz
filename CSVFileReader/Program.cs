using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVFileReader
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fileContent;
            string[] lineparts;
            List<string> newFileContent = new List<string>();

            fileContent = File.ReadAllLines("Country_Flags.csv").ToList();
            
            foreach (string s in fileContent)
            {
                lineparts = s.Split(',');
                newFileContent.Add("3," + lineparts[1] + "," + lineparts[0]);
            }
            
            File.AppendAllLines("CountryFlagsNew.csv", newFileContent);

            Console.ReadKey();
        }
    }
}
