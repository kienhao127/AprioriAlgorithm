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
                for (int j = i + 1; j < F.Count; j++)
                {
                    if (!F[i].itemset[len].Equals(F[j].itemset[len]))
                    {
                        if (subset > 1)
                        {
                            for (int iF = 0; iF < len; iF++)
                            {
                                //   Console.WriteLine("iF {0}", iF);
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

                                //  Console.WriteLine("ic {0}", ic);

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

        public static void writeFile(string filePath, List<item> F, int n, bool isAppend)
        {
            using (StreamWriter writetext = new StreamWriter(filePath, isAppend))
            {
                if (F.Count > 0)
                {
                    writetext.WriteLine(F.Count);
                    for (int i = 0; i < F.Count; i++)
                    {
                        float sup = (float)F[i].count / n;
                        string itemset = "";
                        for (int j = 0; j < F[i].itemset.Count; j++)
                        {
                            itemset += " " + F[i].itemset[j];
                        }
                        writetext.WriteLine(sup.ToString("0.00") + itemset);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            float minsupp = (float)0.2;
            List<string> foodMart = loadCsvFile("./../../FoodMart.csv");
            string[] headerTitle = foodMart[0].Split(',');
            List<item> C = initPass(foodMart);
            List<item> F = createF(foodMart, C, minsupp);
            writeFile("./../../FI.txt", F, foodMart.Count - 1, false);
            List<item> Fk = F;

            int k = 2;
            do
            {
                C = candidateGen(Fk);

                for (int i = 1; i < foodMart.Count; i++)
                {
                    string[] subItem = foodMart[i].Split(',');

                    for (int j = 0; j < C.Count; j++)
                    {
                        int subCount = 0;
                        for (int t = 0; t < C[j].itemset.Count; t++)
                        {
                            for (int s = 0; s < subItem.Count(); s++)
                            {
                                if (C[j].itemset[t] == headerTitle[s] && subItem[s] == "1")
                                {
                                    subCount++;
                                }
                            }
                        }
                        if (subCount == k)
                        {
                            C[j].count++;
                        }
                    }
                }

                Fk = createF(foodMart, C, minsupp);
                writeFile("./../../FI.txt", Fk, foodMart.Count - 1, true);
                F.AddRange(Fk);
                k++;
            } while (Fk.Count > 0);

            for (int i = 0; i < F.Count; i++)
            {
                Console.Write("itemset: ");
                for (int j = 0; j < F[i].itemset.Count; j++)
                {
                    Console.Write("{0} ", F[i].itemset[j]);
                }
                Console.WriteLine("\ncount: {0}\n", F[i].count);
            }
        }

        public static List<item> initPass(List<string> foodMart)
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

            for (int i = 1; i < foodMart.Count; i++)
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

        public static List<item> createF(List<string> foodMart, List<item> Ck, float minsupp)
        {
            List<item> F = Ck;
            int index = 0;
            while (index < F.Count)
            {
                if ((float)F[index].count / (foodMart.Count() - 1) < minsupp)
                {
                    F.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            return F;
        }
    }
}
