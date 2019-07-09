using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DecryptionSoftware : MonoBehaviour
{

    public GameObject decryptionPanel;

    public float charPause;
    public int encryptedTextLength;
    public int waveAmount;
    public int charsBeforeNextHint;

    private string password;

    private TMP_Text encryptedText;
    private TMP_Text decryptedText;

    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;

    private bool decrypting = false;
    private KeyCode lastKey;

    private int counter = 0;
    private string currentChar = "";
    private int length = 0;
    private char[] passChars;

    private string node;

    private BootManager manager;
    private BootExplorer explorer;

    void Start()
    {
        manager = transform.GetComponent<BootManager>();
        explorer = transform.GetComponent<BootExplorer>();

        anchorMin = decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMin;
        anchorMax = decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMax;

        decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMin = new Vector2(0.15F, 0.05F);
        decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMax = new Vector2(0.15F, 0.05F);

        encryptedText = decryptionPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        decryptedText = decryptionPanel.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (decrypting)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    lastKey = vKey;
                }
            }

            if (counter < passChars.Length)
            {
                currentChar = passChars[counter].ToString();
                if ((KeyCode)System.Enum.Parse(typeof(KeyCode), currentChar.ToUpper()) == lastKey)
                {
                    decryptedText.text += currentChar;
                    counter++;
                }
            }
            else
            {
                StartCoroutine(manager.CloseDecryptor());
                explorer.ApplyDecryption(node);
                decrypting = false;
            }
        }        
    }

    public void LaunchDecryption(string concernedNode, string pass)
    {
        node = concernedNode;
        password = pass;

        StartCoroutine(Decryption());
    }

    IEnumerator Decryption()
    {
        decrypting = true;

        counter = 0;
        currentChar = "";
        length = password.Length;
        passChars = password.ToCharArray();

        char[] encryptedChars = new char[encryptedTextLength];

        int currentWave = Random.Range(0, waveAmount);

        

        //Fill encrypted Text
        for(int i = 0; i < encryptedTextLength; i++)
        {
            encryptedChars[i] = System.Convert.ToChar(Random.Range(32, 127));
        }
        encryptedText.text = encryptedChars.ArrayToString();

        //Hints
        int charHintCounter = 0;
        int hintIndex = 0;

        //Update Encrypted Text
        while (decrypting)
        {
            for (int i = 0; i < encryptedTextLength / waveAmount; i++)
            {
                charHintCounter++;

                if(charHintCounter >= charsBeforeNextHint)
                {
                    charHintCounter = 0;
                    currentWave = Random.Range(0, waveAmount);
                    hintIndex = 0;
                }

                for (int j = 0; j < waveAmount; j++)
                {
                    int index = (j * (encryptedTextLength / waveAmount)) + i;
                    encryptedChars[index] = System.Convert.ToChar(Random.Range(32, 127));
                    if (char.IsWhiteSpace(encryptedChars[index]))
                    {
                        encryptedChars[index] = '@';
                    }
                    if (encryptedChars[index] == '-')
                    {
                        encryptedChars[index] = '&';
                    }

                    if(hintIndex < password.Length)
                    {
                        if(j == currentWave)
                        {
                            encryptedChars[index] = passChars[hintIndex];
                            hintIndex++;
                        }
                    }

                    encryptedText.text = encryptedChars.ArrayToString();

                    
                }

                yield return new WaitForSeconds(charPause);
            }            
        }

        yield return null;
    }
}
