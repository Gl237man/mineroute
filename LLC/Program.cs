using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename;
            if (args.Length > 0)
            {
                filename = args[0];
            }
            else
            {
                filename = "Test.LLC";
            }

            Scanner scanner = new Scanner(filename);
            Parser parser = new Parser(scanner);
            parser.tab = new SymbolTable(parser);
            parser.gen = new CodeGenerator();
            parser.Parse();

            if (parser.errors.count == 0)
            {
                Console.WriteLine("Проверка синтаксиса прошла успешно");
                //Состовление списка нодов
                var mNodes = parser.tab.PortObjs.Select(t => new MultiNode {BaseType = t.type+"Port",name = t.name,wide = t.wide }).ToList();
                mNodes.AddRange(parser.tab.Trigers.Select(t => new MultiNode {BaseType = "TRIGD",name = t.name,wide = t.wide }));
                mNodes.AddRange(parser.tab.WireObjs.Select(t => new MultiNode { BaseType = "DUMMY", name = t.name, wide = t.wide }));
                mNodes.AddRange(parser.tab.ConstObjs.Select(t => new MultiNode { BaseType = "CONST_" + t.val.ToString("X4") + "_" , name = t.name, wide = t.wide }));
                mNodes.AddRange(parser.tab.Bops.Select(t => new MultiNode { BaseType = t.opType, name = t.Name, wide = 0 }));
                //Состовление списка соеденений
                var mWires = parser.tab.conections.Select(t => new MultiWire { Src = t.from, Dist = t.to, DistPort = t.ToPort, SrcPort = t.FromPort }).ToList();
                //Определение размерностей
                //Установка выходов на логических опперациях
                var logicOp = mNodes.Where(t => t.BaseType == "EQ" || t.BaseType == "NOTEQ" || t.BaseType == "MORE"
                                             || t.BaseType == "LESS" || t.BaseType == "LESSEQ" || t.BaseType == "MOREEQ").ToList();
                foreach (var node in logicOp)
                {
                    node.outWide = 1;
                }

                bool NextIterration = true;

                while (NextIterration)
                {
                    NextIterration = false;
                    //установка воходов на нодах
                    var unUpdatedOp = mNodes.Where(t => t.outWide == 0);
                    foreach (var node in unUpdatedOp)
                    {
                        if (node.wide != 0)
                        {
                            node.outWide = node.wide;
                            NextIterration = true;
                        }
                    }
                    //Установка размерности соеденений
                    var unUpdatetWires = mWires.Where(t => t.Wide == 0).ToList();
                    foreach (var wire in unUpdatetWires)
                    {
                        var srcNode = mNodes.First(t => t.name == wire.Src);
                        if (srcNode.outWide > 0)
                        {
                            wire.Wide = srcNode.outWide;
                            NextIterration = true;
                        }
                    }
                    var unWidetOp = mNodes.Where(t => t.wide == 0);
                    foreach (var node in unWidetOp)
                    {
                        var fromWire = mWires.First(t => t.Dist == node.name);
                        if (fromWire.Wide > 0)
                        {
                            node.wide = fromWire.Wide;
                        }
                    }
                }
                //Проверка размерностей

                //Определение финалного типа
                foreach (var node in mNodes)
                {
                    node.Type = node.BaseType + node.wide;    
                }
                //Удаление Dummy обьектов
                var dummy = mNodes.Where(t => t.BaseType == "DUMMY").ToList();
                foreach (var node in dummy)
                {
                    var toWires = mWires.Where(t => t.Dist == node.name).ToList();
                    var fromWires = mWires.Where(t => t.Src == node.name).ToList();
                    if (toWires.Count > 1)
                    {
                        Console.WriteLine("ERROR: Есть неоднозначные соеденения");
                    }
                    foreach (var wire in fromWires)
                    {
                        wire.Src = toWires.First().Src;
                        wire.Src = toWires.First().Src;
                    }
                    foreach (var wire in toWires) mWires.Remove(wire);
                }
                foreach (var node in dummy) mNodes.Remove(node);

                NetUtils.Mnet mainNetwork = new NetUtils.Mnet();
                mainNetwork.Nodes = new List<NetUtils.Node>();
                mainNetwork.Wires = new List<NetUtils.Wire>();

                /*
                //Автодекомпозиция и выгрузка возможных нодов
                
                //Порты
                var inPorts = mNodes.Where(t => t.BaseType.Contains("INPort")).ToList();
                foreach (var node in inPorts)
                {
                    var wires = mWires.Where(t => t.Src == node.name).ToList();
                    for (int i = 0; i < node.wide; i++)
                    {
                        mainNetwork.Nodes.Add(new NetUtils.Node { NodeName = node.name + "[" + i + "]", NodeType = node.BaseType });
                        foreach (var wire in wires)
                        {
                            mainNetwork.Wires.Add(new NetUtils.Wire { SrcName = node.name + "[" + i + "]",SrcPort = "O0"
                                                                     ,DistName = wire.Dist , DistPort = wire.DistPort + i});
                        }
                    }
                    foreach (var wire in wires) mWires.Remove(wire);
                }
                foreach (var node in inPorts) mNodes.Remove(node);

                var outPorts = mNodes.Where(t => t.BaseType.Contains("OUTPort")).ToList();
                foreach (var node in outPorts)
                {
                    var wires = mWires.Where(t => t.Dist == node.name).ToList();
                    for (int i = 0; i < node.wide; i++)
                    {
                        mainNetwork.Nodes.Add(new NetUtils.Node { NodeName = node.name + "[" + i + "]", NodeType = node.BaseType });
                        foreach (var wire in wires)
                        {
                            mainNetwork.Wires.Add(new NetUtils.Wire
                            {
                                DistName = node.name + "[" + i + "]",
                                DistPort = "O0"
                             ,
                                SrcName = wire.Src,
                                SrcPort = wire.DistPort + i
                            });
                        }
                    }
                    foreach (var wire in wires) mWires.Remove(wire);
                }
                foreach (var node in inPorts) mNodes.Remove(node);
                //Константы
                //Регистры
                */
                //Выгрузка обьектов
                foreach (var node in mNodes)
                {
                    mainNetwork.Nodes.Add(new NetUtils.Node { NodeName = node.name, NodeType = node.Type });
                }
                //Выгрузка соеденений
                foreach (var wire in mWires)
                {
                    for (int i = 0; i < wire.Wide; i++)
                    {
                        mainNetwork.Wires.Add(new NetUtils.Wire { DistName = wire.Dist, DistPort = wire.DistPort + i, SrcName = wire.Src, SrcPort = wire.SrcPort + i });
                    }
                }
                //Автогенерация clk, reset
                var regs = mainNetwork.Nodes.Where(t => t.NodeType.Contains("TRIGD")).ToList();
                if (regs.Count > 0)
                {
                    mainNetwork.Nodes.Add(new NetUtils.Node { NodeName = "clk", NodeType = "INPort" });
                    mainNetwork.Nodes.Add(new NetUtils.Node { NodeName = "reset", NodeType = "INPort" });
                    foreach (var node in regs)
                    {
                        mainNetwork.Wires.Add(new NetUtils.Wire { DistName = node.NodeName, DistPort = "clk", SrcName = "clk", SrcPort = "O0" });
                        mainNetwork.Wires.Add(new NetUtils.Wire { DistName = node.NodeName, DistPort = "reset", SrcName = "reset", SrcPort = "O0" });
                    }
                }
                //Декомпозиция обьектов
                int throughId = 0;
                //Базовая декомпозиция регистров
                var registers = mainNetwork.Nodes.Where(t => t.NodeType.Contains("TRIGD")).ToList();
                foreach (var node in registers)
                {
                    int regnums = Convert.ToInt32(node.NodeType.Replace("TRIGD", ""));
                    var wclk = mainNetwork.Wires.First(t => t.DistName == node.NodeName && t.DistPort == "clk");
                    var wreset = mainNetwork.Wires.First(t => t.DistName == node.NodeName && t.DistPort == "reset");
                    mainNetwork.Wires.Remove(wclk);
                    mainNetwork.Wires.Remove(wreset);
                    var inwares = mainNetwork.Wires.Where(t => t.DistName == node.NodeName).ToList();
                    var outwares = mainNetwork.Wires.Where(t => t.SrcName == node.NodeName).ToList();
                    mainNetwork.Nodes.Remove(node);
                    List<NetUtils.Node> newnodes = new List<NetUtils.Node>();
                    //Новые ноды
                    for (int i = 0; i < regnums; i++)
                    {
                        var newNode = new NetUtils.Node { NodeName = node.NodeName + "_" + throughId, NodeType = "TRIG_D" };
                        newnodes.Add(newNode);
                        mainNetwork.Nodes.Add(newNode);
                        mainNetwork.Wires.Add(new NetUtils.Wire { SrcName = wclk.SrcName, SrcPort = wclk.SrcPort, DistName = newNode.NodeName, DistPort = "clk" });
                        mainNetwork.Wires.Add(new NetUtils.Wire { SrcName = wreset.SrcName, SrcPort = wreset.SrcPort, DistName = newNode.NodeName, DistPort = "sclr" });
                        throughId++;
                    }
                    //Ремапинг соеденений
                    foreach (var wire in inwares)
                    {
                        wire.DistName = newnodes[Convert.ToInt32(wire.DistPort.Replace("I", ""))].NodeName;
                        wire.DistPort = "datain";
                    }
                    foreach (var wire in outwares)
                    {
                        wire.SrcName = newnodes[Convert.ToInt32(wire.SrcPort.Replace("O", ""))].NodeName;
                        wire.SrcPort = "regout";
                    }

                }
                //Базовая декомпозиция констант
                var consts = mainNetwork.Nodes.Where(t => t.NodeType.Contains("CONST")).ToList();
                foreach (var node in consts)
                {
                    int len = Convert.ToInt32(node.NodeType.Split('_')[2]);
                    int[] value = hexConv(node.NodeType.Split('_')[1], len);
                    bool have1 = value.Contains(1);
                    bool have0 = value.Contains(0);
                    var vcc = new NetUtils.Node { NodeName = "VCC_" + throughId, NodeType = "VCC" };
                    var gnd = new NetUtils.Node { NodeName = "GND_" + throughId, NodeType = "GND" };
                    throughId++;
                    var wares = mainNetwork.Wires.Where(t => t.SrcName == node.NodeName).ToList();
                    foreach (var wire in wares)
                    {
                        if (value[Convert.ToInt32(wire.SrcPort.Replace("O", ""))] == 1)
                        {
                            wire.SrcPort = "O0";
                            wire.SrcName = vcc.NodeName;
                        }
                        else
                        {
                            wire.SrcPort = "O0";
                            wire.SrcName = gnd.NodeName;
                        }
                    }
                    if (have0) mainNetwork.Nodes.Add(gnd);
                    if (have1) mainNetwork.Nodes.Add(vcc);
                    mainNetwork.Nodes.Remove(node);
                }
                //Базовая декомпозиция Портов
                var inports = mainNetwork.Nodes.Where(t => t.NodeType.Contains("INPort") && t.NodeType.Length > ("INPort").Length).ToList();
                foreach (var node in inports)
                {
                    int len = Convert.ToInt32(node.NodeType.Replace("INPort", ""));
                    var outwares = mainNetwork.Wires.Where(t => t.SrcName == node.NodeName).ToList();
                    var newNodes = new List<NetUtils.Node>();
                    for (int i = 0; i < len; i++)
                    {
                        var newNode = new NetUtils.Node { NodeName = node.NodeName + "_" + i, NodeType = "INPort" };
                        newNodes.Add(newNode);
                        mainNetwork.Nodes.Add(newNode);
                        
                    }
                    //Ремапинг соеденений
                    foreach (var wire in outwares)
                    {
                        wire.SrcName = newNodes[Convert.ToInt32(wire.SrcPort.Replace("O", ""))].NodeName;
                        wire.SrcPort = "O0";
                    }
                    mainNetwork.Nodes.Remove(node);
                }
                var outports = mainNetwork.Nodes.Where(t => t.NodeType.Contains("OUTPort") && t.NodeType.Length > ("OUTPort").Length).ToList();
                foreach (var node in outports)
                {
                    int len = Convert.ToInt32(node.NodeType.Replace("OUTPort", ""));
                    var outwares = mainNetwork.Wires.Where(t => t.SrcName == node.NodeName).ToList();
                    var newNodes = new List<NetUtils.Node>();
                    for (int i = 0; i < len; i++)
                    {
                        var newNode = new NetUtils.Node { NodeName = node.NodeName + "_" + i, NodeType = "OUTPort" };
                        newNodes.Add(newNode);
                        mainNetwork.Nodes.Add(newNode);

                    }
                    //Ремапинг соеденений
                    foreach (var wire in outwares)
                    {
                        wire.DistName = newNodes[Convert.ToInt32(wire.DistPort.Replace("I", ""))].NodeName;
                        wire.DistPort = "I0";
                    }
                    mainNetwork.Nodes.Remove(node);
                }
                //Декомпозиция из файлов MNET

                //Пересоздание DUP
            }
        }

        private static int[] hexConv(string p, int len)
        {
            p = p.Replace("0", "0000");
            p = p.Replace("1", "Z").Replace("Z", "0001");
            p = p.Replace("2", "0010");
            p = p.Replace("3", "0011");
            p = p.Replace("4", "0100");
            p = p.Replace("5", "0101");
            p = p.Replace("6", "0110");
            p = p.Replace("7", "0111");
            p = p.Replace("8", "1000");
            p = p.Replace("9", "1001");
            p = p.Replace("A", "1010");
            p = p.Replace("B", "1011");
            p = p.Replace("C", "1100");
            p = p.Replace("D", "1101");
            p = p.Replace("E", "1110");
            p = p.Replace("F", "1111");

            var cp = p.Reverse().ToArray();
            int[] outi = new int[len];
            for (int i = 0; i < len; i++)
            {
                if (i < p.Length)
                {
                    outi[i] = Convert.ToInt32(cp[i].ToString());
                }
            }

            return outi;
        }
    }
}
