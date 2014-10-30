using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteTestGen
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = System.IO.Directory.GetFiles(@".\",@"*.MNET");

            StringBuilder bilder = new StringBuilder();
            foreach (var file in files)
            {
                var filen= file.Replace(@"./", "").Replace(".MNET", "");
                bilder.AppendLine("MnetLutDecomposite "+ filen);
                bilder.AppendLine("MnetLutOptimise "+ filen + "_D");
                bilder.AppendLine("Mnetsynt3 " + filen + "_D_O");
                bilder.AppendLine("Binhl2JsWE " + filen + "_D_O");
            }
            System.IO.File.WriteAllText("runall.cmd", bilder.ToString());
        }
    }
}
