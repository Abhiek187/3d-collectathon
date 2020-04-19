using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TextScript : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private int health;
    private Transform megaGems;
    private int megaGemsTotal;
    private Transform miniGems;
    private int miniGemsTotal;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        megaGems = GameObject.Find("Mega Gems").transform;
        megaGemsTotal = megaGems.childCount;
        miniGems = GameObject.Find("Mini Gems").transform;
        miniGemsTotal = miniGems.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        health = GameObject.Find("FPSController").GetComponent<FirstPersonController>().m_Health;

        textUI.SetText($"<size=100><sprite={health}></size>\n" + // sprite # corresponds to # of hearts
            $"<sprite=6> x <color=yellow>{megaGemsTotal - megaGems.childCount}/{megaGemsTotal}</color>\n" +
            $"<sprite=5> x <color=blue>{miniGemsTotal - miniGems.childCount}</color>");
    }
}
