using System;
using System.Collections.Generic;

namespace vqm2MNET
{
    class Program
    {
	static Module module;
	static void Main (string[] args)
	{
		module = new Module();
		string InFileName = "test.vqm";
		string[] InFileData = System.IO.File.ReadAllLines (InFileName);
		string[] CleanStrings = ClearData (InFileData);
		for (int i=0; i<CleanStrings.Length; i++) 
		{
			ProccessString(CleanStrings[i]);
		}
        }

	static void ProccessString (string str)
	{
		string[] Params = str.Split(' ');
		switch (Params[0]) 
		{
		case "module":
			module.Name = Params[1];
			break;
		default:
			break;
		}
		//Метод не окончен!
	}

        private static string[] ClearData(string[] InFileData)
        {
            List<string> CleanList = new List<string>();
            string tstr = "";

            for (int i = 0; i < InFileData.Length; i++)
            {
                if (!InFileData[i].Trim().StartsWith(@"//"))
                {
                    if (InFileData[i].Trim() != "")
                    {
                        string ts1 = InFileData[i].Trim();

                        for (int j = 0; j < ts1.Length; j++)
                        {
                            if (ts1.Substring(j, 1) == ";")
                            {
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
