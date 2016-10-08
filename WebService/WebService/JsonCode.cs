using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class JsonCode
    {
        Data data = new Data();

        public Data JsonDeCoding(string json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            return data;
        }

        public string JsonCoding(Data data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }
    }
}