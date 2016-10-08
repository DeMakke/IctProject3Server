using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class Data
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("name")]
        public int name { get; set; }
        [JsonProperty("base64")]
        public int base64 { get; set; }
    }
}