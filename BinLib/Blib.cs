using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinLib
{
    public class Blib
    {
        List<File> Files;

        public Blib()
        {
            Files = new List<File>();
        }

        private File GetFile(string name)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].FileName == name)
                {
                    return Files[i];
                }
            }
            return null;
        }

        private void SaveFile(File file)
        {
            bool update = false;
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].FileName == file.FileName)
                {
                    Files[i] = file;
                    update = true;
                    break;
                }
            }
            if (!update)
            {
                Files.Add(file);
            }
            
        }
        public byte[] ReadAllBytes(string FileName)
        {
            byte[] b = GetFile(FileName).Content;
            return (byte[])b.Clone();
        }

        public string ReadAllText(string FileName)
        {
            string S = Encoding.Default.GetString(ReadAllBytes(FileName));
            return S;
        }

        public string[] ReadAllLines(string FileName)
        {
            string S = ReadAllText(FileName);
            return S.Replace("\n","").Split(Convert.ToChar("\r"));
        }

        public void WriteAllBytes(string FileName, byte[] bytes)
        {
            File F = new File { Content = (byte[])bytes.Clone(), FileName = FileName };
            SaveFile(F);
        }

        public void WriteAllText(string FileName, string Text)
        {
            byte[] bin = Encoding.Default.GetBytes(Text);
            File F = new File { FileName = FileName, Content = bin };
            SaveFile(F);
        }

        public void WriteAllLines(string FileName, string[] lines)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(lines[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                SB.Append("\r\n" + lines[i]);
            }
            string S = SB.ToString();
            byte[] bin = Encoding.Default.GetBytes(S);

            File F = new File {FileName = FileName , Content = bin};
            SaveFile(F);
        }


        public Blib(string FileName)
        {
            if (System.IO.File.Exists(FileName))
                Load(FileName);
            else
            {
                Files = new List<File>();
                Save(FileName);
            }
        }

        public void Save(string FileName)
        {
            System.IO.FileStream F = System.IO.File.OpenWrite(FileName);
            System.IO.Compression.GZipStream GZ = new System.IO.Compression.GZipStream(F, System.IO.Compression.CompressionLevel.Optimal);
            GZ.Write(BitConverter.GetBytes(Files.Count), 0, 4);
            for (int i = 0; i < Files.Count; i++)
            {
                byte[] binName = Encoding.Default.GetBytes(Files[i].FileName);
                GZ.Write(BitConverter.GetBytes(binName.Length), 0, 4);
                GZ.Write(binName, 0, binName.Length);
                GZ.Write(BitConverter.GetBytes(Files[i].Content.Length), 0, 4);
                GZ.Write(Files[i].Content, 0, Files[i].Content.Length);
            }
            GZ.Flush();
            GZ.Close();
            F.Close();
        }

        public void Load(string FileName)
        {
            Files = new List<File>();
            System.IO.FileStream F = System.IO.File.OpenRead(FileName);
            System.IO.Compression.GZipStream GZ = new System.IO.Compression.GZipStream(F, System.IO.Compression.CompressionMode.Decompress);

            int FileNum = GetInt(GZ);
            for (int i = 0; i < FileNum; i++)
            {
                File FL = new File();
                int fnlen = GetInt(GZ);
                byte[] BinName = new byte[fnlen];
                GZ.Read(BinName, 0, fnlen);
                FL.FileName = Encoding.Default.GetString(BinName);
                int Flen = GetInt(GZ);
                byte[] Bin = new byte[Flen];
                GZ.Read(Bin, 0, Flen);
                FL.Content = Bin;
                Files.Add(FL);
            }
            GZ.Close();
            F.Close();
        }

        private static int GetInt(System.IO.Compression.GZipStream GZ)
        {
            int i;
            byte[] b = new byte[4];
            GZ.Read(b, 0, 4);
            i = BitConverter.ToInt32(b, 0);
            return i;
        }
    }
}
