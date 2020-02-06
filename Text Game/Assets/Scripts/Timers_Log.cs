using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Timers_Log : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;

    public GameObject pfTimersOption;
    public GameObject instantiatedTimersOption;
    public List<TimersEntry> instantiatedTimersOptions;
    public GameObject TimersOptionPanel;
    public Image[] instantiatedTimersImageArray;
    public TextMeshProUGUI[] instantiatedTimersTextArray;

    // Start is called before the first frame update
    void Start()
    {        
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    [System.Serializable]
    public class TimersEntry
    {
        public Player player;
        public GameObject timersEntry;
    }

    public void updateTimersLog()
    {
        instantiatedTimersOptions = instantiatedTimersOptions.OrderByDescending(instantiatedTimersOption => Mathf.CeilToInt(instantiatedTimersOption.player.castSpeedTotal / instantiatedTimersOption.player.castSpeed)).ToList();

        for (int i = 0; i < instantiatedTimersOptions.Count; i++)
        {
            Destroy(instantiatedTimersOptions[i].timersEntry);

            instantiatedTimersOptions[i].timersEntry = Instantiate(pfTimersOption, TimersOptionPanel.transform);
            instantiatedTimersImageArray = instantiatedTimersOptions[i].timersEntry.GetComponentsInChildren<Image>();
            instantiatedTimersTextArray = instantiatedTimersOptions[i].timersEntry.GetComponentsInChildren<TextMeshProUGUI>();

            if (instantiatedTimersOptions[i].player.isCastingSpell == true)
            {
                int toNextTurn = Mathf.CeilToInt(instantiatedTimersOptions[i].player.castSpeedTotal / instantiatedTimersOptions[i].player.castSpeed);

                instantiatedTimersImageArray[1].overrideSprite = instantiatedTimersOptions[i].player.PlayerPortrait;

                instantiatedTimersTextArray[0].text =
                    instantiatedTimersOptions[i].player.name + "\n" +
                    instantiatedTimersOptions[i].player.activeSpell.name + " @" + toNextTurn;

                instantiatedTimersImageArray[2].transform.localScale =
                    new Vector3(Mathf.Clamp((instantiatedTimersOptions[i].player.castSpeedTotal / instantiatedTimersOptions[i].player.activeSpell.castTime), 0, 1),
                        instantiatedTimersOptions[i].player.playerCastBarFill.GetComponent<Image>().transform.localScale.y,
                        instantiatedTimersOptions[i].player.playerCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                Destroy(instantiatedTimersOptions[i].timersEntry);
                instantiatedTimersOptions.Remove(instantiatedTimersOptions[i]);
            }
        }
    }

    public void addToTimersLog(Player player = null)
    {
        TimersEntry newTimersEntry = new TimersEntry();
        newTimersEntry.player = player;
        instantiatedTimersOptions.Add(newTimersEntry);
    }
}
