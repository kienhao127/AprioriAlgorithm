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

            List<item> headerTable = createHeaderTable(foodMart);

            //de xem ket qua C1
            for (int i = 0; i < headerTable.Count(); i++)
            {
                String temp = headerTable[i].itemset[0];
                Console.WriteLine("itemset: {0} \t count: {1}", headerTable[i].itemset[0], headerTable[i].count);
            }
        }

        public static List<item> createHeaderTable(List<string> foodMart)
        {
            string[] item = foodMart[0].Split(',');

            List<item> headerTable = new List<item>();

            for (int i = 0; i < item.Count(); i++)
            {
                Console.WriteLine(item[i]);
                item it = new item();
                it.itemset.Add(item[i]);
                it.count = 0;
                headerTable.Add(it);
            }

            for (int i = 1; i < foodMart.Count() - 1; i++)
            {
                string[] subItem = foodMart[i].Split(',');

                for (int j = 0; j < subItem.Count(); j++)
                {
                    if (subItem[j] == "1")
                    {
                        headerTable[j].count++;
                    }
                }
            }

            return headerTable;
        }
    }
}
