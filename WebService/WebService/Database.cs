using System;
using System.Collections.Generic;
using System.Data;
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
            cmd.CommandText = "SELECT fileTable.fileId, fileTable.fileName FROM files INNER JOIN fileTable ON files.fileID = fileTable.fileID INNER JOIN usersPerFile ON files.fileID = usersPerFile.fileID WHERE(usersPerFile.UserID = '"+Userid+"');";

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

        public bool AddRecord(Data fileData, string UserID, byte[] file)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[fileTable] ([fileName],[filePath],[UserID]) OUTPUT inserted.fileID VALUES (@filename, @filepath, @userID)";

                cmd.Parameters.AddWithValue("@filename", fileData.name);
                cmd.Parameters.AddWithValue("@filepath", fileData.path);
                cmd.Parameters.AddWithValue("@userID", UserID);

                Guid uniqueid = (Guid)cmd.ExecuteScalar();

                cmd2 = connection.CreateCommand();
                cmd2.CommandText = "INSERT INTO [dbo].[files](fileID, ActualFile) VALUES (@fileID, @binary);";

                cmd2.Parameters.Add("@fileID", SqlDbType.UniqueIdentifier).Value = uniqueid;

                cmd2.Parameters.Add("@binary", SqlDbType.VarBinary, file.Length).Value = file;

                cmd2.ExecuteNonQuery();

                cmd2 = connection.CreateCommand();
                cmd2.CommandText = "INSERT INTO[dbo].[usersPerFile]([fileID], [userID]) VALUES('" + Convert.ToString(uniqueid) + "', '" + UserID + "')";
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

        public BinaryFile getSelectedItem(Guid uniqueid)
        {
            BinaryFile binaryFile = new BinaryFile();

            try
            {
                connection.Open();

                cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT fileName, ActualFile FROM files INNER JOIN fileTable ON files.fileID = fileTable.fileID WHERE files.fileID = @fileID;";
                cmd.Parameters.Add("@fileID", SqlDbType.UniqueIdentifier).Value = uniqueid;

                SqlDataReader reader;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    binaryFile.name = reader.GetString(0);
                    binaryFile.binary = (byte[])reader.GetValue(1);
                }
                reader.Close();
                connection.Close();

                return binaryFile;
            }
            catch (SqlException e)
            {
                connection.Close();
                Debug.WriteLine(e.Message);
                return new BinaryFile();
            }
            catch (Exception e)
            {
                connection.Close();
                Debug.WriteLine(e.Message);
                return new BinaryFile();
            }


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
                user.id = UserId;
            }
            else
            {
                user.token = 9999;
                
            }

            return user;
        }

        public List<Gebruiker> GetUsersData()//functie haalt gevens van alle gebruikers op
        {
            connection.Open();
            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserID, UserName FROM Users";
            SqlDataReader reader;
            reader = cmd.ExecuteReader();
            Gebruiker user;
            List<Gebruiker> userList = new List<Gebruiker>();
            Guid userid = new Guid();

            while (reader.Read())
            {
                user = new Gebruiker();
                userid = reader.GetGuid(0);
                user.id = Convert.ToString(userid);
                user.name = reader.GetString(1);
                userList.Add(user);
            }
            reader.Close();
            connection.Close();

            return userList;
        }

        public bool SelectedUsers(string fileid, List<Gebruiker> selectedUsers)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();

                foreach (Gebruiker user in selectedUsers)
                {
                    cmd.CommandText = "INSERT INTO [dbo].[usersPerFile] VALUES (@fileid,@userid)";
                    cmd.Parameters.AddWithValue("@userid", user.id);
                    cmd.Parameters.AddWithValue("@fileid", fileid);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool ShareWithAll(string fileid)
        {
            try
            {
                int shareBool = 1;
                Guid ID = Guid.Parse(fileid);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE [dbo].[files] set publiek=@shareBool WHERE fileID =@fileID";
                cmd.Parameters.AddWithValue("@shareBool", shareBool);
                cmd.Parameters.AddWithValue("@fileID", ID);

                cmd.ExecuteNonQuery();
                return true;

            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool AddUser(string id, string name, string password)
        {
            try
            {
                Guid ID = Guid.Parse(id);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Users (UserID, UserName, Password) VALUES(@UserID,@UserName,@Password)";
                cmd.Parameters.AddWithValue("@UserID", ID);
                cmd.Parameters.AddWithValue("@UserName", name);
                cmd.Parameters.AddWithValue("@Password", password);

                cmd.ExecuteNonQuery();
                return true;

            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool DeleteUser(string id)
        {
            try
            {
                Guid ID = Guid.Parse(id);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM [dbo].[Users] WHERE UserID=@UserId";
                cmd.Parameters.AddWithValue("@UserID", ID);

                cmd.ExecuteNonQuery();
                return true;

            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool UpdateUser(string userId, string name, string password)
        {
            try
            {
                Guid ID = Guid.Parse(userId);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE Users set UserName=@UserName,Password=@Password WHERE UserID=@UserID";
                cmd.Parameters.AddWithValue("@UserID", ID);
                cmd.Parameters.AddWithValue("@UserName", name);
                cmd.Parameters.AddWithValue("@Password", password);

                cmd.ExecuteNonQuery();
                return true;

            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool ChangeUserData(User gebruiker)
        {
            try
            {                
                connection.Open();
                cmd = connection.CreateCommand();
                if (gebruiker.name != "" && gebruiker.password != "")//beide
                {
                    cmd.CommandText = "UPDATE Users SET UserName=@username, Password=@password WHERE UserID=@userid";
                    cmd.Parameters.AddWithValue("@userid", gebruiker.id);
                    cmd.Parameters.AddWithValue("@username", gebruiker.name);
                    cmd.Parameters.AddWithValue("@password", gebruiker.password);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return true;
                }
                else if(gebruiker.name != "" && gebruiker.password == "")//naam
                {
                    cmd.CommandText = "UPDATE Users SET UserName=@username WHERE UserID=@userid";
                    cmd.Parameters.AddWithValue("@userid", gebruiker.id);
                    cmd.Parameters.AddWithValue("@username", gebruiker.name);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return true;
                }
                else if (gebruiker.name == "" && gebruiker.password != "")//ww
                {
                    cmd.CommandText = "UPDATE Users SET Password=@password WHERE UserID=@userid";
                    cmd.Parameters.AddWithValue("@userid", gebruiker.id);
                    cmd.Parameters.AddWithValue("@password", gebruiker.password);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return true;
                }
                return false;

            }
            catch (SqlException e)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}