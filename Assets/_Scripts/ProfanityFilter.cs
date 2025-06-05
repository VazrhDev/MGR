using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ProfanityFilter : MonoBehaviour
{
    [SerializeField] TextAsset forbiddenWords;

    string forbiddenWordsString = @"\b(?:";
    string[] seperators = { "\n", "," };
    [SerializeField] string[] words;

    void Start()
    {
        words = forbiddenWords.text.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

    }

    public bool FilterText(TMP_InputField inputText)
    {
        for (int i = 0; i < words.Length; i++)
        {
            if (i < words.Length - 1)
            {
                forbiddenWordsString += words[i] + "|";
            }
            else
            {
                forbiddenWordsString += words[i];
            }

        }
        forbiddenWordsString += @")\b";
        forbiddenWordsString = Regex.Replace(forbiddenWordsString, @"\t|\n|\r", "");

        bool nameNotOke = Regex.IsMatch(inputText.text.ToLower(), forbiddenWordsString.ToLower());
        forbiddenWordsString = @"\b(?:";
        return nameNotOke;
    }


}
