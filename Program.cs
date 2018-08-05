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

        public static List<item> candidateGen(List<item> F)
        {
            int subset = F[0].itemset.Count;

            List<item> C = new List<item>();

            bool canCreate = true;
            int len = F[0].itemset.Count - 1;

            for (int i = 0; i < F.Count; i++)
            {
                for (int j = 1; j < F.Count; j++)
                {
                    if (!F[i].itemset[len].Equals(F[j].itemset[len]))
                    {
                        if (subset > 1)
                        {
                            for (int iF = 0; iF < len; iF++)
                            {
                                if (!F[i].itemset[iF].Equals(F[j].itemset[iF]))
                                {
                                    canCreate = false;
                                    break;
                                }
                                canCreate = true;
                            }
                        }

                        if (canCreate)
                        {
                            item c = new item();
                            c.itemset.AddRange(F[i].itemset.GetRange(0, F[i].itemset.Count));
                            c.itemset.Add(F[j].itemset[len]);
                            c.count = 0;
                            C.Add(c);
                            bool isRemove = true;
                            for (int ic = 0; ic < c.itemset.Count; ic++)
                            {
                                item t = new item();
                                t.itemset = c.itemset.GetRange(0, c.itemset.Count);
                                t.itemset.RemoveAt(ic);
                                
                                for (int iF1 = 0; iF1 < F.Count; iF1++)
                                {
                                    for (int ii = 0; ii < F[iF1].itemset.Count; ii++)
                                    {
                                        for (int jj = 0; jj < t.itemset.Count; jj++)
                                        {
                                            if (!F[iF1].itemset[ii].Equals(t.itemset[jj]))
                                            {
                                                break;
                                            }
                                            isRemove = false;
                                        }      
                                    }
                                }
                            }
                            if (isRemove)
                            {
                                C.Remove(c);
                            }
                        }
                    }
                }
            }
            return C;
        }

        static void Main(string[] args)
        {
            float minsup = (float)0.1;
            List<string> foodMart = loadCsvFile("./../../FoodMart.csv");

            List<item> headerTable = createHeaderTable(foodMart, minsup);

            headerTable = candidateGen(headerTable);
            headerTable = candidateGen(headerTable);
            //de xem ket qua C1
            for (int i = 0; i < headerTable.Count(); i++)
            {
                String temp = headerTable[i].itemset[0];
                Console.WriteLine("itemset: {0} \t count: {1}", headerTable[i].itemset[0], headerTable[i].count);
            }
        }

        public static List<item> createHeaderTable(List<string> foodMart, float minsup)
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

            int index = 0;
            while (index < headerTable.Count)
            {
                if ((float)headerTable[index].count / (foodMart.Count() - 1) < minsup)
                {
                    headerTable.RemoveAt(index);
                } 
                else
                {
                    index++;
                }
            }

            return headerTable;
        }
    }
}
