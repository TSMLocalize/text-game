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

    public List<Vector3> ListPositions;

    // Start is called before the first frame update
    void Start()
    {        
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    [System.Serializable]
    public class TimersEntry
    {
        public Enemy enemy;
        public Player player;
        public GameObject timersEntry;
        public float timeTilTurn;        
    }

    public void updateTimersLog()
    {
        instantiatedTimersOptions = instantiatedTimersOptions.OrderByDescending(instantiatedTimersOption => Mathf.CeilToInt(instantiatedTimersOption.timeTilTurn)).ToList();

        for (int i = 0; i < instantiatedTimersOptions.Count; i++)
        {
            Destroy(instantiatedTimersOptions[i].timersEntry);
            instantiatedTimersOptions[i].timersEntry = Instantiate(pfTimersOption, TimersOptionPanel.transform);            
            instantiatedTimersImageArray = instantiatedTimersOptions[i].timersEntry.GetComponentsInChildren<Image>();
            instantiatedTimersTextArray = instantiatedTimersOptions[i].timersEntry.GetComponentsInChildren<TextMeshProUGUI>();

            if (instantiatedTimersOptions[i].player != null && instantiatedTimersOptions[i].player.isCastingSpell)
            {
                instantiatedTimersOptions[i].timeTilTurn = Mathf.CeilToInt(instantiatedTimersOptions[i].player.castSpeedTotal / instantiatedTimersOptions[i].player.castSpeed);

                instantiatedTimersImageArray[1].overrideSprite = instantiatedTimersOptions[i].player.PlayerPortrait;

                instantiatedTimersTextArray[0].text =
                    instantiatedTimersOptions[i].player.name + "\n" +
                    instantiatedTimersOptions[i].player.activeSpell.name + " @" + instantiatedTimersOptions[i].timeTilTurn;

                instantiatedTimersImageArray[2].transform.localScale =
                    new Vector3(Mathf.Clamp((instantiatedTimersOptions[i].player.castSpeedTotal / instantiatedTimersOptions[i].player.activeSpell.castTime), 0, 1),
                        instantiatedTimersImageArray[2].transform.localScale.y, instantiatedTimersImageArray[2].transform.localScale.z);
            }
            else if (instantiatedTimersOptions[i].enemy != null && instantiatedTimersOptions[i].enemy.isCastingSpell)
            {
                instantiatedTimersOptions[i].timeTilTurn = Mathf.CeilToInt(instantiatedTimersOptions[i].enemy.castSpeedTotal / instantiatedTimersOptions[i].enemy.castSpeed);

                instantiatedTimersImageArray[1].overrideSprite = instantiatedTimersOptions[i].enemy.EnemyPortrait;

                instantiatedTimersTextArray[0].text =
                    instantiatedTimersOptions[i].enemy.EnemyName + "\n" +
                    instantiatedTimersOptions[i].enemy.activeSpell.name + " @" + instantiatedTimersOptions[i].timeTilTurn;

                instantiatedTimersImageArray[2].transform.localScale =
                    new Vector3(Mathf.Clamp((instantiatedTimersOptions[i].enemy.castSpeedTotal / instantiatedTimersOptions[i].enemy.activeSpell.castTime), 0, 1),
                        instantiatedTimersImageArray[2].transform.localScale.y, instantiatedTimersImageArray[2].transform.localScale.z);
            } 
            else
            {
                Destroy(instantiatedTimersOptions[i].timersEntry);
                instantiatedTimersOptions.Remove(instantiatedTimersOptions[i]);
            }
        }
    }

    public void addToTimersLog(Player player = null, Enemy enemy = null)
    {
        TimersEntry newTimersEntry = new TimersEntry();
        newTimersEntry.player = player;
        newTimersEntry.enemy = enemy;

        if (player != null && player.isCastingSpell)
        {
            newTimersEntry.timeTilTurn = Mathf.CeilToInt(player.castSpeedTotal / player.castSpeed);
        } else if (enemy != null && enemy.isCastingSpell)
        {
            newTimersEntry.timeTilTurn = Mathf.CeilToInt(enemy.castSpeedTotal / enemy.castSpeed);
        }                       
        instantiatedTimersOptions.Add(newTimersEntry);        
    }
}
