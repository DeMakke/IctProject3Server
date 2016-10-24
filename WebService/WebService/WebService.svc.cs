using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
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
        
        public void GetData()
        {
            Database databseInterface = new Database();

            databseInterface.GetData();
        }

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
            string JSONData = reader.ReadToEnd();
            JsonCode json = new JsonCode();
            Base64Code base64 = new Base64Code();

            //JSONData = json.cropString(JSONData);
            string CleanedJSON = json.cropString(JSONData);

            Data receivedDataO = json.JsonDeCoding(CleanedJSON);

            Tuple<byte[], string> file = base64.DeSerializeBase64(receivedDataO); 
            base64.saveFile(file.Item1, file.Item2);
            return JSONData;
        }
    }
}
