using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WebService
{
    public class Database
    {
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.DBconnectionFrederik); // maak je eigen connectionstring en verander de naam
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmd2 = new SqlCommand();


        //adding a user to the db for testing purposes
        public bool SetUser() //call to this function is found in the WebServcice.svc.cs inside the ValidateUser function
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[Users] ([UserID],[UserName],[Password]) VALUES (@userid,@username, @password)";
                Guid id = new Guid();
                cmd.Parameters.AddWithValue("@userid", id);
                cmd.Parameters.AddWithValue("@username", "Frederik");
                cmd.Parameters.AddWithValue("@password", "123");

                int result = cmd.ExecuteNonQuery();
                if (result >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();

            }
        }


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

        public bool DeleteData(Item file)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();               
                cmd.CommandText = "DELETE FROM fileTable WHERE fileID = @fileID"; 
                cmd.Parameters.AddWithValue("@fileID", file.id);               

                int result = cmd.ExecuteNonQuery();
                if (result >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

        }

        public bool AddRecord(Data fileData)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[fileTable] ([fileName],[filePath]) OUTPUT inserted.fileID VALUES (@filename, @filepath)";

                cmd.Parameters.AddWithValue("@filename", fileData.name);
                cmd.Parameters.AddWithValue("@filepath", fileData.path);


                Guid uniqueid = (Guid)cmd.ExecuteScalar();


                cmd2 = connection.CreateCommand();
                cmd2.CommandText = "INSERT INTO [dbo].[files](fileID, ActualFile)SELECT '@uniqueid', BulkColumn FROM OPENROWSET(BULK '@filepath', SINGLE_BLOB) as f;";

                cmd2.Parameters.AddWithValue("@uniqueid", uniqueid);
                cmd2.Parameters.AddWithValue("@filepath", fileData.path);

                cmd2.ExecuteNonQuery();
                return true;

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

        public List<Item> GetItems()
        {
            List<Item> itemlist = new List<Item>();

            connection.Open();

            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT [fileID],[fileName] FROM [dbo].[fileTable]";

            SqlDataReader reader;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Item currentItem = new Item();
                currentItem.id = reader.GetGuid(0);
                currentItem.name = reader.GetString(1);

                itemlist.Add(currentItem);
            }
            reader.Close();
            connection.Close();

            return itemlist;
        }

        public void getSelectedItem(Guid uniqueid)
        {

            //return file;
        }

        public User ValidateUser(User user)
        {
            connection.Open();

            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT [Password] FROM [dbo].[Users] WHERE UserName = @username";
            cmd.Parameters.AddWithValue("@username", user.name);

            string password = "";
            Md5Class hashing = new Md5Class();
            bool checkPassword = false;
            Random random = new Random();
            SqlDataReader reader;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                password = reader.GetString(0);
            }
            reader.Close();
            connection.Close();
            using (MD5 md5Hash = MD5.Create())
            {
                checkPassword = hashing.VerifyMd5Hash(md5Hash, password, user.hash);
            }
            if (checkPassword)
            {
                user.token = random.Next(1, 8999);
            }
            else
            {
                user.token = 9999;
            }

            return user;
        }





    }
}