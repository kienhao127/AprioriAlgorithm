using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriAlgorithm
{
    public class item
    {
        public List<string> itemset = new List<string>();
        public int count = 0;
    }
    class Program
    {
        public static List<string> loadCsvFile(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string> lines = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                lines.Add(line);
            }
            return lines;
        }

        static void Main(string[] args)
        {
            List<string> foodMart = loadCsvFile("./../../FoodMart.csv");
        }
    }
}
