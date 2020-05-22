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

        if (this.statusTimerNumber <= 0)
        {
            if (this.afflictedPlayer != null)
            {
                //These clear up what to do once the debuff has dissipated
                switch (type)
                {
                    case "Poison":
                        for (int i = 0; i < afflictedPlayer.constantAnimationStates.Count; i++)
                        {
                            if (afflictedPlayer.constantAnimationStates[i] == "IsCritical")
                            {
                                afflictedPlayer.constantAnimationStates.Remove(afflictedPlayer.constantAnimationStates[i]);
                            }
                        }
                        
                        animHandler.animationController(afflictedPlayer);
                        break;
                    case "Sleep":
                        afflictedPlayer.isAsleep = false;
                        afflictedPlayer.speed = afflictedPlayer.preDebuffSpeed;
                        afflictedPlayer.constantAnimationStates.Remove("IsDead");
                        animHandler.animationController(afflictedPlayer);
                        break;
                    default:
                        break;
                }                
                
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
