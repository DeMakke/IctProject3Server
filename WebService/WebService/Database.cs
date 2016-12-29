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
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.DBconnectionFrederik); // maak je eigen connectionstring en verander de naam

        SqlCommand cmd = new SqlCommand();
        SqlCommand cmd2 = new SqlCommand();
        SqlCommand cmd3 = new SqlCommand();


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
                Boolean noDuplicate = true;
                foreach (Item item in itemlist)
                {
                    if (item.id == currentItem.id)
                    {
                        noDuplicate = false;
                    }
                }
                if (noDuplicate)
                {
                    itemlist.Add(currentItem);
                }
            }
            reader.Close();
            connection.Close();
            return itemlist;
        }

        public bool DeleteData(Item file, string userName)
        {
            try
            {
                Guid guid = GetUserGUID(userName);
                connection.Open();
                cmd = connection.CreateCommand(); //delete actual file
                cmd.CommandText = "DELETE FROM fileTable WHERE fileID = @fileID and UserID = @UserID"; 
                cmd.Parameters.AddWithValue("@fileID", file.id);
                cmd.Parameters.AddWithValue("@UserID", guid);
                int result1 = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                cmd2 = connection.CreateCommand(); //delete share link only
                cmd2.CommandText = "DELETE FROM usersPerFile WHERE fileID = @fileID and UserID = @UserID"; 
                cmd2.Parameters.AddWithValue("@fileID", file.id);
                cmd2.Parameters.AddWithValue("@UserID", guid);
                int result2 = cmd2.ExecuteNonQuery();
                cmd2.Parameters.Clear();

                if (result1 >= 1) //user who tries to delete the file is the owner so all shared links can be deleted
                {
                    cmd3 = connection.CreateCommand();
                    cmd3.CommandText = "DELETE FROM usersPerFile WHERE fileID = @fileID";
                    cmd3.Parameters.AddWithValue("@fileID", file.id);
                    int result3 = cmd3.ExecuteNonQuery();
                    cmd3.Parameters.Clear();
                }

                if ((result1 >= 1) || (result2 >= 1)) //either owner/sharedWithUser deleted a file/share link to user
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

        }

        public Guid GetUserGUID(string userName)//functie haalt guid van 1 gebruiker op
        {
            connection.Open();
            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserID, UserName FROM Users WHERE UserName = @userName";
            cmd.Parameters.AddWithValue("@userName", userName);
            SqlDataReader reader;
            reader = cmd.ExecuteReader();
            Guid guid = new Guid();
            
            while (reader.Read())
            {
                guid = reader.GetGuid(0);
            }
            reader.Close();
            connection.Close();

            return guid;
        }

        public bool AddRecord(Data fileData, string UserID, byte[] file)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO [dbo].[fileTable] ([fileName],[filePath],[UserID],[publiek]) OUTPUT inserted.fileID VALUES (@filename, @filepath, @userID, @publiek)";

                cmd.Parameters.AddWithValue("@filename", fileData.name);
                cmd.Parameters.AddWithValue("@filepath", fileData.path);
                cmd.Parameters.AddWithValue("@userID", UserID);
                cmd.Parameters.AddWithValue("@publiek", 0);

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
                //Guid newguid = Guid.NewGuid();
                string newguidformatted = "00000000-0000" + Convert.ToString(Guid.NewGuid()).Substring(13);

                user.token = new Guid(newguidformatted);
                user.id = UserId;
            }
            else
            {
                user.token = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");

            }

            return user;
        }

        public List<Gebruiker> GetUsersData()//functie haalt gevens van alle gebruikers op
        {
            List<Gebruiker> userList = new List<Gebruiker>();
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT UserID, UserName FROM Users";
                SqlDataReader reader;
                reader = cmd.ExecuteReader();
                Gebruiker user;
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
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
            finally {
                connection.Close();
            }
            return userList;
        }

        public bool SelectedUsers(string fileid, List<Gebruiker> selectedUsers)
        {
            try
            {
                Guid dbUserID = new Guid();
                List<Gebruiker> bestaandeGebruikers = new List<Gebruiker>();
                Gebruiker bestaandeGebruiker;

                //1. bestaande gebruikers voor het bestand ophalen
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM [dbo].[usersPerFile] WHERE fileID=@fileID";
                cmd.Parameters.AddWithValue("@fileID", fileid);
                SqlDataReader reader;
                reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();

                //2. gebruikers die door de client worden meegestuurd en al in de usersperfile tabel bij de file voorkomen, wegfilteren van selectedusers lijst

                while (reader.Read())
                {
                    dbUserID = reader.GetGuid(1);
                    foreach (Gebruiker user in selectedUsers)
                    {
                        if (user.id == dbUserID.ToString())
                        {
                            bestaandeGebruiker = user;
                            bestaandeGebruikers.Add(bestaandeGebruiker);
                        }
                    }
                }
                reader.Close();

                foreach (Gebruiker user in bestaandeGebruikers)
                {
                    selectedUsers.Remove(user);
                }

                //3. nieuwe gebruikers toevoegen aan de file in de tabel usersperfile
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
        public List<Item> GetGuestData()
        {
            List<Item> itemlist = new List<Item>();
            connection.Open();

            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT fileTable.fileId, fileTable.fileName FROM files INNER JOIN fileTable ON files.fileID = fileTable.fileID WHERE(fileTable.publiek = 1);";

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
        public bool ShareWithAll(string fileid, string ownerId)
        {
            try
            {
                Guid ID = Guid.Parse(fileid);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE [dbo].[fileTable] set publiek=@shareBool WHERE fileID =@fileID";
                cmd.Parameters.AddWithValue("@shareBool", 1);
                cmd.Parameters.AddWithValue("@fileID", ID);
                cmd.ExecuteNonQuery();
                connection.Close();

                List<Gebruiker> gebruikers = GetUsersData();
                connection.Open();
                foreach (Gebruiker gebruiker in gebruikers)
                {
                    if (gebruiker.id != ownerId)
                    {
                        cmd2 = connection.CreateCommand();
                        cmd2.CommandText = "INSERT INTO usersPerFile (fileID, userID) VALUES (@fileId, @userId)";
                        cmd2.Parameters.AddWithValue("@fileId", ID);
                        cmd2.Parameters.AddWithValue("@userId", gebruiker.id);
                        cmd2.ExecuteNonQuery();
                    }
                }
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
                cmd.Parameters.Clear();
                cmd2.Parameters.Clear();
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