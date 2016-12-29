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
        public static List<Session> ActiveUsers = new List<Session>();
        JsonCode json = new JsonCode();
        Database db = new Database();
        

        public void GetData()
        {
            Database databseInterface = new Database();

            //databseInterface.GetData();
        }

        public string Default(Stream data)
        {
            StreamReader reader = new StreamReader(data);
            String JSONData = reader.ReadToEnd();

            return "Positive response";
        }

        public string GetFile(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                StreamReader reader = new StreamReader(Data);
                string JSONData = reader.ReadToEnd();

                BinaryFile binaryfile = new BinaryFile();
                Database database = new Database();
                Base64Code base64 = new Base64Code();
                Data data = new Data();
                JsonCode json = new JsonCode();
                //data.base64 = "amEgbmVlIGlrIGdhIG1pam4gcGFzIG5pZSBkb29yc3R1cmVu";
                //data.name = "tsserver.txt";

                string fileid = JSONData.Replace("\\\"", "\"");

                binaryfile = database.getSelectedItem(new Guid(fileid));
                data.base64 = base64.SerializeBase64(binaryfile.binary);
                data.name = binaryfile.name;

                string JsonToSend = json.JsonCoding(data);

                return JsonToSend;
            }
            else
            {
                return "user is not logged in";
            }
        }

        //17.	server functie die json xxx succes string terugstuurt naar client (aanpassen)
        
        public string DeleteFile(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                    Succes succes = new Succes();
                    Item item = new Item();
                    StreamReader reader = new StreamReader(Data);

                    string JSONData = reader.ReadToEnd();
                    item = json.JsonDeCodingItem(JSONData);

                    Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));
                    string userName = session.name;
                    succes.value = db.DeleteData(item, userName);
                    return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }

        public string SaveFile(Stream data, string id,string max, string current,string token)
        {
            if (CheckUserStatus(token))
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
                    Tuple<byte[], string> filebytes = base64.DeSerializeBase64(inp[Convert.ToInt16(id)].FileData);
                    //inp[Convert.ToInt16(id)].FileData.path = base64.saveFile(filebytes.Item1, inp[Convert.ToInt16(id)].FileData.name); // this line saves the fiile on the server itself

                    Database database = new Database();
                    Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));

                    bool status = database.AddRecord(inp[Convert.ToInt16(id)].FileData, session.id, filebytes.Item1);
                    Debug.WriteLine(status);
                    JSONData = "OK" + inp[Convert.ToInt16(id)].base64stringpackets.Count;

                    inp.RemoveAll(x => x.id == Convert.ToInt16(id));

                }

                return JSONData;

            }
            else
            {
                return "user is not logged in";
            }
        }

        public string CheckDivisionOfData(Stream Data, string token)
        {
            if (CheckUserStatus(token))
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
                    else
                    {
                        amountOfPackets = 1;
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
            else
            {
                return "user is not logged in";
            }


        }

        public string GetFileNames(Stream Data, string token)
        {
            JsonCode json = new JsonCode();
            Database database = new Database();
            if (CheckUserStatus(token))
            {
                

                Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));

                List<Item> itemlist = database.GetData(session.id);
                //List<Item> itemlist = database.GetItems();
                //Debug.WriteLine("test");
                string reply = json.Serialize<List<Item>>(itemlist);

                List<Item> itemlistdebug = json.Deserialize<List<Item>>(reply);

                return reply;
            }
            else
            {

                List<Item> itemlist = database.GetGuestData();
                //List<Item> itemlist = database.GetItems();
                //Debug.WriteLine("test");
                string reply = json.Serialize<List<Item>>(itemlist);

                List<Item> itemlistdebug = json.Deserialize<List<Item>>(reply);

                return reply;
            }
        }

        public String ValidateUser(Stream data, string token)
        {
            //db.SetUser();    //initialise a user in the db for testing purposes, only do this once!
            StreamReader reader = new StreamReader(data);
            string JSONData = reader.ReadToEnd();
            User user = json.JsonDeCodingUser(JSONData);
            user = db.ValidateUser(user);
            if (!CheckUserStatus(user.id)) //toegevoegd door Frederik zodat deze functie gebruikt kan worden voor reeds bestaande session-users om het password te checken zonder dat er telkens een nieuwe session wordt aangemaakt voor deze user
            {
                // begin code van ~dries
                Session userToAdd = new Session(user.name, user.hash, user.token, user.id); //maakt de usersession aan op de server (dit is een childclass van User)
                ActiveUsers.Add(userToAdd); // voegt de user toe aan de sessions op de server
                                            // einde code dries
            }


            string result = json.JsonCoding(user);
            return result;

        }

        // false = user is niet ingelogged of niet gevonden      true = user is ingelogged
        public bool CheckUserStatus(string id)
        {
            try
            {
                Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(id));
                int index = ActiveUsers.FindIndex(ActiveUsers => ActiveUsers.token == new Guid(id));
                ActiveUsers[index].Refresh();
                if (session.Status == true)
                {
                    return true;
                }
                else
                {
                    ActiveUsers.RemoveAt(ActiveUsers.FindIndex(ActiveUsers => ActiveUsers.token == new Guid(id)));
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public string GetUsers(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                JsonCode json = new JsonCode();
                Database db = new Database();

                List<Gebruiker> gebruikerlist = db.GetUsersData();
                string reply = json.Serialize<List<Gebruiker>>(gebruikerlist);

                return reply;
            }
            else
            {
                return "user is not logged in";
            }

        }


        //sprint4 story 7 bestanden delen
        public string SetUsers(Stream Data, string fileid, string token)
        {
            if (CheckUserStatus(token))
            {
                JsonCode json = new JsonCode();
                Database db = new Database();
                Succes succes = new Succes();
                StreamReader reader = new StreamReader(Data);
                string JSONData = reader.ReadToEnd();
                List<Gebruiker> userlist = json.Deserialize<List<Gebruiker>>(JSONData);
                succes.value = db.SelectedUsers(fileid, userlist);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }

        //sprint 4 story 6 publiek delen
        public string PublicShare(Stream Data, string fileid, string token)
        {
            if (CheckUserStatus(token))
            {
                Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));
                string ownerId = session.id;
                Database database = new Database();
                JsonCode json = new JsonCode();
                Succes succes = new Succes();
                StreamReader reader = new StreamReader(Data);
                succes.value = database.ShareWithAll(fileid, ownerId);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }

        }

        public string AddUser(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                Database database = new Database();
                JsonCode json = new JsonCode();
                Succes succes = new Succes();

                StreamReader reader = new StreamReader(Data);
                string JSONData = reader.ReadToEnd();
                User user = json.JsonDeCodingUser(JSONData);

                string id = user.id;
                string name = user.name;
                string password = user.password;
                succes.value = database.AddUser(id, name, password);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }

        public string DeleteUser(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                Database database = new Database();
                JsonCode json = new JsonCode();
                Succes succes = new Succes();

                StreamReader reader = new StreamReader(Data);
                string JSONData = reader.ReadToEnd();
                User user = json.JsonDeCodingUser(JSONData);

                string id = user.id;
                succes.value = database.DeleteUser(id);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }

        public string UpdateUser(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                Database database = new Database();
                JsonCode json = new JsonCode();
                Succes succes = new Succes();

                StreamReader reader = new StreamReader(Data);
                string JSONData = reader.ReadToEnd();
                User user = json.JsonDeCodingUser(JSONData);

                string id = user.id;
                string name = user.name;
                string password = user.password;
                succes.value = database.UpdateUser(id, name, password);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }

        public string Logout(Stream Data, string token)
        {
            Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));
            ActiveUsers.Remove(session);

            User user = new User();
            user.token = new Guid();
            user.name = "guest";
            string result = json.JsonCoding(user);
            return result;
        }

        //print 5 story 12
        public string ChangeUserData(Stream Data, string token)
        {
            if (CheckUserStatus(token))
            {
                json = new JsonCode();
                db = new Database();
                Succes succes = new Succes();
                StreamReader reader = new StreamReader(Data);
                Session session = ActiveUsers.Find(ActiveUsers => ActiveUsers.token == new Guid(token));

                string JSONData = reader.ReadToEnd();
                User user = json.Deserialize<User>(JSONData);
                user.id = session.id;
                
                succes.value = db.ChangeUserData(user);
                return json.JsonCoding(succes);
            }
            else
            {
                return "user is not logged in";
            }
        }
    }
}
