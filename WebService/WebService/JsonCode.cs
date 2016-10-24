using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace WebService
{
    public class JsonCode
    {
        Data data = new Data();

        public Data JsonDeCoding(String json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            return data;
        }

        public String JsonCoding(Data data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        public Succes JsonDeCodingSucces(String json)
        {
            Succes succes = JsonConvert.DeserializeObject<Succes>(json);
            return succes;
        }

        public String JsonCoding(Succes succes)
        {
            string json = JsonConvert.SerializeObject(succes);
            return json;
        }

        public FileList JsonDeCodingFileList(String json)
        {
            FileList fileList = JsonConvert.DeserializeObject<FileList>(json);
            return fileList;
        }

        public String JsonCoding(FileList fileList)
        {
            string json = JsonConvert.SerializeObject(fileList);
            return json;
        }

        public string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        public string cropString(string input)
        {
            string output;
            input = input.Remove(0, 1);
            output = input.Remove(input.Length - 1);
            output = output.Replace("\\\"", "\"");
            output = "{\"" + output + "\"}";
            return output;
        }


    }
}