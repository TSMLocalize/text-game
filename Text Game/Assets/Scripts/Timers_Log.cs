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

    public GameObject pfTimer;
    public List<GameObject> TimerList;
    public List<Vector3> TimerListPositions;
    public GameObject TimersPanel;
    public Image[] TimersImageArray;
    public TextMeshProUGUI[] instantiatedTimersTextArray;

    public List<Vector3> ListPositions;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();

    }

    void Update()
    {
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            TimersImageArray = BM.PlayersInBattle[i].playerTimersEntry.GetComponentsInChildren<Image>();
            TimersImageArray[1].overrideSprite = BM.PlayersInBattle[i].PlayerPortrait;
            //TimerList = TimerList.OrderByDescending(s => s.);
        }

        updateTimers();
    }

    public void updateTimers()
    {
        TimerListPositions.Clear();

        for (int i = 0; i < TimerList.Count; i++)
        {
            TimerListPositions.Add(TimerList[i].transform.position);
        }
    }

    public class TimerEntry {
    //This needs to be a class so we can work with it properly
    }

    /*
    public void addToTimersLog()
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
        } else if (player != null && player.isCastingSpell == false)
        {
            newTimersEntry.timeTilTurn = Mathf.CeilToInt((100 - player.speedTotal) / player.speed);
        }                   
                
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
            else if (instantiatedTimersOptions[i].player != null && instantiatedTimersOptions[i].player.isCastingSpell == false)
            {
                instantiatedTimersOptions[i].timeTilTurn = Mathf.CeilToInt((100 - instantiatedTimersOptions[i].player.speedTotal) / instantiatedTimersOptions[i].player.speed);

                instantiatedTimersImageArray[1].overrideSprite = instantiatedTimersOptions[i].player.PlayerPortrait;

                instantiatedTimersTextArray[0].text =
                    instantiatedTimersOptions[i].player.name + "\n" + " @" + instantiatedTimersOptions[i].timeTilTurn;

                instantiatedTimersImageArray[2].color = Color.blue;

                instantiatedTimersImageArray[2].transform.localScale =
                    new Vector3(Mathf.Clamp((((100 - instantiatedTimersOptions[i].player.speedTotal) / instantiatedTimersOptions[i].player.speed) / 10), 0, 1),
                        instantiatedTimersImageArray[2].transform.localScale.y, instantiatedTimersImageArray[2].transform.localScale.z);
            }
            else
            {
                Destroy(instantiatedTimersOptions[i].timersEntry);
                instantiatedTimersOptions.Remove(instantiatedTimersOptions[i]);
            }
        }
    }
    */
}
