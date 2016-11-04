using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public class SaveFilePackets
    {
        public int AOP;
        public int id;
        public List<string> base64stringpackets;
        public Data FileData;
        public SaveFilePackets(int id)
        {
            this.id = id;
            base64stringpackets = new List<string>();
        }
    }
}