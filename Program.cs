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

        public static List<item> initPass(List<string> foodMart)
        {
            string[] item = foodMart[0].Split(',');

            List<item> headerTable = new List<item>();

            for (int i = 0; i < item.Count(); i++)
            {
                //Console.WriteLine(item[i]);
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

        public static void genRules(List<item> FRoot, List<item> F, float minconf)
        {
            bool isAppend = false;
            for (int i = 0; i < F.Count; i++)
            {
                if (F[i].itemset.Count < 2)
                {
                    continue;
                }
                List<item> H = new List<item>();
                for (int j = 0; j < F[i].itemset.Count; j++)
                {
                    item F_b = new item();
                    F_b.itemset.Add(F[i].itemset[j]);
                    item F_a = new item();
                    F_a.itemset = F[i].itemset.GetRange(0, j);
                    F_a.itemset.AddRange(F[i].itemset.GetRange(j+1, F[i].itemset.Count -1 - j));
                    float conf = (float)F[i].count / getItemCount(FRoot, F_a);
                    if (conf >= minconf)
                    {
                        H.Add(F_b);
                        //printRuleToScreen(F_a, F_b, conf);
                        writeARFile("./../../AR.txt", F_a, F_b, conf, isAppend);
                        isAppend = true;
                    }
                }
                ap_genRules(FRoot, F[i], H, F[i].itemset.Count, 1, minconf);
            }
        }

        public static void ap_genRules(List<item> FRoot, item Fk, List<item> H, int k, int m, float minconf)
        {
            List<item> Hm = new List<item>();
            if ((k > m + 1) && (H.Count > 0))
            {
                Hm = candidateGen(H);
                int i = 0;
                while (i < Hm.Count)
                {
                    item F_b = Hm[i];
                    item F_a = new item();
                    for (int z = 0; z < Fk.itemset.Count; z++)
                    {
                        for (int x = 0; x < F_b.itemset.Count; x++)
                        {
                            if (Fk.itemset[z] == F_b.itemset[x])
                            {
                                break;
                            }
                            if (x == F_b.itemset.Count - 1)
                            {
                                F_a.itemset.Add(Fk.itemset[z]);
                            }
                        }
                    }
                    float conf = (float)Fk.count / getItemCount(FRoot, F_a);
                    if (conf >= minconf)
                    {
                        //printRuleToScreen(F_a, F_b, conf);
                        writeARFile("./../../AR.txt", F_a, F_b, conf, true);
                        i++;
                    }
                    else
                    {
                        Hm.RemoveAt(i);
                    }
                }
                ap_genRules(FRoot, Fk, Hm, k, m + 1, minconf);
            }
        }

        public static int getItemCount(List<item> FRoot, item item)
        {
            for (int i = 0; i < FRoot.Count; i++)
            {
                if (FRoot[i].itemset.Count == item.itemset.Count)
                {
                    if (is2ItemEquals(FRoot[i], item))
                    {
                        return FRoot[i].count;
                    }
                }
            }
            return -1;
        }

        public static bool is2ItemEquals(item item1, item item2)
        {
            for (int i = 0; i < item2.itemset.Count; i++)
            {
                if (!(item1.itemset[i] == item2.itemset[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void printRuleToScreen(item F_a, item F_b, float conf)
        {
            Console.Write(conf.ToString("0.00") + " ");
            for (int a = 0; a < F_a.itemset.Count; a++)
            {
                Console.Write("{0}", F_a.itemset[a]);
                if (a < F_a.itemset.Count - 1)
                {
                    Console.Write(",");
                }
            }
            Console.Write("-> ");
            for (int b = 0; b < F_b.itemset.Count; b++)
            {
                Console.Write("{0}", F_b.itemset[b]);
                if (b < F_b.itemset.Count - 1)
                {
                    Console.Write(",");
                }
            }
            Console.WriteLine("");
        }

        public static void writeARFile(string filePath, item F_a, item F_b, float conf, bool isAppend)
        {
            using (StreamWriter writetext = new StreamWriter(filePath, isAppend))
            {
                writetext.Write(conf.ToString("0.00") + " ");
                for (int a = 0; a < F_a.itemset.Count; a++)
                {
                    writetext.Write("{0}", F_a.itemset[a]);
                    if (a < F_a.itemset.Count - 1)
                    {
                        writetext.Write(",");
                    }
                }
                writetext.Write("-> ");
                for (int b = 0; b < F_b.itemset.Count; b++)
                {
                    writetext.Write("{0}", F_b.itemset[b]);
                    if (b < F_b.itemset.Count - 1)
                    {
                        writetext.Write(",");
                    }
                }
                writetext.WriteLine("");
            }
        }

        static void Main(string[] args)
        {
            float minsupp = (float)0.2;
            float minconf = (float)0.4;

            //Đọc file CSV
            Console.WriteLine("Doc file csv ...");
            List<string> foodMart = loadCsvFile("./../../FoodMart.csv");
            string[] headerTitle = foodMart[0].Split(',');

            //Tạo bảng C1 (đếm số lần xuất hiện của từng hạng mục)
            Console.WriteLine("Dem so lan xuat hien cua tung hang muc...");
            List<item> C = initPass(foodMart);

            //Tạo tập phổ biến (Xuất file FI)
            Console.WriteLine("Dang xuat file FI.txt ...");
            List<item> F = createF(foodMart, C, minsupp);
            writeFile("./../../FI.txt", F, foodMart.Count - 1, false);
            List<item> Fk = F;

            int k = 2;
            if (F.Count == 0)
            {
                return;
            }
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

            //for (int i = 0; i < F.Count; i++)
            //{
            //    Console.Write("itemset: ");
            //    for (int j = 0; j < F[i].itemset.Count; j++)
            //    {
            //        Console.Write("{0} ", F[i].itemset[j]);
            //    }
            //    Console.WriteLine("\ncount: {0}\n", F[i].count);
            //}

            //Tạo luật kết hợp (Tạo file AR)
            Console.WriteLine("Dang xuat file AR.txt ...");
            genRules(F, F, minconf);
            Console.WriteLine("Hoan tat...");
        }
    }
}
