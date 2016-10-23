using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class FileList
    {
        [JsonProperty("files")]
        public List<string> files { get; set; }
    }
}