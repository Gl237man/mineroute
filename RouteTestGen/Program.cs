using System.Text;

namespace RouteTestGen
{
    static class Program
    {
        static void Main()
        {
            string[] files = System.IO.Directory.GetFiles(@".\",@"*.MNET");

            var bilder = new StringBuilder();
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
