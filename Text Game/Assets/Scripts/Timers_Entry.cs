using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timers_Entry : MonoBehaviour
{
    public Player TimersEntryPlayer;    
    public Vector3 CurrentPosition;
    public Image[] TimerImageArray;

    // Start is called before the first frame update
    void Start()
    {
        TimerImageArray = this.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        TimerImageArray[1].overrideSprite = TimersEntryPlayer.PlayerPortrait;
    }
}
