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
            Console.WriteLine("Соеденений после оптимизации " + _mainNet.Wires.Count);
            System.IO.File.WriteAllText(file + @"_O.MNET" ,_mainNet.GetSting());
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
