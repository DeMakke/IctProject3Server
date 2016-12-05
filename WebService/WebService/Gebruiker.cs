using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace WebService
{
    [DataContract]
    public class Gebruiker
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string name { get; set; }
    }
    
}