using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class User
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("hash")]
        public string hash { get; set; }

        [JsonProperty("token")]
        public int token { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }
    }
}