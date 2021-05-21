using System.Collections;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TextScript : MonoBehaviour
{
    [SerializeField] private AudioClip loseGemsSound;

    private TextMeshProUGUI textUI;
    private int health;
    private int prevHealth;
    private Transform megaGems;
    private int megaGemsTotal;
    private Transform miniGems;
    private int miniGemsTotal;
    private bool coroutineIsRunning = false;

    public int miniGemsCount = 0; // access count for end screen

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        prevHealth = GameObject.Find("FPSController").GetComponent<FirstPersonController>().m_Health;
        megaGems = GameObject.Find("Mega Gems").transform;
        megaGemsTotal = megaGems.childCount;
        miniGems = GameObject.Find("Mini Gems").transform;
        miniGemsTotal = miniGems.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        health = GameObject.Find("FPSController").GetComponent<FirstPersonController>().m_Health;
        miniGemsCount = miniGemsTotal - miniGems.childCount;

        if (health == 4 && health != prevHealth)
        {
            StartCoroutine(LoseGems());
        }
        else if (!coroutineIsRunning)
        {
            // Don't display the usual text if we're showing the gems getting lost
            textUI.SetText($"<size=100><sprite={health}></size>\n" + // sprite # corresponds to # of hearts
            $"<sprite=6> x <color=yellow>{megaGemsTotal - megaGems.childCount}/{megaGemsTotal}</color>\n" +
            $"<sprite=5> x <color=blue>{miniGemsCount}</color>");
        }

        prevHealth = health;
    }

    private IEnumerator LoseGems()
    {
        // Show losing gems on the UI after dying
        coroutineIsRunning = true;

        // If we have less than 10 gems, lose just the remaining ones
        int miniGemsLost = (miniGemsTotal - 10 - miniGems.childCount) >= 0 ? 10 : (miniGemsTotal - miniGems.childCount);
        miniGemsTotal -= miniGemsLost;
        textUI.rectTransform.position += Vector3.down * 20; // move text down to accomodate for extra space
        textUI.SetText($"<size=100><sprite={health}></size>\n" + // sprite # corresponds to # of hearts
            $"<sprite=6> x <color=yellow>{megaGemsTotal - megaGems.childCount}/{megaGemsTotal}</color>\n" +
            $"<sprite=5> x <color=blue>{miniGemsTotal - miniGems.childCount}</color>\n" +
            $"<color=red>-{miniGemsLost}</color>");

        yield return new WaitForSeconds(2);
        Vector3 playerPosition = GameObject.Find("FPSController").transform.position;
        AudioSource.PlayClipAtPoint(loseGemsSound, playerPosition);
        textUI.rectTransform.position += Vector3.up * 20; // move text back up
        coroutineIsRunning = false;
    }
}
