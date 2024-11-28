using System.Text.RegularExpressions;
using System.Xml.Serialization;
using MilovanovicTest.Models;

namespace MilovanovicTest
{
    class Program
    {
        private static string findPersonLine =
            "https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id=";

        private static string newCSV = "Data/newCSV.csv";
        private static string mainCSV = "Data/oldCSV.csv";
        private static string testCSV = "Data/oldCSVTest.csv";
        
        static HttpClient client = new HttpClient();
        static async Task<Person> getPersonAsync(string path)
        {
            Person person = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var xmlString = await response.Content.ReadAsStringAsync();
                
                // preprocess string
                var lines = xmlString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var result = string.Join(Environment.NewLine, lines.Skip(2).Take(1));
                result = Regex.Replace(result, @"<SOAP-ENV:Body>", " ");
                result = Regex.Replace(result, @"</SOAP-ENV:Body>", " ");
                result = Regex.Replace(result, @"xmlns=""http://tempuri.org""", "");
                
                // serialize
                XmlSerializer serializer = new XmlSerializer(typeof(Person));
                using (StringReader reader = new StringReader(result))
                {
                    person = (Person)serializer.Deserialize(reader);
                }
            }
            return person;
        }
        
        static async void addCsv(string path)
        {
            List<int> newPeople = readCSV(path);
            int count = 0;
            using (StreamReader sr = new StreamReader(testCSV))
            {
                string line;
                bool isFirst = true;
                while ((line = sr.ReadLine()) != null)
                {
                    if (isFirst) {
                        isFirst = false;
                        continue;
                    }

                    if (count >= 5)
                    {
                        break;
                    }
                    if (int.TryParse(line, out int personId))
                    {
                        if (newPeople.Contains(personId))
                        {
                            newPeople.Remove(personId);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid entry found: {line}");
                    }
                }
            }
            count += newPeople.Count;
            addToCSV(newPeople);
            Console.WriteLine($"Added {count} people");
            foreach (var id in newPeople)
            {
                Console.WriteLine(getPersonAsync(findPersonLine + id).Result.FindPersonResult.Name);
            }
        }
        
        static List<int> readCSV(string fileName)
        {
            var peopleIds = new List<int>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                bool isFirst = true;
                while ((line = sr.ReadLine()) != null)
                {
                    if (isFirst) {
                        isFirst = false;
                        continue;
                    }
                    if (int.TryParse(line, out int personId))
                    {
                        peopleIds.Add(personId);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid entry found: {line}");
                    }
                }
            }
            return peopleIds;
        }
        
        static void addToCSV(List<int> persons)
        {
            using (StreamWriter writer = new StreamWriter(testCSV, append: true))
            {
                foreach (int person in persons)
                {
                    writer.WriteLine(person.ToString());
                }
            }
        }
        
        static void Main()
        {
            addCsv(newCSV);
        }
        
    }
}