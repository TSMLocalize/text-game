using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timers_Entry : MonoBehaviour
{
    public Player TimersEntryPlayer;    
    public Vector3 CurrentPosition;
    public float currentValue;
    public Image[] TimerImageArray;
    public bool castMode;
    public bool readyMode;
    public bool waitMode;
    public TextMeshProUGUI NumberText;
    public TextMeshProUGUI MainText;    

    // Update is called once per frame
    void Update()
    {        
        TimerImageArray = this.GetComponentsInChildren<Image>();
        TimerImageArray[1].overrideSprite = TimersEntryPlayer.PlayerPortrait;

        if (readyMode)
        {
            TimerImageArray[2].color = new Color32(0, 150, 0, 255);
        }        
        else if (castMode)
        {           
            TimerImageArray[2].color = Color.magenta;
        }        
        else if (waitMode)
        {            
            TimerImageArray[2].color = Color.blue;
        }        
    }
}
