using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class Item
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
    }
}