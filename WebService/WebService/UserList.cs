using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class UserList
    {
        [JsonProperty("users")]
        public List<Gebruiker> users { get; set; }
    }
}