using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class loadXmlFile : MonoBehaviour 
{
    public TextAsset xmlRawFile;
    public Text uiText;

    // Use this for initialization
    void Start () 
    {
        string data = xmlRawFile.text;
        parseXmlFile(data);
    }

    void parseXmlFile(string xmlData)
    {
        string totVal = "";
        XmlDocument xmlDoc = new XmlDocument ();
        xmlDoc.Load(new StringReader(xmlData));

        string xmlPathPattern = "//Navigation/Browser";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        foreach(XmlNode node in myNodeList)
        {
            XmlNode name = node.FirstChild;
            XmlNode addr = name.NextSibling;
            XmlNode phone = addr.NextSibling;

            totVal += " Name : "+name.InnerXml+"\n Address : "+ addr.InnerXml+"\n Mobile : "+phone.InnerXml+"\n\n";
            uiText.text = totVal;
        }
    }
}