﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using UnityEngine.UI;
using TMPro;

public class XMLLoader : MonoBehaviour 
{
    public TextAsset xmlRawFile;
    public TMP_Text display_Text;
    public TMP_Text displayHeader_Text;

    public float textSpeed;

    private XPathNavigator nav;
    private XPathDocument docNav;

    void Start () 
    {
        // Open the XML.
        docNav = new XPathDocument("Assets\\Scripts\\CommandLog.xml");
        // Create a navigator to query with XPath.
        nav = docNav.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();
        //Move to first Child
        nav.MoveToFirstChild();

        UpdateXMLData();
    }

    public void MoveDown(string node)
    {
        if (nav.HasChildren)
        {
            nav.MoveToChild(node, string.Empty);
        }

        UpdateXMLData();
    }

    public void MoveUp()
    {
        nav.MoveToParent();

        UpdateXMLData();
    }


    void UpdateXMLData()
    {
        string finalText = "";

        //Get the header
        StartCoroutine(AnimateText(displayHeader_Text, "> " + nav.Name));

        //Check if the current node is an entry
        if (nav.HasAttributes)
        {
            if (nav.GetAttribute("type", string.Empty) == "entry")
            {
                finalText += "\n" + nav.Value;
            }
        }

        //Check for children
        else if (nav.HasChildren)
        {
            //Get the amount of children
            int childCount = nav.SelectChildren(XPathNodeType.All).Count;
            //Move to first child
            nav.MoveToFirstChild();

            for (int i = 0; i < childCount; i++)
            {
                finalText += "> " + nav.Name + "\n";

                //Move to the next sibling
                nav.MoveToNext();
            }

            //Return to the parent node
            nav.MoveToParent();
        }

        //Update Text
        StartCoroutine(AnimateText(display_Text, finalText));
    }

    IEnumerator AnimateText(TMP_Text ui, string s)
    {
        ui.text = "";

        foreach(char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }

}