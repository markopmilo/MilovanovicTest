using System.Xml.Serialization;

namespace MilovanovicTest.Models;
[Serializable]
[XmlRoot("FindPersonResponse")]
public class Person
{
    [XmlElement("FindPersonResult")]
    public FindPersonResult FindPersonResult { get; set; } = new FindPersonResult();
}