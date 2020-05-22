using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusAilment : MonoBehaviour
{
    public Battle_Manager BM;
    public Action_Handler act_handler;
    public TextMeshPro StatusTimer;
    public float statusTimerNumber;
    public Sprite icon;
    public string type;
    public Player afflictedPlayer;
    public Enemy afflictedEnemy;    

    // Start is called before the first frame update
    void Start()
    {                
        BM = FindObjectOfType<Battle_Manager>();        
        act_handler = FindObjectOfType<Action_Handler>();
        StatusTimer = GetComponentInChildren<TextMeshPro>();
        GetComponent<SpriteRenderer>().sprite = icon;
    }


    void Update()
    {
        StatusTimer.text = statusTimerNumber.ToString();

        if (this.statusTimerNumber <= 0)
        {
            if (this.afflictedPlayer != null)
            {
                afflictedPlayer.currentAfflictions.Remove(this);
                this.afflictedPlayer = null;

            } else if (this.afflictedEnemy != null)
            {
                afflictedEnemy.currentAfflictions.Remove(this);
                this.afflictedEnemy = null;
            }

            act_handler.statusAilmentList.Remove(this);

            Destroy(this.gameObject);            
        }        
    }   
}
