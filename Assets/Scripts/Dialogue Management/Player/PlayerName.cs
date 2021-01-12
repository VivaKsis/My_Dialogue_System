using TMPro;
using UnityEngine;

public static class InsertPlayerName
{
    public static string playerName;

    private const string KEY_TO_REPLACE = "{playerName}";

    private static bool IsKey(string sentence, int braceIndex)
    {
        for (int a = braceIndex, keyIndex = 0; keyIndex < KEY_TO_REPLACE.Length; a++, keyIndex++)
        {
            if (sentence[a] != KEY_TO_REPLACE[keyIndex])
            {
                return false;
            }
        }
        return true;
    }

    public static string InsertPlayerNameIntoCharacterSentence(string sentence)
    {
        string finalSentence = "";

        for (int a = 0; a < sentence.Length; a++)
        {
            if (sentence[a] == '{')
            {
                if (IsKey(sentence, a))
                {
                    for (int nameIndex = 0; nameIndex < playerName.Length; nameIndex++)
                    {
                            finalSentence += playerName[nameIndex];
                    }
                    while (sentence[a] != '}')
                    {
                        a++;
                    }
                    a++;
                    if (a >= sentence.Length)
                    {
                        break;
                    }
                }
            }
            finalSentence += sentence[a];
        }

        return finalSentence;
    }
}
