using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private int health = 4;
    private Transform megaGems;
    private int megaGemsTotal;
    private int miniGems = 0;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        megaGems = GameObject.Find("Mega Gems").transform;
        megaGemsTotal = megaGems.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        textUI.SetText($"<size=100><sprite={health}></size>\n" + // sprite # corresponds to # of hearts
            $"<sprite=6> x <color=yellow>{megaGemsTotal - megaGems.childCount}/{megaGemsTotal}</color>\n" +
            $"<sprite=5> x <color=blue>0</color>");
    }
}
