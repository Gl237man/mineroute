using System.Text;

namespace MNETVisualiser
{
    static class Program
    {
        static void Main(string[] args)
        {
            string fname = "test";
            if (args.Length > 0) fname = args[0];

            NetUtils.Mnet net = new NetUtils.Mnet();
            net.ReadMnetFile(fname + ".MNET");

            StringBuilder sb = new StringBuilder();

            foreach (var node in net.Nodes)
            {
                sb.AppendLine($"graph.addNode('{node.NodeName}', '{node.NodeType}');");
            }

            foreach (var wire in net.Wires)
            {
                sb.AppendLine($"graph.addLink('{wire.SrcName}', '{wire.DistName}');");
            }

            string mainFile = System.IO.File.ReadAllText("Main_.js");
            string repalace = sb.ToString();

            mainFile = mainFile.Replace(@"/***REPLACE***/", repalace);

            System.IO.File.WriteAllText("main.js", mainFile);

        }
    }
}
