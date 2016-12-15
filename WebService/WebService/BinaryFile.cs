using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class BinaryFile
    {
        [JsonProperty("binary")]
        public byte[] binary { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
    }
}