using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    [SerializeField] TMP_Text eventUI;
    [SerializeField] TMP_Text eventLevelUpUI;
    [SerializeField] TMP_Text eventWeaponUnlockUI;
    [SerializeField] TMP_Text eventKillUI;
    [SerializeField] GameObject KillFeedUI;

    void EventUI(string eventText)
    {
        StartCoroutine(DisplayEvent(eventText));
    }

    IEnumerator DisplayEvent(string eventText)
    {
        if (eventText.Substring(0, 4) == "Away")
        {
            eventUI.color = new Color32(236, 110, 10, 205);
        }
        else
        {
            eventUI.color = new Color32(87, 95, 197, 205);
        }
        eventUI.text = eventText;
        yield return new WaitForSeconds(1f);
        eventUI.text = "";
    }

    void EventKillUI(string eventText)
    {
        StartCoroutine(DisplayKillEvent(eventText));
    }
    IEnumerator DisplayKillEvent(string eventText)
    {

        //eventKillUI.text = eventText;
        TMP_Text newKill = Instantiate(eventKillUI, KillFeedUI.transform);
        newKill.text = eventText;
        newKill.transform.SetAsFirstSibling();
        if (KillFeedUI.transform.childCount > 5)
        {
            if (KillFeedUI.transform.GetChild(5).gameObject)
                Destroy(KillFeedUI.transform.GetChild(5).gameObject);
        }
        yield return new WaitForSeconds(5.5f);
        if (newKill)
            Destroy(newKill);
    }
    void EventLevelUpUI(string eventText)
    {
        StartCoroutine(DisplayLevelUpEvent(eventText));
    }
    IEnumerator DisplayLevelUpEvent(string eventText)
    {

        eventLevelUpUI.text = eventText;
        yield return new WaitForSeconds(3f);
        eventLevelUpUI.text = "";
    }
    void EventWeaponUnlockUI(string eventText)
    {
        StartCoroutine(DisplayWeaponUnlockEvent(eventText));
    }
    IEnumerator DisplayWeaponUnlockEvent(string eventText)
    {

        eventWeaponUnlockUI.text = eventText;
        yield return new WaitForSeconds(3f);
        eventWeaponUnlockUI.text = "";
    }

}
