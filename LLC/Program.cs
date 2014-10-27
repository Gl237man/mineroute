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
                mNodes.AddRange(parser.tab.ConstObjs.Select(t => new MultiNode { BaseType = "CONST_" + t.val, name = t.name, wide = t.wide }));
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

            }
        }
    }
}
