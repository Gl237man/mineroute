using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul
{
    public class Log
    {
        string LogFileName;
        public Log (string fileName)
        {
            LogFileName = fileName;
        }
        public void Write(string value)
        {
            if (!System.IO.File.Exists(LogFileName)) System.IO.File.WriteAllText(LogFileName, "");
            string[] s = System.IO.File.ReadAllLines(LogFileName);
            var outstr = s.ToList();
            outstr.Add(value);
            System.IO.File.WriteAllLines(LogFileName, outstr.ToArray());
        }
    }
}
