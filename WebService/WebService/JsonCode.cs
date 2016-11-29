using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace WebService
{
    public class JsonCode
    {
        Data data = new Data();

        public Data JsonDeCoding(String json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            return data;
        }

        public String JsonCoding(Data data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        public Succes JsonDeCodingSucces(String json)
        {
            Succes succes = JsonConvert.DeserializeObject<Succes>(json);
            return succes;
        }

        public String JsonCoding(Succes succes)
        {
            string json = JsonConvert.SerializeObject(succes);
            return json;
        }

        public Item JsonDeCodingItem(String json)
        {
            Item item = JsonConvert.DeserializeObject<Item>(json);
            return item;
        }

        public String JsonCoding(Item item)
        {
            string json = JsonConvert.SerializeObject(item);
            return json;
        }
        public User JsonDeCodingUser(String json)
        {
            User user = JsonConvert.DeserializeObject<User>(json);
            return user;
        }

        public String JsonCoding(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            return json;
        }

        public FileList JsonDeCodingFileList(String json)
        {
            FileList fileList = JsonConvert.DeserializeObject<FileList>(json);
            return fileList;
        }

        public String JsonCoding(FileList fileList)
        {
            string json = JsonConvert.SerializeObject(fileList);
            return json;
        }

        public UserList JsonDeCodingUserList(String json)
        {
            UserList userList = JsonConvert.DeserializeObject<UserList>(json);
            return userList;
        }
        public String JsonCodingUserList(UserList userList)
        {
            string json = JsonConvert.SerializeObject(userList);
            return json;
        }

        public string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }


        public T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        public string cropString(string input)
        {
            string output;
            input = input.Remove(0, 1);
            output = input.Remove(input.Length - 1);
            output = output.Replace("\\\"", "\"");
            output = "{\"" + output + "\"}";
            return output;
        }


    }
}