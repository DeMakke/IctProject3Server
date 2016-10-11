using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WebService.svc or WebService.svc.cs at the Solution Explorer and start debugging.

    public class WebService : IWebService
    {
        public string Default(Stream data)
        {
            StreamReader reader = new StreamReader(data);
            String JSONData = reader.ReadToEnd();

            return "Positive response";
        }

        public string GetFile(Stream Data)
        {
            string Json;
            Data data = new Data();
            JsonCode json = new JsonCode();
            data.base64 = "amEgbmVlIGlrIGdhIG1pam4gcGFzIG5pZSBkb29yc3R1cmVu";
            data.name = "tsserver.txt";
            Json = json.JsonCoding(data);
            return Json;
        }

        public string SaveFile(Stream data)
        {
            StreamReader reader = new StreamReader(data);
            String JSONData = reader.ReadToEnd();
            //hier komt eerst de rest van de functies en dan returned ge een boolean
            return "";
        }
    }
}
