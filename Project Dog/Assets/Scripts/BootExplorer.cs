using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.XPath;
using System.Xml;

public class BootExplorer : MonoBehaviour
{
    private const float lineHeight = 20F;
    private const float charWidth = 11.47F;

    public bool tutorialLaunch;
    [SerializeField]
    [Tooltip("The duration between each character being added during the typewriter animation")]
    private float textSpeed;

    public GameObject node_Prefab;
    public GameObject explorerPanel;

    public Color node_Color;
    public Color nodeMarkup_Color;
    public Color entryMarkup_Color;
    public Color entryText_Color;

    public int textFieldAmount;

    private GameObject explorerWindow;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> node_ActiveFields;
    //All the textfields from bottom to top
    private TMP_Text[] node_TextFields;

    //XML Navigation Variables
    private XPathNavigator nav;
    private XPathDocument docNav;

    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;

    private bool launched;

    private List<TMP_Text> encryptedFields;
    private List<TMP_Text> lockedFields;

    private BootManager manager;
    private Assistant assistant;

    void Start()
    {
        manager = transform.GetComponent<BootManager>();
        assistant = transform.GetComponent<Assistant>();
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
        docNav = new XPathDocument("Assets\\Scripts\\ExplorerFile.xml");
        // Create a navigator to query with XPath.
        nav = docNav.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();
        ////Move to first Child
        //nav.MoveToFirstChild();

        if (!tutorialLaunch)
        {
            //Initial Update
            StartCoroutine(UpdateData());
        }
        else
        {
            explorerWindow.GetComponent<RectTransform>().anchorMin = new Vector2(0.11F, 0.05F);
            explorerWindow.GetComponent<RectTransform>().anchorMax = new Vector2(0.11F, 0.05F);
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

        Debug.Log("Pixel Height: " + canvasHeight + ", Explorer Window Size: " + explorerSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);

        if(activeAmount != slotAmount)
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

    private string AddColor(Color c, string s)
    {
        string final = "";

        final += "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        final += s;
        final += "</color>";

        return final;
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
        else node_TextFields[0].text = "<>";
  
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
                //Display the node name with the entry description
                StartCoroutine(AnimateText(node_TextFields[1], AddColor(entryMarkup_Color, "<" + nav.Name + ">") + AddColor(node_Color, s)));

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
                    if(node_TextFields[2 + (lineCounter - 1)].text.Length + (wordArray[i].Length) >= maxChars)
                    {
                        lineCounter++;
                    }

                    //Add the word to the field
                    node_TextFields[2 + (lineCounter - 1)].text += wordArray[i] + " ";

                    yield return null;
                }

                entry = true;
            }
        }
        else
        {
            //Display the node name
            StartCoroutine(AnimateText(node_TextFields[1], AddColor(nodeMarkup_Color, "<" + nav.Name + ">")));
        }
        
        yield return new WaitForSeconds(0.2F);

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
                //Check if the child node is secured
                else if (nav.GetAttribute("security", string.Empty) != "")
                {                    
                    //Check if the child node is encrypted
                    if (nav.GetAttribute("security", string.Empty) == "encrypted")
                    {
                        //Display the child node name with entry description
                        StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">")));

                        //Add the node to the array for animation
                        encryptedFields.Add(node_TextFields[i + 2]);
                    }
                    //Check if the child node is locked
                    else if (nav.GetAttribute("security", string.Empty) == "locked")
                    {
                        //Display the child node name with entry description
                        StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">")));

                        //Add the node to the array
                        lockedFields.Add(node_TextFields[i + 2]);
                    }                    
                }
                //Check if the child node is an entry
                else if (nav.GetAttribute("entry", string.Empty) != "")
                {
                    //Get the entry description
                    string s = nav.GetAttribute("entry", string.Empty);
                    //Display the child node name with entry description
                    StartCoroutine(AnimateText(node_TextFields[i + 2], AddColor(entryMarkup_Color, "<" + nav.Name + ">") + AddColor(node_Color, s)));
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
    }
    
    public void AnimateEncryptedNode()
    {
        if(encryptedFields.Count > 0)
        {
            foreach(TMP_Text tmp in encryptedFields)
            {
                char[] chars = tmp.text.ToCharArray();
                int modulo = Random.Range(1, 5);

                for (int i = 0; i < chars.Length; i++)
                {
                    if(i % modulo == 0)
                    {
                        chars[i] = System.Convert.ToChar(Random.Range(32, 127));
                        tmp.text = new string(chars);
                    }
                }
            }
        }
    }

    public void NavigateDown(string node)
    {
        if (launched)
        {
            if (nav.HasChildren)
            {
                nav.MoveToChild(node, string.Empty);

                if (nav.HasAttributes)
                {
                    if (nav.GetAttribute("status", string.Empty) == "hidden")
                    {
                        nav.MoveToParent();
                        return;
                    }
                    else if (nav.GetAttribute("security", string.Empty) == "encrypted")
                    {
                        nav.MoveToParent();
                        StartCoroutine(assistant.DisplayMessage("This node is encrypted", 0.1F));
                        StartCoroutine(assistant.HideMessage(1F));
                        return;
                    }
                    else if (nav.GetAttribute("security", string.Empty) == "locked")
                    {
                        nav.MoveToParent();
                        StartCoroutine(assistant.DisplayMessage("This node is locked", 0.1F));
                        StartCoroutine(assistant.HideMessage(1F));
                        return;
                    }
                }
            }

            StartCoroutine(UpdateData());
        }
    }

    public void NavigateUp(int amount)
    {
        if (launched)
        {
            for (int i = 0; i < amount; i++)
            {
                nav.MoveToParent();
            }

            StartCoroutine(UpdateData());
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
            go.name = "Node TextField " + (i +  1);
            node_TextFields[i] = go.GetComponent<TMP_Text>();
        }
    }

    private IEnumerator AnimateText(TMP_Text ui, string s)
    {
        //Add text one character at a time to create a typewriter effect

        ui.text = "";

        foreach (char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }
}
