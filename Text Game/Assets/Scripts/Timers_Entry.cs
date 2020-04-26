using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timers_Entry : MonoBehaviour
{
    public Player TimersEntryPlayer;    
    public Vector3 CurrentPosition;
    public float currentValue;
    public Image[] TimerImageArray;
    public bool castMode;

    // Update is called once per frame
    void Update()
    {
        TimerImageArray = this.GetComponentsInChildren<Image>();
        TimerImageArray[1].overrideSprite = TimersEntryPlayer.PlayerPortrait;
        
        if (castMode)
        {
            TimerImageArray[2].color = Color.magenta;
        }
        else
        {
            TimerImageArray[2].color = Color.blue;
        }
    }
}
