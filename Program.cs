using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChicagoV6
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            FileStream fs = new FileStream(@"Crimes_-_2001_to_present.csv", FileMode.Open, FileAccess.ReadWrite);
            var list = new List<string>();
            using (var reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("ASSAULT") || line.Contains("OVER $500") || line.Contains("$500 AND UNDER") || line.Contains("2015"))
                    {
                        string[] values = line.Split('"');
                        if (values.Length > 1)
                            values[1] = values[1].Replace(",", " ");
                        line = string.Empty;
                        foreach (var a1 in values)
                        {
                            line += a1;
                        }
                        list.Add(line);
                    }
                }
            }
            string[] result = list.ToArray();

            int[] Flag2 = new int[32];
            int[] Flag3 = new int[32];
            int[] Flag4 = new int[4];

            string[] heading = new string[] { "YEAR", "OVER_$500", "$500_AND_UNDER" };
            string[] heading2 = new string[] { "YEAR", "Arrest", "Not_Arrest" };


            int firstYear = 2001;
            int intYear, Code;

            foreach (var i in result)
            {
                var currRow = i.Split('\n');
                foreach (var j in currRow)
                {
                    var withInRow = j.Split(',');

                    if (int.TryParse(withInRow[17], out intYear))
                    {
                        for (int k = 0; k < 31; k = k + 2)
                        {
                            if (intYear == firstYear + (k / 2))
                            {
                                int a = (withInRow[6] == "OVER $500") ? Flag2[k] = Flag2[k] + 1 : ((withInRow[6] == "$500 AND UNDER") ? Flag2[k + 1] = Flag2[k + 1] + 1 : 0);
                                int b = (withInRow[5] == "ASSAULT" && withInRow[8] == "true") ? Flag3[k] = Flag3[k] + 1 : ((withInRow[5] == "ASSAULT" && withInRow[8] == "false") ? Flag3[k + 1] = Flag3[k + 1] + 1 : 0);

                            }

                        }
                        if (intYear == 2015)
                        {
                            if (int.TryParse(withInRow[14], out Code))
                            {
                                if (withInRow[14] == "01A" || withInRow[14] == "04A" || withInRow[14] == "04B" || withInRow[14] == "04A" || Code == 02 || Code == 03 || Code == 05 || Code == 06 || Code == 07 || Code == 09)
                                {
                                    Flag4[0] = Flag4[0] + 1;
                                }

                                if (withInRow[14] == "01B" || withInRow[14] == "08A" || withInRow[14] == "08B" || withInRow[14] == "04A" || (Code <= 10 && Code <= 20) || Code == 22 || Code == 24 || Code == 26)
                                {
                                    Flag4[1] = Flag4[1] + 1;
                                }

                                if (withInRow[14] == "01A" || withInRow[14] == "04A" || withInRow[14] == "04B" || Code == 02 || Code == 03 || Code == 14)
                                {
                                    Flag4[2] = Flag4[2] + 1;
                                }
                                if (Code == 05 || Code == 06 || Code == 07 || Code == 09)
                                {
                                    Flag4[3] = Flag4[3] + 1;
                                }
                            }
                        }
                    }
                }
            }
            StringBuilder sbFileA = new StringBuilder();
            StringBuilder sbFileB = new StringBuilder();
            StringBuilder sbFileC = new StringBuilder();

            sbFileA.Append("[").Append('\n');
            sbFileB.Append('{').Append("\"Assault\":[").Append('\n');

            for (int k = 0; k < 31; k = k + 2)
            {
                sbFileA.Append('{').AppendFormat("\"{0}\":\"{1}\"", heading[0], firstYear + (k / 2)).Append(',').Append('\n').AppendFormat("\"{0}\":\"{1}\"", heading[1], Flag2[k]).Append(',').Append('\n').AppendFormat("\"{0}\":\"{1}\"", heading[2], Flag2[k + 1]).Append('}');
                sbFileB.Append('{').AppendFormat("\"{0}\":\"{1}\"", heading2[0], firstYear + (k / 2)).Append(',').Append('\n').AppendFormat("\"{0}\":\"{1}\"", heading2[1], Flag3[k]).Append(',').Append('\n').AppendFormat("\"{0}\":\"{1}\"", heading2[2], Flag3[k + 1]).Append('}'); ;

                if (firstYear + (k / 2) != 2016)
                {
                    sbFileA.Append(',').Append('\n');
                    sbFileB.Append(',').Append('\n');
                }
            }
            sbFileA.Append(']').Append('}');
            sbFileB.Append(']').Append('}');
            sbFileC.Append("[").Append('\n').Append('{').AppendFormat("\"Crime\":\"Indexed_Crime\"").Append(',').AppendFormat("\"Value\":{0}", Flag4[0]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Non-Indexed_Crime\"").Append(',').AppendFormat("\"Value\":{0}", Flag4[1]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Violent_Crime\"").Append(',').AppendFormat("\"Value\":{0}", Flag4[2]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Property_Crime\"").Append(',').AppendFormat("\"Value\":{0}", Flag4[3]).Append('}').Append('\n').Append(']');
            FileStream fs2 = new FileStream(@"c:\users\training\source\repos\ChicagoV6\ChicagoV6\Json\JsonsampleJSON.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs2);
            sw.WriteLine(sbFileA);
            sw.Flush();

            FileStream fs3 = new FileStream(@"c:\users\training\source\repos\ChicagoV6\ChicagoV6\Json\sampleJSON2.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs3);
            sw.WriteLine(sbFileB);
            sw.Flush();

            FileStream fs4 = new FileStream(@"c:\users\training\source\repos\ChicagoV6\ChicagoV6\Json\sampleJSON3.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs4);
            sw.WriteLine(sbFileC);
            sw.Flush();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs);
        }
    }
}

