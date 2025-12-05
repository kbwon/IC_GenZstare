using UnityEngine;

[System.Serializable]
public class SessionReport
{
    public string userId;
    public string track;     // "A" or "B"

    public bool greeted;
    public int goodChoices;
    public int neutralChoices;
    public int badChoices;
    public bool askedForHelp;
}

