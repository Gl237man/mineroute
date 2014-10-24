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

                //Проверка размерностей

                //Определение финалного типа

            }
        }
    }
}
