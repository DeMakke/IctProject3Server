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
        public string name { get; set; }
        [JsonProperty("path")]
        public string path { get; set; }
        [JsonProperty("base64")]
        public string base64 { get; set; }
        [JsonProperty("size")]
        public int size { get; set; }
    }
}