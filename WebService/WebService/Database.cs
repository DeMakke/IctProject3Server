using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WebService
{
    public class Database
    {
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.DBconnectionDries); // maak je eigen connectionstring en verander de naam
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
                cmd.Parameters.AddWithValue("@username", "Dries");
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


        public List<Item> GetData(string Userid)
        {
            List<Item> itemlist = new List<Item>();
            connection.Open();

            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT fileshare.dbo.fileTable.fileID, fileshare.dbo.fileTable.fileName FROM fileTable INNER JOIN fileshare.dbo.usersPerFile ON fileshare.dbo.fileTable.fileID = fileshare.dbo.usersPerFile.fileID WHERE fileshare.dbo.fileTable.UserID = '"+Userid+"' OR fileshare.dbo.usersPerFile.userID = '"+Userid+"';";

            //selecteer de eigen bestanden
            //"SELECT [fileID],[fileName]" +
            //"FROM [dbo].[fileTable]" + 
            //"WHERE UserID = LoginID" + //LoginID = ID van ingelogde gebruiker =/ gast
            //"OR" +

            //selecteer de openbaar gedeelde bestanden
            //"WHERE shareBoolean = TRUE" +
            //"OR" +

            //selecteer de met loginID gedeelde bestanden
            //"SELECT [fileID],[fileName]" +
            //"FROM [dbo].[fileTable]" +
            //"WHERE [dbo].[fileTable].[fileID] = [dbo].[usersPerFile].[fileID]" +
            //"AND" +
            //"WHERE [usersPerFile].[userID] = [loginID]";

            SqlDataReader reader;
            reader = cmd.ExecuteReader();

            string name = "";
            Guid id = new Guid();
            while (reader.Read())
            {
                id = reader.GetGuid(0);
                name = reader.GetString(1);
                Item currentItem = new Item();
                currentItem.id = id;
                currentItem.name = name;
                itemlist.Add(currentItem);
            }
            reader.Close();
            connection.Close();
            return itemlist;
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
                /*
                cmd2 = connection.CreateCommand();
                cmd2.CommandText = "DELETE FROM [dbo].[files] WHERE fileID = @fileID";
                cmd2.Parameters.AddWithValue("@fileID", file.id);

                int result2 = cmd2.ExecuteNonQuery();
                */
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
                cmd2.CommandText = "INSERT INTO [dbo].[files](fileID, ActualFile)SELECT '"+Convert.ToString(uniqueid)+"', BulkColumn FROM OPENROWSET(BULK '"+ fileData.path + "', SINGLE_BLOB) as f;";

                //cmd2.Parameters.AddWithValue("@uniqueID", (Guid)uniqueid);
                
                //cmd2.Parameters.AddWithValue("@filepath", fileData.path);

                cmd2.ExecuteNonQuery();

                return true;

            }
            catch (SqlException sqlex)
            {
                Debug.WriteLine(sqlex.Message);
                return false;
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
            cmd.CommandText = "SELECT [Password], [UserId] FROM [dbo].[Users] WHERE UserName = @username";
            cmd.Parameters.AddWithValue("@username", user.name);

            string password = "";
            string UserId = "";
            Md5Class hashing = new Md5Class();
            bool checkPassword = false;
            Random random = new Random();
            SqlDataReader reader;

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                password = reader.GetString(0);
                UserId = Convert.ToString(reader.GetGuid(1));
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
                user.id = UserId;
            }

            return user;
        }





    }
}