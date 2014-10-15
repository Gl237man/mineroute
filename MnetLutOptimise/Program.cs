using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetUtils;

namespace MnetLutOptimise
{
    class Program
    {
        private static Mnet _mainNet;
        private static int lineCount = 0;

        static void Main(string[] args)
        {
            
            string file = args.Length == 0 ? "lut_0006_D" : args[0];
            _mainNet = new Mnet();
            _mainNet.ReadMnetFile(file + @".MNET");
            Console.WriteLine("Всего Соеденений " + _mainNet.Wires.Count);
            Console.WriteLine();
            Optimise("AND");
            Console.WriteLine();
            Optimise("OR");
            Console.WriteLine();
            //NAND оптимизация

            TransNOT();
            
            string ElementName = "AND";
            
            Console.WriteLine("Оптимизация NOT");
            Console.WriteLine("До Оптимизации " + _mainNet.Nodes.Where(t => t.NodeType.Contains("NOT")).Count());
            List<Node> allAnd = _mainNet.Nodes.Where(t => t.NodeType.Contains("AND")).ToList();
            var allNot = _mainNet.Nodes.Where(t => t.NodeType == "NOT");
            List<Wire> allNotWire = new List<Wire>();
            foreach (Node node in allNot)
            {
                allNotWire.AddRange(_mainNet.Wires.Where(t => t.SrcName == node.NodeName));
            }
            foreach (Node node in allAnd)
            {
                List<Wire> wireNotToAnd = allNotWire.Where(t => t.DistName == node.NodeName).ToList();
                List<Wire> allWiresToAnd = _mainNet.Wires.Where(t => t.DistName == node.NodeName).ToList();
                List<Wire> allWiresToNot = new List<Wire>();
                List<Node> allNotTodel = new List<Node>();
                int[] bits = new int[allWiresToAnd.Count];
                //Заполнение битов
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    if (wireNotToAnd.Contains(allWiresToAnd[i]))
                    {
                        bits[i] = 1;
                    }
                }
                foreach(Wire wire in wireNotToAnd)
                {
                    allNotTodel.Add(_mainNet.Nodes.FirstOrDefault(t => t.NodeName == wire.SrcName));
                    allWiresToNot.Add(_mainNet.Wires.FirstOrDefault(t => t.DistName == wire.SrcName));
                }

                //Получения числа
                int val = 0;
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    val = val | bits[i];
                    val = val << 1;
                }
                val = val >> 1;
                //Преобразование and
                node.NodeType = "NANDT" + bits.Length + "_" + val.ToString("X2");
                //Перенос соеденений
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    if (bits[i] == 1)
                    {
                        Wire twire = _mainNet.Wires.FirstOrDefault(t => t.DistName == allWiresToAnd[i].SrcName);
                        
                        allWiresToAnd[i].SrcName = twire.SrcName;
                        allWiresToAnd[i].SrcPort = twire.SrcPort;
                    }
                }
                //Удаление ненужных соеденений и нодов
                foreach (var t in allWiresToNot) _mainNet.Wires.Remove(t);
                foreach (var t in allNotTodel) _mainNet.Nodes.Remove(t);
                
            }

            Console.WriteLine("После Оптимизации " + _mainNet.Nodes.Where(t => t.NodeType.Contains("NOT")).Count());

            Console.WriteLine("Соеденений после оптимизации " + _mainNet.Wires.Count);
            System.IO.File.WriteAllText(file + @"_O.MNET" ,_mainNet.GetSting());
        }

        private static void TransNOT()
        {
            var allDup = _mainNet.Nodes.Where(t => t.NodeType.Contains("DUP"));
            var allNot = _mainNet.Nodes.Where(t => t.NodeType == "NOT");
            List<Wire> allNotWire = new List<Wire>();
            List<Wire> allNotToDup = new List<Wire>();
            foreach (Node node in allNot)
            {
                allNotWire.AddRange(_mainNet.Wires.Where(t => t.SrcName == node.NodeName));
            }
            foreach (Node node in allDup)
            {
                allNotToDup.AddRange(allNotWire.Where(t => t.DistName == node.NodeName));
            }
            //Перенос нота после ДУП
            foreach (Wire wire in allNotToDup)
            {
                var wireToNOT = _mainNet.Wires.FirstOrDefault(q => q.DistName == allNot.FirstOrDefault(t => t.NodeName == wire.SrcName).NodeName);
                //Удалить NOT
                _mainNet.Nodes.Remove(allNot.FirstOrDefault(t => t.NodeName == wireToNOT.DistName));
                //Перенисти соеденение на DUP
                wireToNOT.DistName = wire.DistName;
                //Создать NOTы c DUP
                var dup = _mainNet.Nodes.FirstOrDefault(t => t.NodeName == wireToNOT.DistName);
                List<Wire> WiresFromDup = _mainNet.Wires.Where(t => t.SrcName == dup.NodeName).ToList();
                List<Node> newNots = new List<Node>();
                List<Wire> newWires = new List<Wire>();
                for (int i = 0; i < WiresFromDup.Count; i++)
                {
                    newNots.Add(new Node { NodeName = "OPTNOT_" + lineCount, NodeType = "NOT" });
                    lineCount++;
                }
                //Создать новые соеденения с NOT
                for (int i = 0; i < WiresFromDup.Count; i++)
                {
                    newWires.Add(new Wire { SrcName = newNots[i].NodeName, SrcPort = "O0", 
                                            DistName = WiresFromDup[i].DistName, DistPort = WiresFromDup[i].DistPort });
                }
                //Перенести старые соеденения с DUP
                for (int i = 0; i < WiresFromDup.Count; i++)
                {
                    WiresFromDup[i].DistName = newNots[i].NodeName;
                    WiresFromDup[i].DistPort = "I0";
                }
                //добавить новые соеденения и ноды
                _mainNet.Wires.AddRange(newWires);
                _mainNet.Nodes.AddRange(newNots);
                //Удалить старое соеденение
                _mainNet.Wires.Remove(wire);
            }


            //List<Wire> notToDup = allNot.Where()
        }

        private static void Optimise(string ElementName)
        {
            Console.WriteLine("Оптимизация " + ElementName);
            Console.WriteLine("До Оптимизации " + _mainNet.Nodes.Where(t => t.NodeType.Contains(ElementName)).Count());
            bool OpNeed = true;
            while (OpNeed)
            {
                
                //Оптимизация AND
                var allAnd = _mainNet.Nodes.Where(t => t.NodeType.Contains(ElementName));
                //Поиск 2х связаных AND
                Wire AndConnection = _mainNet.Wires.FirstOrDefault(t =>
                    allAnd.FirstOrDefault(al => al.NodeName == t.DistName) != null &&
                    allAnd.FirstOrDefault(al => al.NodeName == t.SrcName) != null);
                if (AndConnection != null)
                {
                    Node AndBase = allAnd.FirstOrDefault(al => al.NodeName == AndConnection.DistName);
                    Node AndConnected = allAnd.FirstOrDefault(al => al.NodeName == AndConnection.SrcName);
                    var ConnectedWires = _mainNet.Wires.Where(t => (t.DistName == AndBase.NodeName
                                                                   || t.DistName == AndConnected.NodeName) && t != AndConnection);
                    //Создание нового нода
                    Node NewAnd = new Node { NodeName = "OPT" + ElementName + lineCount, NodeType = ElementName + ConnectedWires.Count() };
                    lineCount++;
                    //Перенос соеденений
                    List<Wire> ConnectionWireList = ConnectedWires.ToList();
                    for (int i = 0; i < ConnectionWireList.Count; i++)
                    {
                        ConnectionWireList[i].DistName = NewAnd.NodeName;
                        ConnectionWireList[i].DistPort = "I" + i;
                    }
                    //Перенос исходящего соеденения
                    Wire OutConn = _mainNet.Wires.FirstOrDefault(t => t.SrcName == AndBase.NodeName);
                    OutConn.SrcName = NewAnd.NodeName;
                    //Удалене старых нодов
                    _mainNet.Nodes.Remove(AndBase);
                    _mainNet.Nodes.Remove(AndConnected);
                    //Удаление лишнего соеденения
                    _mainNet.Wires.Remove(AndConnection);
                    //Добовление нового нода
                    _mainNet.Nodes.Add(NewAnd);
                    

                }
                else
                {
                    OpNeed = false;
                }
            }
            Console.WriteLine("После Оптимизации " + _mainNet.Nodes.Where(t => t.NodeType.Contains(ElementName)).Count());
        }
    }
}
