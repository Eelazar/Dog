using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordDictionary : MonoBehaviour
{
    public static KeywordDictionary current;

    public Sentence[] sentences;

    public void Awake()
    {
        current = this;
    }

}
