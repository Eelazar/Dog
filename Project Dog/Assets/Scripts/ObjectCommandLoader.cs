using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class ObjectCommandLoader : MonoBehaviour
{

    void Awake()
    {

        var objectFiles = Resources.LoadAll("Data/Commands/Objects/Test", typeof(TextAsset)).Cast<TextAsset>();

        foreach (TextAsset objectFile in objectFiles)
        {
            Stream stream = new MemoryStream(objectFile.bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(ObjectCommands));
            StreamReader reader = new StreamReader(stream);
            ObjectCommands deserialized = (ObjectCommands)serializer.Deserialize(reader.BaseStream);
            reader.Close();

            ObjectCommandManager.AddCommands(deserialized);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
