using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusAilment : MonoBehaviour
{
    public Battle_Manager BM;
    public Animation_Handler animHandler;
    public Action_Handler act_handler;
    public TextMeshPro StatusTimer;
    public float statusTimerNumber;
    public Sprite icon;
    public string type;
    public Player afflictedPlayer;
    public Enemy afflictedEnemy;
    public string playerorenemy;
    public string associatedAnimationState;
    

    // Start is called before the first frame update
    void Start()
    {                
        BM = FindObjectOfType<Battle_Manager>();
        animHandler = FindObjectOfType<Animation_Handler>();
        act_handler = FindObjectOfType<Action_Handler>();
        StatusTimer = GetComponentInChildren<TextMeshPro>();
        GetComponent<SpriteRenderer>().sprite = icon;
    }

    void Update()
    {
        StatusTimer.text = statusTimerNumber.ToString();

        //Scripts for what to do when applied status hits 0
        if (this.statusTimerNumber <= 0)
        {            
            if (playerorenemy == "player")
            {
                if(type == "Sleep")
                {
                    afflictedPlayer.isAsleep = false;
                    afflictedPlayer.speed = afflictedPlayer.preDebuffSpeed;
                }

                afflictedPlayer.constantAnimationStates.Remove(associatedAnimationState);
                animHandler.animationController(afflictedPlayer);
                afflictedPlayer.currentAfflictions.Remove(this);
                this.afflictedPlayer = null;

            } 
            
            if (playerorenemy == "enemy")
            {
                if (type == "Sleep")
                {
                    afflictedEnemy.isAsleep = false;
                    afflictedEnemy.speed = afflictedEnemy.preDebuffSpeed;
                }

                afflictedEnemy.constantAnimationStates.Remove(associatedAnimationState);
                animHandler.enemyAnimationController(afflictedEnemy);
                afflictedEnemy.currentAfflictions.Remove(this);
                this.afflictedEnemy = null;
            }

            act_handler.statusAilmentList.Remove(this);

            Destroy(this.gameObject);            
        }        
    }
}


