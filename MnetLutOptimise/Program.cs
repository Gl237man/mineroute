using System;
using System.Collections.Generic;
using System.Linq;
using NetUtils;

namespace MnetLutOptimise
{
    static class Program
    {
        private static Mnet _mainNet;
        private static int _lineCount;

        static void Main(string[] args)
        {
            
            //string file = args.Length == 0 ? "lut_0100_D" : args[0];
            string file = args.Length == 0 ? "test_D" : args[0];
            _mainNet = new Mnet();
            _mainNet.ReadMnetFile(file + @".MNET");

    

            var dupList = _mainNet.Nodes.Where(t => t.NodeType.Contains("DUP")).ToList();

            //Обьеденение DUP
            List<Node> list = dupList;
            var dupdupWires = _mainNet.Wires.Where(t => list.FirstOrDefault(a => a.NodeName == t.SrcName) != null &&
                                                           list.FirstOrDefault(a => a.NodeName == t.DistName) != null);
            while (dupdupWires.Any())
            {
                var wire = dupdupWires.First();
                var firstDup = dupList.First(a => a.NodeName == wire.SrcName);
                var secondDup = dupList.First(a => a.NodeName == wire.DistName);
                var fromSecondWires = _mainNet.Wires.Where(t => t.SrcName == secondDup.NodeName).ToList();
                foreach (var w in fromSecondWires)
                {
                    w.SrcName = firstDup.NodeName;
                }
                _mainNet.Wires.Remove(wire);
                _mainNet.Nodes.Remove(secondDup);
            }
            dupList = _mainNet.Nodes.Where(t => t.NodeType.Contains("DUP")).ToList();
            //Обновление Типов и имен выходных портов
            foreach (var node in dupList)
            {
                var wlist = _mainNet.Wires.Where(t => t.SrcName == node.NodeName).ToList();
                for (int i = 0; i < wlist.Count; i++)
                {
                    wlist[i].SrcPort = "O" + i;
                }
                node.NodeType = "DUP" + wlist.Count;
            }



            Console.WriteLine("Всего Соеденений " + _mainNet.Wires.Count);
            Console.WriteLine();
            Optimise("AND");
            Console.WriteLine();
            Optimise("OR");
            Console.WriteLine();
            //NAND оптимизация

            TransNot();
            
            //string ElementName = "AND";
            
            Console.WriteLine("Оптимизация NOT");
            Console.WriteLine("До Оптимизации " + _mainNet.Nodes.Count(t => t.NodeType.Contains("NOT")));
            List<Node> allAnd = _mainNet.Nodes.Where(t => t.NodeType.Contains("AND")).ToList();
            var allNot = _mainNet.Nodes.Where(t => t.NodeType == "NOT");
            var allNotWire = new List<Wire>();
            foreach (Node node in allNot)
            {
                allNotWire.AddRange(_mainNet.Wires.Where(t => t.SrcName == node.NodeName));
            }
            foreach (Node node in allAnd)
            {
                var wireNotToAnd = allNotWire.Where(t => t.DistName == node.NodeName).ToList();
                var allWiresToAnd = _mainNet.Wires.Where(t => t.DistName == node.NodeName).ToList();
                var allWiresToNot = new List<Wire>();
                var allNotTodel = new List<Node>();
                var bits = new int[allWiresToAnd.Count];
                //Заполнение битов
                /*
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    if (wireNotToAnd.Contains(allWiresToAnd[i]))
                    {
                        bits[i] = 1;
                    }
                }
                 */
                foreach(Wire wire in wireNotToAnd)
                {
                    allNotTodel.Add(_mainNet.Nodes.FirstOrDefault(t => t.NodeName == wire.SrcName));
                    allWiresToNot.Add(_mainNet.Wires.FirstOrDefault(t => t.DistName == wire.SrcName));
                }

                
                //Перенос соеденений
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    //if (bits[i] == 1)
                    //{
                    Wire twire = _mainNet.Wires.FirstOrDefault(t => t.DistName == allWiresToAnd[i].SrcName);
                    if (twire != null)
                    {
                        if (_mainNet.Nodes.First(t => t.NodeName == twire.DistName).NodeType == "NOT")
                        {
                            allWiresToAnd[i].SrcName = twire.SrcName;
                            allWiresToAnd[i].SrcPort = twire.SrcPort;
                            bits[i] = 1;
                        }
                    }
                    //}
                    allWiresToAnd[i].DistPort = "I" + i;
                }

                //Получения числа
                int val = 0;
                for (int i = 0; i < allWiresToAnd.Count; i++)
                {
                    val = val | bits[i];
                    val = val << 1;
                }
                val = val >> 1;

                //val = RevertVal(val, allWiresToAnd.Count);

                //Преобразование and
                node.NodeType = "NANDT" + bits.Length + "_" + val.ToString("X2");

                //Удаление ненужных соеденений и нодов
                foreach (var t in allWiresToNot) _mainNet.Wires.Remove(t);
                foreach (var t in allNotTodel) _mainNet.Nodes.Remove(t);
                
            }

            Console.WriteLine("После Оптимизации " + _mainNet.Nodes.Count(t => t.NodeType.Contains("NOT")));

            Console.WriteLine("Соеденений после оптимизации " + _mainNet.Wires.Count);
            System.IO.File.WriteAllText(file + @"_O.MNET" ,_mainNet.GetSting());
        }

/*
        private static int RevertVal(int val, int p)
        {
            int[] bits = new int[p];
            for (int i = 0; i < p; i++)
            {
                bits[i] = val & 1;
                val = val >> 1;
            }

            val = 0;
            for (int i = 0; i < p; i++)
            {
                val = val | bits[i];
                val = val << 1;
            }
            val = val >> 1;
            return val;
        }
*/

        private static void TransNot()
        {
            var allDup = _mainNet.Nodes.Where(t => t.NodeType.Contains("DUP"));
            var allNot = _mainNet.Nodes.Where(t => t.NodeType == "NOT").ToList();
            var allNotWire = new List<Wire>();
            var allNotToDup = new List<Wire>();
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
                var wireToNot = _mainNet.Wires.First(q => q.DistName == allNot.First(t => t.NodeName == wire.SrcName).NodeName);
                //Удалить NOT
                _mainNet.Nodes.Remove(allNot.FirstOrDefault(t => t.NodeName == wireToNot.DistName));
                //Перенисти соеденение на DUP
                wireToNot.DistName = wire.DistName;
                //Создать NOTы c DUP
                var dup = _mainNet.Nodes.First(t => t.NodeName == wireToNot.DistName);
                List<Wire> wiresFromDup = _mainNet.Wires.Where(t => t.SrcName == dup.NodeName).ToList();
                var newNots = new List<Node>();
                for (int i = 0; i < wiresFromDup.Count; i++)
                {
                    newNots.Add(new Node { NodeName = "OPTNOT_" + _lineCount, NodeType = "NOT" });
                    _lineCount++;
                }
                //Создать новые соеденения с NOT
                var newWires = wiresFromDup.Select((t, i) => new Wire
                {
                    SrcName = newNots[i].NodeName, SrcPort = "O0", DistName = t.DistName, DistPort = t.DistPort
                }).ToList();
                //Перенести старые соеденения с DUP
                for (int i = 0; i < wiresFromDup.Count; i++)
                {
                    wiresFromDup[i].DistName = newNots[i].NodeName;
                    wiresFromDup[i].DistPort = "I0";
                }
                //добавить новые соеденения и ноды
                _mainNet.Wires.AddRange(newWires);
                _mainNet.Nodes.AddRange(newNots);
                //Удалить старое соеденение
                _mainNet.Wires.Remove(wire);
            }


            //List<Wire> notToDup = allNot.Where()
        }

        private static void Optimise(string elementName)
        {
            Console.WriteLine("Оптимизация {0}", elementName);
            Console.WriteLine("До Оптимизации {0}", _mainNet.Nodes.Count(t => t.NodeType.Contains(elementName)));
            bool opNeed = true;
            while (opNeed)
            {
                
                //Оптимизация AND
                var allAnd = _mainNet.Nodes.Where(t => t.NodeType.Contains(elementName)).ToList();
                //Поиск 2х связаных AND
                Wire andConnection = _mainNet.Wires.FirstOrDefault(t =>
                    allAnd.FirstOrDefault(al => al.NodeName == t.DistName) != null &&
                    allAnd.FirstOrDefault(al => al.NodeName == t.SrcName) != null);
                if (andConnection != null)
                {
                    Node andBase = allAnd.First(al => al.NodeName == andConnection.DistName);
                    Node andConnected = allAnd.First(al => al.NodeName == andConnection.SrcName);
                    var connectedWires = _mainNet.Wires.Where(t => (t.DistName == andBase.NodeName
                                                                   || t.DistName == andConnected.NodeName) && t != andConnection).ToArray();
                    //Создание нового нода
                    var newAnd = new Node { NodeName = "OPT" + elementName + _lineCount, NodeType = elementName + connectedWires.Count() };
                    _lineCount++;
                    //Перенос соеденений
                    List<Wire> connectionWireList = connectedWires.ToList();
                    for (int i = 0; i < connectionWireList.Count; i++)
                    {
                        connectionWireList[i].DistName = newAnd.NodeName;
                        connectionWireList[i].DistPort = "I" + i;
                    }
                    //Перенос исходящего соеденения
                    Wire outConn = _mainNet.Wires.First(t => t.SrcName == andBase.NodeName);
                    outConn.SrcName = newAnd.NodeName;
                    //Удалене старых нодов
                    _mainNet.Nodes.Remove(andBase);
                    _mainNet.Nodes.Remove(andConnected);
                    //Удаление лишнего соеденения
                    _mainNet.Wires.Remove(andConnection);
                    //Добовление нового нода
                    _mainNet.Nodes.Add(newAnd);
                    

                }
                else
                {
                    opNeed = false;
                }
            }
            Console.WriteLine("После Оптимизации {0}", _mainNet.Nodes.Count(t => t.NodeType.Contains(elementName)));
        }
    }
}
