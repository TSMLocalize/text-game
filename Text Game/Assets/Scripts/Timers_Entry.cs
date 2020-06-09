using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timers_Entry : MonoBehaviour
{
    public Player TimersEntryPlayer;
    public Enemy TimersEntryEnemy;
    public Vector3 CurrentPosition;
    public float currentValue;
    public Image[] TimerImageArray;
    public bool castMode;
    public bool readyMode;
    public bool waitMode;
    public TextMeshProUGUI NumberText;
    public TextMeshProUGUI MainText;
    public bool isPlayer;
    public bool isEnemy;

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            TimerImageArray = this.GetComponentsInChildren<Image>();
            TimerImageArray[1].overrideSprite = TimersEntryPlayer.PlayerPortrait;

            if (readyMode)
            {
                TimerImageArray[0].color = new Color32(0, 150, 0, 255);
            }
            else if (castMode)
            {
                TimerImageArray[0].color = Color.magenta;
            }
            else if (waitMode)
            {
                TimerImageArray[0].color = Color.blue;
            }
        } 
        
        if (isEnemy)
        {
            TimerImageArray = this.GetComponentsInChildren<Image>();
            TimerImageArray[1].overrideSprite = TimersEntryEnemy.EnemyPortrait;

            if (readyMode)
            {
                TimerImageArray[0].color = new Color32(204, 108, 0, 255);
            }
            else if (castMode)
            {
                TimerImageArray[0].color = new Color32(92, 21, 136, 255);
            }
            else if (waitMode)
            {
                TimerImageArray[0].color = new Color32(147, 0, 0, 255);
            }
        }  
    }
}
