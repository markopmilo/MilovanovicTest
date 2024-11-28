using System.Xml.Serialization;

namespace MilovanovicTest.Models;

public class Address
{
    [XmlElement(ElementName="Street")] 
    public string Street { get; set; } = string.Empty;
    [XmlElement(ElementName="City")] 
    public string City { get; set; } = string.Empty;
    [XmlElement(ElementName="State")] 
    public string State { get; set; } = string.Empty;
    [XmlElement(ElementName="Zip")] 
    public int Zip { get; set; }
}