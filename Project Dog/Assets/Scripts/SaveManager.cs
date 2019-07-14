using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static int currentSave = 1;

    private Vector3 spawnPosition;
    private int currentCamera;
    private string currentScene;

    private XmlDocument xmlDoc;
    private XPathNavigator nav;

    void Start()
    {
        // Open the XML.
        xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets\\Scripts\\SaveFile.xml");
        // Create a navigator to query with XPath.
        nav = xmlDoc.CreateNavigator();
        
    }

    void Update()
    {
        
    }

    public void SetRespawn(Vector3 respawn, int cam, string scene)
    {
        spawnPosition = respawn;
        currentCamera = cam;
        currentScene = scene;

        Save();
    }

    private void Save()
    {
        //Initial XPathNavigator to start in the root.
        nav.MoveToRoot();
        nav.MoveToFirstChild();

        nav.MoveToFirstChild();
        nav.SetValue("" + currentSave);

        nav.MoveToNext("save_" + currentSave, string.Empty);

        nav.MoveToChild("spawn", string.Empty);
        nav.MoveToFirstChild();
        nav.SetValue("" + spawnPosition.x);
        nav.MoveToNext();
        nav.SetValue("" + spawnPosition.y);
        nav.MoveToNext();
        nav.SetValue("" + spawnPosition.z);

        nav.MoveToParent();
        nav.MoveToParent();

        nav.MoveToChild("camera", string.Empty);
        nav.SetValue("" + currentCamera);

        nav.MoveToParent();

        nav.MoveToChild("level", string.Empty);
        nav.SetValue(currentScene);

        xmlDoc.Save("Assets\\Scripts\\SaveFile.xml");
    }
}
