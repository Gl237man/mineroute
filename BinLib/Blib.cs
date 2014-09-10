using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BinLib
{
    public class Blib
    {
        private List<File> _files;

        public Blib()
        {
            _files = new List<File>();
        }

        private File GetFile(string name)
        {
            return _files.FirstOrDefault(t => t.FileName == name);
        }

        private void SaveFile(File file)
        {
            bool update = false;
            for (int i = 0; i < _files.Count; i++)
            {
                if (_files[i].FileName == file.FileName)
                {
                    _files[i] = file;
                    update = true;
                    break;
                }
            }
            if (!update)
            {
                _files.Add(file);
            }
        }

        private byte[] ReadAllBytes(string fileName)
        {
            byte[] b = GetFile(fileName).Content;
            return (byte[]) b.Clone();
        }

        private string ReadAllText(string fileName)
        {
            return Encoding.Default.GetString(ReadAllBytes(fileName));
        }

        public string[] ReadAllLines(string fileName)
        {
            string s = ReadAllText(fileName);
            return s.Replace("\n", "").Split(Convert.ToChar("\r"));
        }

/*
        public void WriteAllBytes(string fileName, byte[] bytes)
        {
            File f = new File { Content = (byte[])bytes.Clone(), FileName = fileName };
            SaveFile(f);
        }
*/

/*
        public void WriteAllText(string FileName, string Text)
        {
            byte[] bin = Encoding.Default.GetBytes(Text);
            File F = new File { FileName = FileName, Content = bin };
            SaveFile(F);
        }
*/

        public void WriteAllLines(string fileName, string[] lines)
        {
            var sb = new StringBuilder();
            sb.Append(lines[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                sb.Append("\r\n" + lines[i]);
            }
            string s = sb.ToString();
            byte[] bin = Encoding.Default.GetBytes(s);

            var f = new File {FileName = fileName, Content = bin};
            SaveFile(f);
        }


/*
        public Blib(string FileName)
        {
            if (System.IO.File.Exists(FileName))
                Load(FileName);
            else
            {
                _files = new List<File>();
                Save(FileName);
            }
        }
*/

        public void Save(string fileName)
        {
            FileStream f = System.IO.File.OpenWrite(fileName);
            var gz = new GZipStream(f, CompressionLevel.Optimal);
            gz.Write(BitConverter.GetBytes(_files.Count), 0, 4);
            foreach (File file in _files)
            {
                byte[] binName = Encoding.Default.GetBytes(file.FileName);
                gz.Write(BitConverter.GetBytes(binName.Length), 0, 4);
                gz.Write(binName, 0, binName.Length);
                gz.Write(BitConverter.GetBytes(file.Content.Length), 0, 4);
                gz.Write(file.Content, 0, file.Content.Length);
            }
            gz.Flush();
            gz.Close();
            f.Close();
        }

        public void Load(string fileName)
        {
            _files = new List<File>();
            FileStream f = System.IO.File.OpenRead(fileName);
            var gz = new GZipStream(f, CompressionMode.Decompress);

            int fileNum = GetInt(gz);
            for (int i = 0; i < fileNum; i++)
            {
                var fl = new File();
                int fnlen = GetInt(gz);
                var binName = new byte[fnlen];
                gz.Read(binName, 0, fnlen);
                fl.FileName = Encoding.Default.GetString(binName);
                int flen = GetInt(gz);
                var bin = new byte[flen];
                gz.Read(bin, 0, flen);
                fl.Content = bin;
                _files.Add(fl);
            }
            gz.Close();
            f.Close();
        }

        private static int GetInt(GZipStream gz)
        {
            var b = new byte[4];
            gz.Read(b, 0, 4);
            int i = BitConverter.ToInt32(b, 0);
            return i;
        }
    }
}