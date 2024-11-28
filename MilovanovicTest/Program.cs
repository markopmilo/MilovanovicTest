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
        
        static void addCsv(string path)
        {
            List<int> newPeople = readCSV(path);
            List<int> rejected = new List<int>();
            List<int> extraPeople = new List<int>();
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
                    if (int.TryParse(line, out int personId))
                    {
                        if (newPeople.Contains(personId))
                        {
                            newPeople.Remove(personId);
                            rejected.Add(personId);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid entry found: {line}");
                    }
                }
            }
            int count = newPeople.Count;
            if (count > 5)
            {
                for (int i = 5; i < count; i++)
                {
                    extraPeople.Add(newPeople[i]);
                    newPeople.Remove(newPeople[i]);
                }
            }
            addToCSV(newPeople);
            Console.WriteLine($"Added {newPeople.Count} people");
            foreach (var id in newPeople)
            {
                Console.WriteLine(getPersonAsync(findPersonLine + id).Result.FindPersonResult.Name);
            }

            if (rejected.Count > 0)
            {
                if (rejected.Count == 1)
                {
                    Console.WriteLine($"1 person from your list was already included in the promotion:");
                }
                else
                {
                    Console.WriteLine($"{rejected.Count} people from your list were already included in the promotion:");
                }
                foreach (var id in rejected)
                {
                    Console.WriteLine(getPersonAsync(findPersonLine + id).Result.FindPersonResult.Name);
                }
            }
            if (extraPeople.Count > 0)
            {
                Console.WriteLine($"You added more than 5 people for your promotion, these following people could not be included:");
                foreach (var id in extraPeople)
                {
                    Console.WriteLine(getPersonAsync(findPersonLine + id).Result.FindPersonResult.Name);
                }
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