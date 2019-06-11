using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using TMPro;

public class XMLLoader : MonoBehaviour 
{
    public TextAsset xmlRawFile;
    public TMP_Text uiText;

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

        string xmlPathPattern = "//Help/Navigation";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        foreach(XmlNode node in myNodeList)
        {
            XmlNode name = node.FirstChild;
            XmlNode addr = name.NextSibling;
            XmlNode phone = addr.NextSibling;

            totVal += "1 : "+name.InnerText +"\n2 : "+ addr.InnerXml+"\n3 : "+phone.Name+"\n\n";
            uiText.text = totVal;
        }
    }
}