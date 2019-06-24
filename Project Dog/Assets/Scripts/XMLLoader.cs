using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using UnityEngine.UI;
using TMPro;

public class XMLLoader : MonoBehaviour 
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The XML File to be loaded")]
    public TextAsset xmlRawFile;
    [SerializeField]
    [Tooltip("The Object where content is to be displayed")]
    public TMP_Text display_Text;
    [SerializeField]
    [Tooltip("The Object where content headers and titles are to be displayed")]
    public TMP_Text displayHeader_Text;
    [SerializeField]
    [Tooltip("The duration between each character being added during the typewriter animation")]
    public float textSpeed;
    #endregion Editor Variables

    #region Private Variables
    //XML Navigation Variables
    private XPathNavigator nav;
    private XPathDocument docNav;
    #endregion Private Variables

    private void Start () 
    {
        // Open the XML.
        docNav = new XPathDocument("Assets\\Scripts\\CommandLog.xml");
        // Create a navigator to query with XPath.
        nav = docNav.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();
        //Move to first Child
        nav.MoveToFirstChild();

        //Initial Update
        UpdateXMLData();
    }

    private void UpdateXMLData()
    {
        string finalText = "";

        //Get the header
        StartCoroutine(AnimateText(displayHeader_Text, "> " + nav.Name));

        //Check for attributes
        if (nav.HasAttributes)
        {
            //Check if the current node is an entry
            if (nav.GetAttribute("type", string.Empty) == "entry")
            {
                //If yes just display the text without looking for children
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

    private IEnumerator AnimateText(TMP_Text ui, string s)
    {
        //Add text one character at a time to create a typewriter effect

        ui.text = "";

        foreach(char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }

}