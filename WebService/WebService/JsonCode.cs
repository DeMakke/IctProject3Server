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

    }
}