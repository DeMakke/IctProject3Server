﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IctProject3_Server
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

        //

    }
}
