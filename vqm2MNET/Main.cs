using System;
using System.Collections.Generic;

namespace vqm2MNET
{
    class NetLink
    {
        public string FromDev;
        public string FromPort;
        public string ToDev;
        public string ToPort;
    }
    class Node
    {
        public string NodeType;
        public string NodeName;
    }

    class Program
    {
        static Module module;
        static void Main(string[] args)
        {
            string InFile = "test2";
            if (args.Length == 1)
            {
                InFile = args[0];
            }
            module = new Module();
            string InFileName = InFile + ".vqm";
            string[] InFileData = System.IO.File.ReadAllLines(InFileName);
            string[] CleanStrings = ClearData(InFileData);
            for (int i = 0; i < CleanStrings.Length; i++)
            {
                ProccessString(CleanStrings[i]);
            }
            List<Node> Nodes = new List<Node>();
            List<NetLink> Links = new List<NetLink>();
            //Заполнение констант
            FillConst(Nodes);
            //Заполнение портов
            FillPorts(Nodes);
            //Заполнение ячеек
            FillCells(Nodes);
            //Заполнение DupСоеденений
            //FillDup(Nodes);
            //Заполнение Соеденений
            FillLinkCell(Links);
            FillLinkPorts(Links);
            //Удаление пустых и односложных Wire
            while (WireOptimize(Links)) { }

            FillDup(Nodes);
            FillLostDupLinks(Links);

            //Выгрузка

			Console.WriteLine("Nodes {0}",Nodes.Count);
			Console.WriteLine("Cells {0}",module.Cells.Count);
			Console.WriteLine("Wires {0}",Links.Count);

            string OutFileName = InFile + ".MNET";
            string Ofile = "";

            for (int i = 0; i < Nodes.Count; i++)
            {
                Ofile += "NODE:" + Nodes[i].NodeType + ":" + Nodes[i].NodeName + "\r\n";
            }

            for (int i = 0; i < Links.Count; i++)
            {
                Ofile += "WIRE:" + Links[i].FromDev + "-" + Links[i].FromPort + ":" + Links[i].ToDev + "-" + Links[i].ToPort + "\r\n";
            }
            System.IO.File.WriteAllText(OutFileName, Ofile);
        }
        //Проверка на входящие линки на ветвитель
        //Хак для обхода модулей синхронизации
        private static void FillLostDupLinks(List<NetLink> Links)
        {
            for (int i = 0; i < module.Wires.Count; i++)
            {
                string LinkName = "";
                for (int j=0;j<module.Wires[i].Connections.Count;j++)
                {
                    if (module.Wires[i].Connections[j].Direction == direction.From)
                    {
                        LinkName = module.Wires[i].Connections[j].CPoint;
                    }
                }

                if (!CheckToDupLink(LinkName, Links, module.Wires[i].Name))
                {
                    NetLink Nl = new NetLink();
                    Nl.FromDev = LinkName;
                    Nl.FromPort = "O0";
                    Nl.ToDev = module.Wires[i].Name;
                    Nl.ToPort = "I0";
                    Links.Add(Nl);
                }
            }
        }

        private static bool CheckToDupLink(string LinkName, List<NetLink> Links, string dev)
        {
            for (int i = 0; i < Links.Count; i++)
            {
                if (Links[i].ToDev == dev)
                {
                    if (Links[i].FromDev == LinkName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void FillDup(List<Node> Nodes)
        {
            for (int i = 0; i < module.Wires.Count; i++)
            {
                Node N = new Node();
                N.NodeName = module.Wires[i].Name;
                N.NodeType = "DUP" + GetWireOutConnetionNum(module.Wires[i].Name);
                Nodes.Add(N);
            }
        }

        private static bool WireOptimize(List<NetLink> Links)
        {
            for (int i = 0; i < module.Wires.Count; i++)
            {
                int InPortNum = GetWireInConnetionNum(module.Wires[i].Name);
                int OutPortNum = GetWireOutConnetionNum(module.Wires[i].Name);
                if (InPortNum == 1 && OutPortNum == 1)
                {
                    NetLink NNL= new NetLink();
                    for (int j = 0; j < Links.Count; j++)
                    {
                        if (Links[j].ToDev == module.Wires[i].Name)
                        {
                            NNL.FromDev = Links[j].FromDev;
                            NNL.FromPort = Links[j].FromPort;
                            Links.RemoveAt(j);
                        }
                    }
                    for (int j = 0; j < Links.Count; j++)
                    {
                        if (Links[j].FromDev == module.Wires[i].Name)
                        {
                            NNL.ToDev = Links[j].ToDev;
                            NNL.ToPort = Links[j].ToPort;
                            Links.RemoveAt(j);
                        }
                    }
                    Links.Add(NNL);
                }
                if ((InPortNum + OutPortNum) < 3)
                {
                    module.Wires.RemoveAt(i); 
                    return true;
                }
            }
            return false;
        }


        private static void FillLinkPorts(List<NetLink> Links)
        {
            for (int i = 0; i < module.Ports.Count; i++)
            {
                string ConnectionName = module.Ports[i].Connection;
                string CellName = module.Ports[i].Name;
                PortType Ptype = module.Ports[i].Ptype;
                if (ConnectionName != null)
                {
                    if (IsWire(ConnectionName))
                    {
                        NetLink Nl = new NetLink();
                        switch (Ptype)
                        {
                            case PortType.IN:
                                if (!WireConnectionExist(ConnectionName, CellName))
                                    module.Wires[GetIdWireByName(ConnectionName)].Connections.Add(new Connection() { CPoint = CellName, Direction = direction.From });
                                Nl.FromDev = CellName;
                                Nl.FromPort = "O0";
                                Nl.ToDev = ConnectionName;
                                Nl.ToPort = "I" + (GetWireConnetionNum(ConnectionName, CellName)).ToString();
                                Links.Add(Nl);
                                break;
                            case PortType.OUT:
                                if (!WireConnectionExist(ConnectionName, CellName))
                                    module.Wires[GetIdWireByName(ConnectionName)].Connections.Add(new Connection() { CPoint = CellName, Direction = direction.To });
                                Nl.FromDev = ConnectionName;
                                Nl.FromPort = "O" + (GetWireConnetionNum(ConnectionName, CellName)).ToString();
                                Nl.ToDev = CellName;
                                Nl.ToPort = "I0";
                                Links.Add(Nl);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static void FillLinkCell(List<NetLink> Links)
        {
            for (int i = 0; i < module.Cells.Count; i++)
            {
                AddInCellLink(Links, module.Cells[i].dataa, module.Cells[i].Name, "dataa");
                AddInCellLink(Links, module.Cells[i].datab, module.Cells[i].Name, "datab");
                AddInCellLink(Links, module.Cells[i].datac, module.Cells[i].Name, "datac");
                AddInCellLink(Links, module.Cells[i].datad, module.Cells[i].Name, "datad");
                AddInCellLink(Links, module.Cells[i].cin, module.Cells[i].Name, "cin");
                AddOutCellLink(Links, module.Cells[i].combout, module.Cells[i].Name, "combout");
                AddOutCellLink(Links, module.Cells[i].cout, module.Cells[i].Name, "cout");

				AddInCellLink(Links, module.Cells[i].clk, module.Cells[i].Name, "clk");
				AddInCellLink(Links, module.Cells[i].datain, module.Cells[i].Name, "datain");
				AddOutCellLink(Links, module.Cells[i].regout, module.Cells[i].Name, "regout");
            }
        }

        private static void AddOutCellLink(List<NetLink> Links, string ConnectionName, string CellName, string PortName)
        {
            if (ConnectionName != null)
            {
                if (IsPort(ConnectionName))
                {
                    NetLink Nl = new NetLink();
                    Nl.FromDev = CellName;
                    Nl.FromPort = PortName;
                    Nl.ToDev = ConnectionName;
                    Nl.ToPort = "I0";
                    Links.Add(Nl);
                }

                if (IsWire(ConnectionName))
                {
                    if (!WireConnectionExist(ConnectionName,CellName))
                        module.Wires[GetIdWireByName(ConnectionName)].Connections.Add(new Connection() { CPoint = CellName, Direction = direction.From });
                    NetLink Nl = new NetLink();
                    Nl.FromDev = CellName;
                    Nl.FromPort = PortName;
                    Nl.ToDev = ConnectionName;
                    Nl.ToPort = "I" + (GetWireInConnetionNum(ConnectionName) - 1).ToString();
                    Links.Add(Nl);
                }
            }
        }

        private static object GetWireConnetionNum(string ConnectionName, string CellName)
        {
            int WirNum = GetIdWireByName(ConnectionName);
            int Onum = 0;
            for (int i = 0; i < module.Wires[WirNum].Connections.Count; i++)
            {
                if (module.Wires[WirNum].Connections[i].CPoint == CellName) Onum = i;
            }
            return Onum;
            
        }

        private static bool WireConnectionExist(string ConnectionName, string CellName)
        {
            for (int i = 0; i < module.Wires[GetIdWireByName(ConnectionName)].Connections.Count; i++)
            {
                if (module.Wires[GetIdWireByName(ConnectionName)].Connections[i].CPoint == CellName) return true;
            }
            return false;
        }

        private static int GetWireInConnetionNum(string ConnectionName)
        {
            int WirNum = GetIdWireByName(ConnectionName);
            int Onum = 0;
            for (int i = 0; i < module.Wires[WirNum].Connections.Count; i++)
            {
                if (module.Wires[WirNum].Connections[i].Direction == direction.From)
                {
                    Onum++;
                }
            }
            return Onum;
        }

        private static void AddInCellLink(List<NetLink> Links, string ConnectionName, string CellName, string PortName)
        {
            if (ConnectionName != null)
            {
                if (IsPort(ConnectionName))
                {
                    NetLink Nl = new NetLink();
                    Nl.FromDev = ConnectionName;
                    Nl.FromPort = "O0";
                    Nl.ToDev = CellName;
                    Nl.ToPort = PortName;
                    Links.Add(Nl);
                }
                if (IsWire(ConnectionName))
                {
                    //if (WireConnectionExist(ConnectionName, CellName))
                        module.Wires[GetIdWireByName(ConnectionName)].Connections.Add(new Connection() { CPoint = CellName, Direction = direction.To });
                    NetLink Nl = new NetLink();
                    Nl.FromDev = ConnectionName;
                    Nl.FromPort = "O" + (GetWireOutConnetionNum(ConnectionName) - 1).ToString();
                    Nl.ToDev = CellName;
                    Nl.ToPort = PortName;
                    Links.Add(Nl);
                }
            }
        }

        private static int GetWireOutConnetionNum(string ConnectionName)
        {
            int WirNum = GetIdWireByName(ConnectionName);
            int Onum = 0;
            for (int i = 0; i < module.Wires[WirNum].Connections.Count; i++)
            {
                if (module.Wires[WirNum].Connections[i].Direction == direction.To)
                {
                    Onum++;
                }
            }
            return Onum;
        }

        private static int GetIdWireByName(string NodeName)
        {
            for (int i = 0; i < module.Wires.Count; i++)
            {
                if (module.Wires[i].Name == NodeName) return i;
            }
            return -1;
        }

        private static bool IsWire(string NodeName)
        {
            for (int i = 0; i < module.Wires.Count; i++)
            {
                if (module.Wires[i].Name == NodeName) return true;
            }
            return false;
        }

        private static bool IsPort(string NodeName)
        {
            for (int i = 0; i < module.Ports.Count; i++)
            {
                if (module.Ports[i].Name == NodeName) return true;
            }
            return false;
        }

        private static void FillConst(List<Node> Nodes)
        {
            Nodes.Add(new Node() { NodeName = "Const0", NodeType = "GND" });
            Nodes.Add(new Node() { NodeName = "Const1", NodeType = "VCC" });
        }

        private static void FillCells(List<Node> Nodes)
        {
            for (int i = 0; i < module.Cells.Count; i++)
            {
                Node node = new Node();
                node.NodeName = module.Cells[i].Name;
                switch (module.Cells[i].CelType)
                {
                    case CellType.cycloneii_lcell_comb:
                        node.NodeType = "C2LUT_" + module.Cells[i].lut_mask + "_" + module.Cells[i].sum_lutc_input;
                        break;
                    case CellType.cycloneii_lcell_ff:
                        node.NodeType = "TRIG_D";
                        break;
                    default:
                        break;
                }
                Nodes.Add(node);
            }
        }

        private static void FillPorts(List<Node> Nodes)
        {
            for (int i = 0; i < module.Ports.Count; i++)
            {
                Node node = new Node();
                node.NodeName = module.Ports[i].Name;
                node.NodeType = module.Ports[i].Ptype == PortType.IN ? "INPort" : "OUTPort";
                Nodes.Add(node);
            }
        }

        static void ProccessString(string str)
        {
            string[] Params = str.Split(' ');
            switch (Params[0])
            {
                case "defparam":
                    for (int i = 0; i < module.Cells.Count; i++)
                    {
                        if (module.Cells[i].Name == Params[1])
                        {
                            switch (Params[2])
                            {
                                case ".sum_lutc_input":
                                    module.Cells[i].sum_lutc_input = Params[4].Trim(new char[] { '"' });
                                    break;
                                case ".lut_mask":
                                    module.Cells[i].lut_mask = Params[4].Trim(new char[] { '"' });
                                    break;
                                default:
                                    throw new Exception("Свойство " + Params[2] + " не обрабатывается");
                                //break;
                            }
                        }
                    }
                    break;
				case "cycloneii_lcell_ff":

					Cell cell1 = new Cell(CellType.cycloneii_lcell_ff);
                    cell1.Name = Params[1];
                    cell1.clk = GetSubParamCell(Params[2], "clk");
                    cell1.datain = GetSubParamCell(Params[3], "datain");
                    cell1.regout = GetSubParamCell(Params[4], "regout");

                    module.Cells.Add(cell1);

				    break;
                case "cycloneii_lcell_comb":
                    Cell cell = new Cell(CellType.cycloneii_lcell_comb);
                    cell.Name = Params[1];
                    string ParamS = CobineParam(Params);
                    cell.dataa = GetSubParamCell(ParamS, "dataa");
                    cell.datab = GetSubParamCell(ParamS, "datab");
                    cell.datac = GetSubParamCell(ParamS, "datac");
                    cell.datad = GetSubParamCell(ParamS, "datad");
                    cell.combout = GetSubParamCell(ParamS, "combout");
                    cell.cin = GetSubParamCell(Params[2], "cin"); // Надо проверить на других тестовых примерах
                    module.Cells.Add(cell);
                    break;
                case "cycloneii_clkctrl":

                    for (int i = 0; i < module.Wires.Count; i++)
                    {
                        if (GetSubParamCell(Params[2], "outclk") == module.Wires[i].Name)
                        {
                            module.Wires[i].Connections.Add(new Connection() { Direction = direction.From, CPoint = GetSubParamCell(Params[2], "inclk").Replace("{","").Replace("}","") });
                        }
                    }
                    
                    for (int i = 0; i < module.Ports.Count; i++)
                    {
                        if (GetSubParamCell(Params[2], "inclk") == module.Ports[i].Name)
                        {
                            module.Ports[i].Connection = GetSubParamCell(Params[2], "outclk".Replace("{","").Replace("}",""));
                        }
                    }
                    
                    break;
                case "assign":
                    //Assign Ports
                    for (int i = 0; i < module.Ports.Count; i++)
                    {
                        if (Params[1] == module.Ports[i].Name)
                        {
                            module.Ports[i].Connection = Params[3];
                        }
                    }
                    //Assign Wires
                    for (int i = 0; i < module.Wires.Count; i++)
                    {
                        if (Params[3] == module.Wires[i].Name)
                        {
                            module.Wires[i].Connections.Add(new Connection() { Direction = direction.To, CPoint = Params[1] });
                        }
                        if (Params[1] == module.Wires[i].Name)
                        {

                            switch (Params[3])
                            {
                                case "1'b0":
                                    module.Wires[i].Connections.Add(new Connection() { Direction = direction.From, CPoint = "Const0" });
                                    break;
                                case "1'b1":
                                    module.Wires[i].Connections.Add(new Connection() { Direction = direction.From, CPoint = "Const1" });
                                    break;
                                default:
                                    module.Wires[i].Connections.Add(new Connection() { Direction = direction.From, CPoint = Params[3] });
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
                            module.Ports.Add(new IOPort() { Name = Params[2] + "[" + i.ToString() + "]", Ptype = PortType.IN });
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
        }

        private static string CobineParam(string[] Params)
        {
            string r = "";
            for (int i = 2; i < Params.Length; i++)
            {
                r += Params[i];
            }
            return r;
        }

        private static string GetSubParamCell(string pdata, string pname)
        {
            string[] Params = pdata.Split(',');
            for (int i = 0; i < Params.Length; i++)
            {
                Params[i] = Params[i].Trim(new char[] { '(', ')', '.' }).Replace("(", " ");
            }
            for (int i = 0; i < Params.Length; i++)
            {
                if (Params[i].Split(' ')[0] == pname) return Params[i].Split(' ')[1];
            }
            return null;
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
                                CleanList.Add(tstr.Replace("\t", "").Replace("\\", ""));
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
