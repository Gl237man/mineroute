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
		case "assign":
				for (int i=0;i<module.Wires.Count;i++)
				{
					if (Params[1] == module.Wires[i].Name)
					{
						switch (Params[3]) 
						{
						case "1'b0":
							module.Wires[i].Connections.Add(new Connection(){Direction = direction.From, CPoint = "Const0"});
							break;
						case "1'b1":
							module.Wires[i].Connections.Add(new Connection(){Direction = direction.From, CPoint = "Const1"});
							break;
						default:
							module.Wires[i].Connections.Add(new Connection(){Direction = direction.From, CPoint = Params[3]});
							break;
						}
					}
				}
			break;
		case "module":
				module.Name = Params[1];
			break;
		case "wire":
				Wire TWire = new Wire();
				TWire.Name = Params[1];
				module.Wires.Add(TWire);
			break;

		case "input":
			if (Params[1].StartsWith("[") && Params[1].EndsWith("]"))
			{
				Params[1] = Params[1].Replace("[", "").Replace("]", "");
				string[] SubDat = Params[1].Split(':');
				int SFrom = Convert.ToInt32(SubDat[1]);
				int STo = Convert.ToInt32(SubDat[0]);
				for (int i = SFrom; i <= STo; i++)
				{
					module.Ports.Add(new IOPort() { Name = Params[2]+"["+i.ToString()+"]", Ptype = PortType.IN });
				}
			}
			else
			{
				module.Ports.Add(new IOPort() { Name = Params[1], Ptype = PortType.IN });
			}
			break;
		case "output":
			if (Params[1].StartsWith("[") && Params[1].EndsWith("]"))
			{
				Params[1] = Params[1].Replace("[", "").Replace("]", "");
				string[] SubDat = Params[1].Split(':');
				int SFrom = Convert.ToInt32(SubDat[1]);
				int STo = Convert.ToInt32(SubDat[0]);
				for (int i = SFrom; i <= STo; i++)
				{
					module.Ports.Add(new IOPort() { Name = Params[2] + "[" + i.ToString() + "]", Ptype = PortType.OUT });
				}
			}
			else
			{
				module.Ports.Add(new IOPort() { Name = Params[1], Ptype = PortType.OUT });
			}
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
