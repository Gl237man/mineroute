using System;
using System.Collections.Generic;

namespace vqm2MNET
{
    class Program
    {
		List<Cell> Cells;

        static void Main(string[] args)
        {
            string InFileName = "test.vqm";
            string[] InFileData = System.IO.File.ReadAllLines(InFileName);
            string[] CleanStrings = ClearData(InFileData);


        }

        private static string[] ClearData(string[] InFileData)
        {
            List<string> CleanList = new List<string>();
            string tstr = "";

            for (int i = 0; i < InFileData.Length; i++)
            {
                if (!InFileData[i].Trim().StartsWith(@"//"))
                {
                    if (!(InFileData[i].Trim() == ""))
                    {
                        string ts1 = InFileData[i].Trim();

                        for (int j = 0; j < ts1.Length; j++)
                        {
                            if (ts1.Substring(j, 1) == ";")
                            {
                                //tstr += ";";
                                CleanList.Add(tstr.Replace("\t","").Replace("\\",""));
                                tstr = "";
                            }
                            else
                            {
                                tstr += ts1.Substring(j, 1);
                            }
                        }
                    }
                }
            }

            return CleanList.ToArray();
        }
    }
}
