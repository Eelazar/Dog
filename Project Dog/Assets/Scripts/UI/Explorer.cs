using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.XPath;
using System.Xml;
using System.IO;

public class Explorer : MonoBehaviour
{
    //Constants
    private const float lineHeight = 20F;
    private const float charWidth = 11.47F;
    private string streamingPath;

    private string[] ogXMLs = {
        "ExplorerFile",
        "ThrowBrolvl1_1",
        "ThrowBrolvl1_2",
        "ThrowBrolvl1_3",
        "ThrowBroExplorerFile",
        "ThrowBroDumpSiteXML",
        "Terminal lvl1_3(roomboi)",
        "Terminal lvl1_2(ThrowBro2)",
        "Terminal lvl1_1(ThrowBro1)",
        "Terminal Conveyorbelt_3(ConveyorbeltThrowBro7-9)",
        "Terminal Conveyorbelt_2(ConveyorbeltThrowBro1-6)",
        "Terminal Conveyorbelt_1(ConveyorbeltThrowBro1-6)",
        "SaveFile",
        "RoomboiExplorerFile",
        "PlayerFoxExplorerFile",
        "PC2",
        "PC1",
        "ConveyorThrowBroExplorerFile_1",
        "ConveyorThrowBroExplorerFile_2",
        "ConveyorThrowBroExplorerFile_3",
        "ConveyorThrowBroExplorerFile_4",
        "ConveyorThrowBroExplorerFile_5",
        "ConveyorThrowBroExplorerFile_6",
        "ConveyorThrowBroExplorerFile_7",
        "ConveyorThrowBroExplorerFile_8",
        "ConveyorThrowBroExplorerFile_9",
    };

    [SerializeField]
    [Tooltip("The duration between each character being added during the typewriter animation")]
    private float textSpeed;

    public GameObject node_Prefab;
    public GameObject explorerPanel;

    public Color node_Color;
    public Color nodeMarkup_Color;
    public Color entryMarkup_Color;
    public Color entryText_Color;
    public Color lockedMarkup_Color;

    public bool bootLaunch;

    public int textFieldAmount;

    private GameObject explorerWindow;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> node_ActiveFields;
    //All the textfields from bottom to top
    private TMP_Text[] node_TextFields;

    //XML Navigation Variables
    private XPathNavigator nav;
    [HideInInspector]
    public XmlDocument tempXMLDoc;

    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;

    private string currentXMLFileName = "ExplorerFile.xml";
    [HideInInspector]
    public string currentXmlObj;

    private bool launched;

    private List<TMP_Text> encryptedFields;
    private List<TMP_Text> lockedFields;

    private TempManager tempManager;
    private UIManager manager;
    private Assistant assistant;
    private DecryptionSoftware decryptor;

    //Singleton

    public static Explorer current;

    void Start()
    {
        streamingPath = Path.Combine(Application.streamingAssetsPath, "XML\\");
        current = this;

        manager = transform.GetComponent<UIManager>();
        assistant = transform.GetComponent<Assistant>();
        decryptor = transform.GetComponent<DecryptionSoftware>();
        tempManager = transform.GetComponent<TempManager>();

        encryptedFields = new List<TMP_Text>();
        lockedFields = new List<TMP_Text>();
        explorerWindow = explorerPanel.transform.parent.gameObject;
        node_ActiveFields = new List<GameObject>();
        node_TextFields = new TMP_Text[textFieldAmount];

        anchorMin = explorerWindow.GetComponent<RectTransform>().anchorMin;
        anchorMax = explorerWindow.GetComponent<RectTransform>().anchorMax;

        GenerateTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in node_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }

        // Open the XML.
        tempXMLDoc = new XmlDocument();

        RestoreXMLs();

        //Filename Example: "ExplorerFile.xml"
        tempXMLDoc.Load(streamingPath + "TempXML\\" + ogXMLs[0] + ".xml");
        // Create a navigator to query with XPath.
        nav = tempXMLDoc.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();        

        if (bootLaunch == true)
        {
            explorerWindow.GetComponent<RectTransform>().anchorMin = new Vector2(0.11F, 0.05F);
            explorerWindow.GetComponent<RectTransform>().anchorMax = new Vector2(0.11F, 0.05F);
        }
        else
        {
            StartCoroutine(UpdateData());
        }
    }

    void Update()
    {
        if (launched)
        {
            ResizeLog();

            AnimateEncryptedNode();
        }
    }

    public IEnumerator UpdateData()
    {
        launched = true;
        bool entry = false;

        //Empty all Text Fields
        foreach (TMP_Text tmp in node_TextFields)
        {
            tmp.text = "";
        }
        encryptedFields.Clear();
        lockedFields.Clear();

        #region Parent Node Stuff
        //Save the current node
        string currentNode = nav.Name;

        //Check if the current node has a parent and move there
        if (nav.MoveToParent())
        {
            //Display the Name of the parent
            StartCoroutine(AnimateText(node_TextFields[0], "<" + nav.Name + ">"));

            yield return new WaitForSeconds(0.2F);

            //Return to the (old) current node
            nav.MoveToChild(currentNode, string.Empty);
        }
        //If there is no parent display an empty node
        else
        {
            node_TextFields[0].text = "<>";
        }
        #endregion Parent Node Stuff

        #region Current Node Stuff
        //Check if the current node has attributes
        if (nav.HasAttributes)
        {

            //Check if the current node has a description
            if (nav.GetAttribute("description", string.Empty) != "")
            {
                //Get the description
                string s = nav.GetAttribute("description", string.Empty);
                //Display the node name with description
                StartCoroutine(AnimateText(node_TextFields[1], AddColor(nodeMarkup_Color, "<" + nav.Name + ">") + AddColor(node_Color, s)));
            }
            //Check if the current node is an entry
            else if (nav.GetAttribute("entry", string.Empty) != "")
            {
                //Get the entry description
                string s = nav.GetAttribute("entry", string.Empty);

                if (s == "true")
                {
                    //Display the node name with the entry description
                    StartCoroutine(AnimateText(node_TextFields[1], AddColor(entryMarkup_Color, "<" + nav.Name + ">")));
                }
                else
                {
                    //Display the node name with the entry description
                    StartCoroutine(AnimateText(node_TextFields[1], AddColor(entryMarkup_Color, "<" + nav.Name + ">") + AddColor(node_Color, s)));
                }

                yield return new WaitForSeconds(0.2F);

                //Get the entry text
                string text = nav.Value;
                //Get the amount of chars in the text
                char[] charArray = text.ToCharArray();
                int charAmount = charArray.Length;
                //Get the width of the textField
                float fieldWidth = node_TextFields[0].GetComponent<RectTransform>().sizeDelta.x;
                //Calculate the amount of chars that fit into one field
                int maxChars = Mathf.FloorToInt(((fieldWidth - (4 * charWidth)) / charWidth));

                string[] wordArray = text.Split(' ');

                //Multiplicator for each line
                int lineCounter = 1;
                for (int i = 0; i < wordArray.Length; i++)
                {
                    //If the current line text + the next word would be too long, go down one line
                    if (node_TextFields[2 + (lineCounter - 1)].text.Length + (wordArray[i].Length) >= maxChars)
                    {
                        lineCounter++;
                    }

                    //Add the word to the field
                    node_TextFields[2 + (lineCounter - 1)].text += wordArray[i] + " ";

                    yield return null;
                }

                entry = true;
            }
            else
            {
                //Display the node name
                StartCoroutine(AnimateText(node_TextFields[1], AddColor(nodeMarkup_Color, "<" + nav.Name + ">")));
            }
        }
        else
        {
            //Display the node name
            StartCoroutine(AnimateText(node_TextFields[1], AddColor(nodeMarkup_Color, "<" + nav.Name + ">")));
        }

        yield return new WaitForSeconds(0.2F);
        #endregion Current Node Stuff

        #region Child Node Stuff
        //Check if the current node has children
        if (nav.HasChildren && entry == false)
        {
            //Get the amount of children
            int childCount = nav.SelectChildren(XPathNodeType.All).Count;
            //Move to the first child
            nav.MoveToFirstChild();

            //For each child
            for (int i = 0; i < childCount; i++)
            {
                //Check if the child node is hidden
                if (nav.GetAttribute("status", string.Empty) == "hidden")
                {

                }
                //Check if the child node is encrypted
                else if (nav.GetAttribute("encrypted", string.Empty) != "" && nav.GetAttribute("encrypted", string.Empty) != "decrypted")
                {
                    //Display the child node name with entry description
                    StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">")));

                    //Add the node to the array for animation
                    encryptedFields.Add(node_TextFields[i + 2]);
                }
                //Check if the child node is locked
                else if (nav.GetAttribute("locked", string.Empty) != "" && nav.GetAttribute("locked", string.Empty) != "false")
                {
                    //Display the child node name with entry description
                    StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(lockedMarkup_Color, "<" + nav.Name + ">")));

                    //Add the node to the array
                    lockedFields.Add(node_TextFields[i + 2]);
                }
                //Check if the child node is an entry
                else if (nav.GetAttribute("entry", string.Empty) != "")
                {
                    //Get the entry description
                    string s = nav.GetAttribute("entry", string.Empty);
                    if (s == "true")
                    {
                        //Display the child node name
                        StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">")));
                    }
                    else
                    {
                        //Display the child node name with entry description
                        StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">") + AddColor(node_Color, s)));
                    }
                }
                else
                {
                    //Display the child node name
                    StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(nodeMarkup_Color, "<" + nav.Name + ">")));
                }

                yield return new WaitForSeconds(0.1F);

                //Move to the next sibling
                nav.MoveToNext();
            }

            //Return to the parent node
            nav.MoveToParent();
        }
        #endregion Child Node Stuff
    }

    public void SwitchXML(string fileName, string objName)
    {
        tempXMLDoc.Save(streamingPath + currentXMLFileName);
        currentXMLFileName = fileName;
        currentXmlObj = objName;

        //Filename Example: "ExplorerFile.xml"
        tempXMLDoc.Load(streamingPath + currentXMLFileName);
        // Create a navigator to query with XPath.
        nav = tempXMLDoc.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();
        nav.MoveToFirstChild();

        if (nav.HasAttributes)
        {
            //Check if the current node has an assistant message
            if (nav.GetAttribute("assistant", string.Empty) != "")
            {
                string text = nav.GetAttribute("assistant", string.Empty);
                string flag = nav.GetAttribute("assistantFlag", string.Empty);

                nav.MoveToAttribute("assistantFlag", string.Empty);
                Message m = null;

                switch (flag)
                {
                    case "always":
                        m = new Message(text, 0, 4, true);
                        assistant.QueueMessage(m);
                        break;
                    case "false":
                        m = new Message(text, 0, 4, true);
                        assistant.QueueMessage(m);
                        nav.SetValue("true");
                        break;
                    case "true":
                        break;

                    default:
                        break;
                }

                nav.MoveToParent();
            }
        }

        LaunchUpdate();
    }

    #region Commands
    public void ExitUnitXML()
    {
        SwitchXML("TempXML\\" + ogXMLs[0] + ".xml", "system");
    }

    public void LaunchUpdate()
    {
        StartCoroutine(UpdateData());
    }

    public void ApplyDecryption(string node)
    {
        if (launched)
        {
            if (nav.MoveToChild(node, string.Empty) == true)
            {
                nav.MoveToAttribute("encrypted", string.Empty);
                nav.SetValue("decrypted");

                nav.MoveToParent();
                nav.MoveToAttribute("assistantFlag", string.Empty);
                nav.SetValue("true");

                nav.MoveToParent();
                nav.MoveToParent();
                StartCoroutine(UpdateData());
            }
        }
    }

    public void DecryptNode()
    {
        if (launched)
        {
            if (nav.HasChildren)
            {
                //Get the amount of children
                int childCount = nav.SelectChildren(XPathNodeType.All).Count;
                //Move to the first child
                nav.MoveToFirstChild();

                //For each child
                for (int i = 0; i < childCount; i++)
                {
                    //Check if the child node is hidden
                    if (nav.GetAttribute("encrypted", string.Empty) != "" && nav.GetAttribute("encrypted", string.Empty) != "decrypted")
                    {
                        if (bootLaunch)
                        {
                            StartCoroutine(manager.LaunchDecryptor());
                        }
                        else
                        {
                            StartCoroutine(tempManager.LaunchDecryptor());
                        }

                        string password = nav.GetAttribute("encrypted", string.Empty);

                        decryptor.LaunchDecryption(nav.Name, password);

                        nav.MoveToParent();
                    }

                    nav.MoveToNext();
                }
            }
        }
    }

    public void UnlockNode(CommandContext cc)
    {
        string node = cc.parameters[0].ToString();
        string password = cc.parameters[1].ToString();

        nav.MoveToChild(node, string.Empty);

        if (nav.HasAttributes)
        {
            if(nav.GetAttribute("locked", string.Empty) != "" && nav.GetAttribute("locked", string.Empty) != "false")
            {
                string xmlPassword = (nav.GetAttribute("locked", string.Empty));

                if(xmlPassword == password)
                {
                    nav.MoveToAttribute("locked", string.Empty);
                    nav.SetValue("false");
                    nav.MoveToParent();
                    nav.MoveToParent();

                    LaunchUpdate();
                    return;
                }
            }
        }
        nav.MoveToParent();
    }

    public void Interact(CommandContext cc)
    {
        string node = cc.parameters[0].ToString();

        if (launched)
        {
            if (!nav.HasChildren)
            {
                //StartCoroutine(UpdateData());
            }
            else if (nav.HasChildren)
            {
                nav.MoveToChild(node, string.Empty);

                if (!nav.HasAttributes)
                {
                    StartCoroutine(UpdateData());
                }
                else if (nav.HasAttributes)
                {
                    if (nav.GetAttribute("status", string.Empty) == "hidden")
                    {
                        nav.MoveToParent();
                        return;
                    }

                    //Check if the current node has an assistant message
                    if (nav.GetAttribute("assistant", string.Empty) != "")
                    {
                        string text = nav.GetAttribute("assistant", string.Empty);
                        string flag = nav.GetAttribute("assistantFlag", string.Empty);

                        nav.MoveToAttribute("assistantFlag", string.Empty);
                        Message m = null;

                        switch (flag)
                        {
                            case "always":
                                m = new Message(text, 0, 4, true);
                                assistant.QueueMessage(m);
                                break;
                            case "false":
                                m = new Message(text, 0, 4, true);
                                assistant.QueueMessage(m);
                                nav.SetValue("true");
                                break;
                            case "true":
                                break;

                            default:
                                break;
                        }

                        nav.MoveToParent();
                    }

                    if (nav.GetAttribute("locked", string.Empty) != "" && nav.GetAttribute("locked", string.Empty) != "false")
                    {
                        nav.MoveToParent();
                        return;
                    }

                    if (nav.GetAttribute("encrypted", string.Empty) != "" && nav.GetAttribute("encrypted", string.Empty) != "decrypted")
                    {
                        nav.MoveToParent();
                        return;
                    }

                    StartCoroutine(UpdateData());
                }
            }
        }
    }

    public void NavigateUp(CommandContext cc)
    {
        int amount = int.Parse(cc.parameters[0].ToString());

        if (launched)
        {
            for (int i = 0; i < amount; i++)
            {
                nav.MoveToParent();
            }

            StartCoroutine(UpdateData());
        }
    }

    public void Save()
    {
        for (int i = 0; i < ogXMLs.Length; i++)
        {
            tempXMLDoc.Save(streamingPath + "TempXML\\" + ogXMLs[0] + ".xml");
        }
    }

    public void XMLTrigger(CommandContext cc)
    {
        string node = cc.parameters[0].ToString();

        if (nav.HasChildren)
        {
            if(nav.MoveToChild(node, string.Empty))
            {
                if(nav.GetAttribute("trigger", string.Empty) != "")
                {
                    string method = nav.GetAttribute("trigger", string.Empty);

                    BaseObject baseObject;

                    ObjectManager.GetObject(currentXmlObj, out baseObject);

                    if (baseObject != null)
                    {
                        baseObject.SendMessage(method, new CommandContext());
                    }
                        
                }

                nav.MoveToParent();
                LaunchUpdate();
            }
        }
    }
    #endregion Commands

    #region Utility
    public void RestoreXMLs()
    {
        for (int i = 0; i < ogXMLs.Length; i++)
        {
            tempXMLDoc.Load(streamingPath + ogXMLs[i] + ".xml");
            tempXMLDoc.Save(streamingPath + "TempXML\\" + ogXMLs[i] + ".xml");
        }
    }

    void GenerateTextFieldArray()
    {
        node_TextFields[0] = explorerPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        node_TextFields[1] = explorerPanel.transform.GetChild(1).GetComponent<TMP_Text>();

        for (int i = 2; i < textFieldAmount; i++)
        {
            GameObject go = Instantiate<GameObject>(node_Prefab);
            go.transform.SetParent(explorerPanel.transform, false);
            go.name = "Node TextField " + (i + 1);
            node_TextFields[i] = go.GetComponent<TMP_Text>();
        }
    }

    void ResizeLog()
    {
        //Measure how many pixels are unused (e.g. the top part of the window -> decoration)
        float yOffset = explorerPanel.GetComponent<RectTransform>().sizeDelta.y;

        //Get the anchors of the explorer window, i.e. from where to where on the screen it spans
        Vector2 parentAnchorMin = new Vector2(explorerWindow.GetComponent<RectTransform>().anchorMin.x, explorerWindow.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(explorerWindow.GetComponent<RectTransform>().anchorMax.x, explorerWindow.GetComponent<RectTransform>().anchorMax.y);

        //From the anchors get the percentage of screen the explorer window occupies
        Vector2 parentScreenPercent = new Vector2(parentAnchorMax.x - parentAnchorMin.x, parentAnchorMax.y - parentAnchorMin.y);

        //Get the height in pixels of the entire game window
        float canvasHeight = Screen.height;

        //Calculate the actual height of the explorer by multiplying the screen height by the window percentage, and finally substracting the unused space
        float explorerSize = (canvasHeight * parentScreenPercent.y) + yOffset;

        //Calculate the highest possible amount of slots that can fit into the explorer
        int slotAmount = Mathf.FloorToInt(explorerSize / (lineHeight + explorerPanel.GetComponent<VerticalLayoutGroup>().spacing));

        //Check how many slots are currently active
        int activeAmount = node_ActiveFields.Count;

        //Debug.Log("Pixel Height: " + canvasHeight + ", Explorer Window Size: " + explorerSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);

        if (activeAmount != slotAmount)
        {
            //If more slots could fit into the log, add them
            if (activeAmount < slotAmount)
            {
                for (int i = activeAmount; i < slotAmount; i++)
                {
                    node_TextFields[i].gameObject.SetActive(true);
                    node_ActiveFields.Add(node_TextFields[i].gameObject);
                }
            }
            //Otherwise remove the excess slots
            else if (activeAmount > slotAmount)
            {
                for (int i = activeAmount; i > slotAmount; i--)
                {
                    node_TextFields[i - 1].gameObject.SetActive(false);
                    node_ActiveFields.Remove(node_TextFields[i - 1].gameObject);
                }

                //Cleanup
                for (int i = activeAmount; i < node_TextFields.Length; i++)
                {
                    node_TextFields[i].gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator AnimateText(TMP_Text ui, string s)
    {
        var wait = new WaitForSeconds(textSpeed);

        ui.text = s;
        ui.maxVisibleCharacters = 0;

        yield return new WaitForEndOfFrame();

        int i = 0;
        char[] charArray = s.ToCharArray();
        while (i < charArray.Length)
        {
            ui.maxVisibleCharacters = i + 1;
            i++;
            yield return wait;
        }
    }

    public void AnimateEncryptedNode()
    {
        if (encryptedFields.Count > 0)
        {
            foreach (TMP_Text tmp in encryptedFields)
            {
                char[] chars = tmp.text.ToCharArray();
                int modulo = Random.Range(1, 5);

                for (int i = 0; i < chars.Length; i++)
                {
                    if (i % modulo == 0)
                    {
                        chars[i] = System.Convert.ToChar(Random.Range(32, 127));
                        tmp.text = new string(chars);
                    }
                }
            }
        }
    }

    private string AddColor(Color c, string s)
    {
        string final = "";

        final += "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        final += s;
        final += "</color>";

        return final;
    }
    #endregion Utility
}
