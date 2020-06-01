using System;
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
    public List<GameObject> TimerList;    
    public List<Vector3> TimerListPositions;
    public List<Player> PlayerSpeeds;
    public GameObject TimersPanel;
    public List<GameObject> countOrdered;
    public Image[] TimerImageArray;
    public TextMeshProUGUI[] instantiatedTimersTextArray;
    public Timers_Entry pfTimer_Entry;    
    public bool battleStart = true;

    public List<Vector3> ListPositions;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    void Update()
    {
        if (battleStart == true)
        {            
            for (int i = 0; i < BM.PlayersInBattle.Count; i++)
            {
                pfTimer_Entry.TimersEntryPlayer = BM.PlayersInBattle[i];
                BM.PlayersInBattle[i].playerTimersEntry = Instantiate(pfTimer_Entry.gameObject, TimersPanel.transform);
                TimerList.Add(BM.PlayersInBattle[i].playerTimersEntry);                
            }
            
            battleStart = false;
        }

        updatePositions();
    }

    //This updates the position and color of each TimersEntry every frame
    public void updatePositions()
    {        
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().readyMode = false;
            BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().waitMode = false;
            BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().castMode = false;


            if (BM.PlayersInBattle[i].isAsleep)
            {
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().NumberText.text = "ZZZ";
            }
            else if (BM.ActivePlayers.Contains(BM.PlayersInBattle[i]))
            {
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().currentValue = 0;                 
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().NumberText.text = "--";
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().MainText.text = "READY!";
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().readyMode = true;
            }             
            else if (BM.PlayersInBattle[i].isCastingSpell)
            {
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().currentValue =
                    (BM.PlayersInBattle[i].castSpeedTotal / BM.PlayersInBattle[i].castSpeed);
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().castMode = true;                
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().NumberText.text = 
                    "@" + Mathf.RoundToInt(BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().currentValue);
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().MainText.text = "Spell Ready:";
            }
            else
            {                
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().currentValue = ((100 - BM.PlayersInBattle[i].speedTotal) / BM.PlayersInBattle[i].speed);
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().waitMode = true;                
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().NumberText.text = 
                    "@" + Mathf.RoundToInt(BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().currentValue);
                BM.PlayersInBattle[i].playerTimersEntry.GetComponent<Timers_Entry>().MainText.text = "Next Turn:";
            }
        }      

        PlayerSpeeds = BM.PlayersInBattle.OrderByDescending(go => go.playerTimersEntry.GetComponent<Timers_Entry>().currentValue).ToList();

        for (int i = 0; i < PlayerSpeeds.Count; i++)
        {
            PlayerSpeeds[i].playerTimersEntry.transform.SetSiblingIndex(i);
        }

        Canvas.ForceUpdateCanvases();
    }
}
