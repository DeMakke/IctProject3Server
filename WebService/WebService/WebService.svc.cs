using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;

namespace WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WebService.svc or WebService.svc.cs at the Solution Explorer and start debugging.

    public class WebService : IWebService
    {
        public static List<SaveFilePackets> inp = new List<SaveFilePackets>();

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

        //17.	server functie die json succes string terugstuurt naar client (aanpassen)
        
        public string DeleteFile(Stream Data)
        {
            JsonCode json = new JsonCode();            
            Succes succes = new Succes();
            Data file = new Data();
            Database db = new Database();
            //geen idee of dit werkt
            StreamReader reader = new StreamReader(Data);
            string JSONData = reader.ReadToEnd();
            file = json.Deserialize<Data>(JSONData);

            succes.value = db.DeleteData(file);
            return json.JsonCoding(succes);
        }

        public string SaveFile(Stream data, string id,string max, string current)
        {
            StreamReader reader = new StreamReader(data);
            string JSONData = reader.ReadToEnd();

            
            Base64Code base64 = new Base64Code();


            inp[inp.FindIndex(x => x.id == Convert.ToInt16(id))].base64stringpackets.Add(JSONData);

            if (current == max)
            {

                inp[Convert.ToInt16(id)].FileData.base64 = "";

                foreach (string item in inp[Convert.ToInt16(id)].base64stringpackets)
                {
                    inp[Convert.ToInt16(id)].FileData.base64 += item;
                }
                Tuple<byte[],string> filebytes = base64.DeSerializeBase64(inp[Convert.ToInt16(id)].FileData);
                inp[Convert.ToInt16(id)].FileData.path = base64.saveFile(filebytes.Item1, inp[Convert.ToInt16(id)].FileData.name);

                Database database = new Database();

                bool status = database.AddRecord(inp[Convert.ToInt16(id)].FileData);
                Debug.WriteLine(status);
                JSONData = "OK" + inp[Convert.ToInt16(id)].base64stringpackets.Count;
                
                inp.RemoveAll(x => x.id == Convert.ToInt16(id));
                
            }
            
            return JSONData;
        }

        public string CheckDivisionOfData(Stream Data)
        {

            StreamReader reader = new StreamReader(Data);
            JsonCode json = new JsonCode();
            string JSONData = reader.ReadToEnd();


            Data receivedDataO = json.Deserialize<Data>(JSONData);

            int amountOfPackets = 0;


            if (receivedDataO.size > 50000)
            {
                amountOfPackets = receivedDataO.size / 50000;

                if ((receivedDataO.size % 50000) != 0)
                {
                    amountOfPackets += 1;
                }
            }

            int current;
            if (inp == null)
            {
                //error
            }
            else
            {
                current = inp.Count;

                inp.Add(new SaveFilePackets(current));
                inp[current].AOP = amountOfPackets;
                inp[current].FileData = receivedDataO;
                string uniqueID = Convert.ToString(current);
                return uniqueID + ":" + Convert.ToString(amountOfPackets);
            }


            return "error";
            
            
        }

        public string GetFileNames(Stream Data)
        {
            JsonCode json = new JsonCode();
            Database database = new Database();
            List<Item> itemlist = database.GetItems();
            //Debug.WriteLine("test");
            string reply = json.Serialize<List<Item>>(itemlist);

            List<Item> itemlistdebug = json.Deserialize<List<Item>>(reply);

            return reply;
            
        }
    }
}
