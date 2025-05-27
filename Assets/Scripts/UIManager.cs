using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public const string maiBreak = "<br>";
    public const string bulletPoint = "\u2022";

    public static UIManager singleton;

    public TextMeshProUGUI killText;

    private void Awake() => singleton = this;

    public void ShowKills(string info)
    {
        
        killText.text = info;
    }
}
