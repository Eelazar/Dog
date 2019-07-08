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

    private string password;

    private TMP_Text encryptedText;
    private TMP_Text decryptedText;

    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;

    void Start()
    {
        anchorMin = decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMin;
        anchorMax = decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMax;

        decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMin = new Vector2(0.15F, 0.05F);
        decryptionPanel.transform.parent.GetComponent<RectTransform>().anchorMax = new Vector2(0.15F, 0.05F);

        encryptedText = decryptionPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        decryptedText = decryptionPanel.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    void Update()
    {
        
    }

    public void LaunchDecryption(string pass)
    {
        password = pass;

        StartCoroutine(Decryption());
    }

    IEnumerator Decryption()
    {
        int counter = 0;
        string currentChar = "";
        int length = password.Length;
        char[] passChars = password.ToCharArray();

        char[] encryptedChars = new char[encryptedTextLength];

        bool done = false;

        //Fill encrypted Text
        for(int i = 0; i < encryptedTextLength; i++)
        {
            encryptedChars[i] = System.Convert.ToChar(Random.Range(32, 127));
        }
        encryptedText.text = encryptedChars.ArrayToString();
        
        while (!done)
        {
            currentChar = passChars[counter].ToString();

            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    Debug.Log(vKey);

                    if (vKey == (KeyCode)System.Enum.Parse(typeof(KeyCode), currentChar.ToUpper()))
                    {
                        decryptedText.text += currentChar;
                        counter++;
                    }

                }
            }

            for(int i = 0; i < encryptedChars.Length; i++)
            {
                for (int j = 0; j < waveAmount; j++)
                {
                    int index = (j * (encryptedTextLength / waveAmount)) + i;
                    if (index >= encryptedChars.Length)
                    {
                        int tempI = index - encryptedChars.Length;
                        index = (j * (encryptedTextLength / waveAmount)) + tempI;
                    }
                    encryptedChars[index] = System.Convert.ToChar(Random.Range(32, 127));

                    encryptedText.text = encryptedChars.ArrayToString();

                    
                }

                yield return new WaitForSeconds(charPause);
            }
            
        }

        yield return null;
    }
}
