using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebService
{
    public class Database
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

        public bool DeleteData(Data file)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE " + file.name + " FROM fileTable";

                if (cmd.ExecuteNonQuery() == 1)
                {
                    // querry gedaan
                    return true;
                }
                else
                {
                    //query niet gedaan
                    return false;
                }
            }
            catch (Exception ex)
            {
                //geeft error aan foutmelding object?
                return false;
            }
            finally
            {
                connection.Close();
            }

        }
    }
}