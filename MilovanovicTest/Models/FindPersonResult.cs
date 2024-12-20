using System.Xml.Serialization;

namespace MilovanovicTest.Models;
[Serializable]
public class FindPersonResult
{
    public int Id { get; set; }
    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;
    [XmlElement(ElementName="SSN")] 
    public string Ssn { get; set; } = string.Empty;
    [XmlElement(ElementName="DOB")] 
    public string DoB { get; set; } = string.Empty;
    [XmlElement(ElementName="Home")]
    public Address HomeAddress { get; set; } = new Address();
    [XmlElement(ElementName="Office")]
    public Address WorkAddress { get; set; } = new Address();
    [XmlArray(ElementName="FavoriteColors")]
    [XmlArrayItem("FavoriteColorsItem")]
    public List<string> FavoriteColors { get; set; } = new List<string>();
    [XmlElement(ElementName="Age")] 
    public int Age { get; set; }
}