using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        SqlConnection connection = new SqlConnection("Data Source=JANLAPTOP;Initial Catalog=fileshare;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();

        public void GetData()
        {
            connection.Open();

            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM fileTable";

            SqlDataReader reader;
            reader = cmd.ExecuteReader();

            string name = "";
            Guid id = new Guid();
            while (reader.Read())
            {
                id = reader.GetGuid(0);
                name = reader.GetString(1);
            }
            reader.Close();
            connection.Close();
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
            String JSONData = reader.ReadToEnd();

            return JSONData;
        }
    }
}
