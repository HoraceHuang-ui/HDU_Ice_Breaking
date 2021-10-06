using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text.RegularExpressions;

namespace Ice_Breaking
{
    public class Person
    {
        public string name;
        public string id;
        public string male;
        public string anonym;
        public string photo;

        public Person()
        {
            name = "";
            id = "";
            photo = "";
            male = "T";
            anonym = "F";
        }
        public Person(string n, string i, string m, string a, string ph)
        {
            name = n;
            id = i;
            male = m;
            anonym = a;
            photo = ph;
        }

        public Person(string s)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            Person p = deserializer.Deserialize<Person>(s);
            name = p.name;
            id = p.id;
            photo = p.photo;
            male = p.male;
            anonym = p.anonym;
        }
    }
    public class DataInitializer
    {
        public List<Person> person = new List<Person> { };
        public string raw_str = "";  //从文件中读出的所有学生信息
        public List<string> s_list = new List<string> { };  //分离后的学生信息
        public List<int> Male = new List<int> { };
        public List<int> Female = new List<int> { };

        public async Task InitDataAsync()
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile File = await storageFolder.CreateFileAsync("ice_breaking\\student_data.txt", CreationCollisionOption.OpenIfExists);

            raw_str = await FileIO.ReadTextAsync(File);
            s_list = FindMatch(raw_str);
            int i = 0;
            Person t;
            foreach (string a in s_list)
            {
                t = new Person(a);
                if (t.anonym == "F")
                {
                    if (t.male == "T")
                        Male.Add(i);
                    else Female.Add(i);
                }
                person.Add(t);
                i++;
            }
        }

        public async Task AddPersonAsync(string new_name, string new_id, string new_male, string new_photo)
        {
            Person new_person = new Person(new_name, new_id, new_male, "F", new_photo);
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile File = await storageFolder.GetFileAsync("ice_breaking\\student_data.txt");
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            int i;
            for (i = 0; i < person.Count; i++)
            {
                if (new_id.CompareTo(person[i].id) < 0)
                    break;
            }
            person.Insert(i, new_person);
            s_list.Insert(i, serializer.Serialize(new_person));
            if (new_male == "T")
                Male.Add(i);
            else
                Female.Add(i);
            raw_str = "";
            foreach (string s in s_list)
            {
                raw_str += s;
            }
            await FileIO.WriteTextAsync(File, raw_str);
        }

        private List<string> FindMatch(string a)
        {
            List<string> s = new List<string> { };
            MatchCollection mc = Regex.Matches(a, @"name: ([\u4e00-\u9fbb]*)([a-zA-Z\s]*)\r?\nid: (\d+)\r?\nmale: \w\r?\nanonym: \w\r?\nphoto: (\S+)\r?\n");
            foreach (Match m in mc)
            {
                s.Add(m.ToString());
            }
            return s;
        }
    }
}
