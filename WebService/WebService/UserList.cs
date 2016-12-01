using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebService
{
    [DataContract]
    public class UserList
    {
        [DataMember]
        public List<Gebruiker> users { get; set; }
    }
}