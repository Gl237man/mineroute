using System;
using System.Collections.Generic;
using System.Text;

namespace VerilogModulesGen
{
    static class Program
    {
        private static readonly List<string> Snames = new List<string>();
        private static readonly List<string> Cmd = new List<string>();
        static void Main()
        {
            Generate("MUL_", "*");
            Generate("DIV_", "/");
            Generate("ADD_", "+");
            Generate("SUB_", "-");
            Generate("AND_", "&");
            Generate("OR_", "|");
            Generate("XOR_", "^");

            GenerateL("EQ_", "==");
	        GenerateL("NOTEQ_", "!=");
	        GenerateL("MORE_", ">");
	        GenerateL("LESS_", "<");
	        GenerateL("LESSEQ_", "<=");
            GenerateL("MOREEQ_", ">=");

            System.IO.File.WriteAllLines("mdir.cmd",Cmd.ToArray());
            System.IO.File.WriteAllLines("list.txt", Snames.ToArray());
        }

        private static void GenerateL(string type, string formula)
        {

            for (int i = 1; i <= 32; i++)
            {
                var builder = new StringBuilder();

                string ostr = "";
                for (int q = 0; q < i; q++)
                {
                    ostr += "A" + q;
                    ostr += ",";
                }
                for (int q = 0; q < i; q++)
                {
                    ostr += "B" + q;
                    ostr += ",";
                }
                string ostr2 = ostr.Substring(0, ostr.Length - 1);
                ostr += "O";
                ostr += ",";
               
                ostr = ostr.Substring(0, ostr.Length - 1);
                builder.AppendLine(@"module " + type + i + "(" + ostr + ");");
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"input A" + q + ";");
                }
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"input B" + q + ";");
                }
                builder.AppendLine(@"output reg O;");
                builder.AppendLine(@"always @(" + ostr2 + ")");
                builder.AppendLine(@"begin");
                builder.AppendLine(@"   reg [" + (i - 1) + ":0]ma;");
                builder.AppendLine(@"   reg [" + (i - 1) + ":0]mb;");
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"   ma[" + q + "]=A" + q + ";");
                }
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"   mb[" + q + "]=B" + q + ";");
                }

                builder.AppendLine(@"   O = ma " + formula + " mb;");

                builder.AppendLine(@"end");

                builder.AppendLine(@"endmodule");

                string result = builder.ToString();

                System.IO.File.WriteAllText(type + i + ".v", result);
                Console.WriteLine(type + i + ".v");
                Snames.Add(type + i);
                Cmd.Add("mkdir " + type + i);
                Cmd.Add("copy " + type + i + @".v .\" + type + i);
            }
        }

        private static void Generate(string type , string formula)
        {

            for (int i = 1; i <= 32; i++)
            {
                var builder = new StringBuilder();

                string ostr = "";
                for (int q = 0; q < i; q++)
                {
                    ostr += "A" + q;
                    ostr += ",";
                }
                for (int q = 0; q < i; q++)
                {
                    ostr += "B" + q;
                    ostr += ",";
                }
                string ostr2 = ostr.Substring(0, ostr.Length - 1);
                for (int q = 0; q < i; q++)
                {
                    ostr += "O" + q;
                    ostr += ",";
                }
                ostr = ostr.Substring(0, ostr.Length - 1);
                builder.AppendLine(@"module " + type + i +"(" + ostr + ");");
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"input A" + q+";");
                }
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"input B" + q + ";");
                }
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"output reg O" + q + ";");
                }

                builder.AppendLine(@"always @(" + ostr2 + ")");
                builder.AppendLine(@"begin");
                builder.AppendLine(@"   reg [" + (i-1) + ":0]ma;");
                builder.AppendLine(@"   reg [" + (i-1) + ":0]mb;");
                builder.AppendLine(@"   reg [" + (i-1) + ":0]mo;");
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"   ma[" + q + "]=A" + q + ";");
                }
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"   mb[" + q + "]=B" + q + ";");
                }

                builder.AppendLine(@"   mo = ma " + formula + " mb;");
                for (int q = 0; q < i; q++)
                {
                    builder.AppendLine(@"   O" + q + "=mo[" + q + "];");
                }
                builder.AppendLine(@"end");

                builder.AppendLine(@"endmodule");

                string result = builder.ToString();

                System.IO.File.WriteAllText(type + i + ".v", result);
                Console.WriteLine(type + i + ".v");
                Snames.Add(type + i);
                Cmd.Add("mkdir " + type + i);
                Cmd.Add("copy " + type + i + @".v .\" + type + i);
            }
        }
    }
}
