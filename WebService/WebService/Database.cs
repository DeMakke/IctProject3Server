using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebService
{
    public class Database
    {
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.DBconnectionFrederik); // maak je eigen connectionstring en verander de naam
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmd2 = new SqlCommand();

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

        public List<Gebruiker> GetUsersData()//functie haalt gevens van alle gebruikers op
        {
            connection.Open();
            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserID, UserName FROM Users";
            SqlDataReader reader;
            reader = cmd.ExecuteReader();
            Gebruiker user = new Gebruiker();
            List<Gebruiker> UserList = new List<Gebruiker>();
                       
            while (reader.Read())
            {
                user.id = reader.GetGuid(0);
                user.name = reader.GetString(1);
                UserList.Add(user);
            }
            reader.Close();
            connection.Close();
            
            return UserList;
        }

        public bool DeleteData(Item file)
        {
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();               
                cmd.CommandText = "DELETE FROM fileTable WHERE fileName = '@fileName'";//later aanpassen met "and user  = '' ofzo 
                cmd.Parameters.AddWithValue("@fileName", file.name);               

                cmd.ExecuteNonQuery();
                return true;

                //if (cmd.ExecuteNonQuery() == 1)
                //{
                //    // querry gedaan
                //    return true;
                //}
                //else
                //{
                //    //query niet gedaan
                //    return false;
                //}
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

        public bool SelectedUsers(Guid fileid, List<Gebruiker> selectedUsers)
        {
            // file bool op public zetten?of doet maxim dit?
            //insert/update lijst van geselecteerde gebruikers in database
            // succes obj terugsturn

            connection.Open();

            cmd = connection.CreateCommand();
            //cmd.CommandText = "SELECT [fileID],[fileName] FROM [dbo].[fileTable]";
            //new

            //hoe heet de boolian in de database??????Boolfile?
            //cmd.CommandText = "UPDATE INTO [dbo].[files](Boolfile)SELECT '@uniqueid', BulkColumn FROM OPENROWSET(BULK '@filepath', SINGLE_BLOB) as f;";

            //cmd.CommandText = "UPDATE [dbo].[files] SET Boolfile = true WHERE(uniqueid = '@uniqueid')";


            foreach (Gebruiker user in selectedUsers) //jan
            {
                cmd.CommandText = "INSERT INTO [dbo].[usersPerFile] VALUES (@fileid,@user)";
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@fileid", fileid);
                cmd.ExecuteNonQuery();
            }

            return true;
        }



        public void getSelectedItem(Guid uniqueid)
        {

            //return file;
        }
    }
}