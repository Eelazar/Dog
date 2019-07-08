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
        LoadCommandsFromFiles();
    }

    public void LoadCommandsFromFiles()
    {
        //Load all xml files in Data/Commands/Objects/
        var objectFiles = Resources.LoadAll("Data/Commands/Objects", typeof(TextAsset)).Cast<TextAsset>();

        foreach (TextAsset objectFile in objectFiles)
        {

            //Read the text file
            Stream stream = new MemoryStream(objectFile.bytes);

            //Serialize the text file as a ObjectCommands Object
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectCommands));
            StreamReader reader = new StreamReader(stream);
            ObjectCommands deserialized = (ObjectCommands)serializer.Deserialize(reader.BaseStream);
            reader.Close();

            //Add Commands to CommandManager to be used later
            ObjectCommandManager.AddCommands(deserialized);
        }
    }
}
