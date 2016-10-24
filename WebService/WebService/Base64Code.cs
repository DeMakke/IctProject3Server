using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace WebService
{
    public class Base64Code
    {
        public Tuple<byte[], string> DeSerializeBase64(Data o)
        {
            string naam = o.name;
            //o.base64 = o.base64.Remove('"');
            Debug.WriteLine(o.base64);
            byte[] bitarray = Convert.FromBase64String(o.base64);
            

            return Tuple.Create(bitarray, naam);
        }

        public void saveFile(byte[] tempBytes, string fileName)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + fileName;
            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string path = Path.GetDirectoryName(filePath);
            string newFullPath = filePath;
            int count = 1;
            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", "IAmOnServer" + fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);

            }
            Debug.WriteLine(newFullPath);

            File.WriteAllBytes(newFullPath, tempBytes);
        }
    }
}