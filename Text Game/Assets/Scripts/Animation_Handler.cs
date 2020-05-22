using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

public class Animation_Handler : MonoBehaviour
{    
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Combo_Manager combo_Manager;
    public Battle_Manager_IEnumerators BM_Enums;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {        
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        combo_Manager = GetComponent<Combo_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BM.stepForward)
        {
            speed = 4.0f;

            //Transform the Sprite forward a set distance and set walking animation
            animationController(BM.activePlayer, "IsWalking");

            float step = speed * Time.deltaTime;
            BM.activePlayer.battleSprite.transform.position = Vector3.MoveTowards(BM.activePlayer.battleSprite.transform.position, BM.activePlayer.target, step);

            if (BM.activePlayer.battleSprite.transform.position == BM.activePlayer.target)
            {
                animationController(BM.activePlayer);

                BM.stepForward = false;
            }
        }
    }

    //Sets the animation to idle, or to animation specified, or to what the previous animation was if a currentAnimationState is set
    public void animationController(Player player, string state = null)
    {
        player.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsCasting", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsChanting", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsWalking", false);
        player.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsCritical", false);

        if (state == "IsFastBlade")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", true);
        }
        if (state == "TakeDamage")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", true);
        }
        else if (state == "IsAttacking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
        }
        else if (state == "IsCasting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsCasting", true);
        }
        else if (state == "IsReady")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
        }
        else if (state == "IsChanting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsChanting", true);
        }
        else if (state == "IsWalking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsWalking", true);
        }
        else if (state == "IsCritical")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsCritical", true);
        }
        else if (player.constantAnimationStates.Count > 1)
        {
            player.battleSprite.GetComponent<Animator>().SetBool(player.constantAnimationStates[1], true);
        } 
        else if (player.constantAnimationStates.Count > 0)
        {
            player.battleSprite.GetComponent<Animator>().SetBool(player.constantAnimationStates[0], true);
        }
    }

    public void enemyAnimationController(Enemy enemy, string state = null)
    {
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsCasting", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsChanting", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsCritical", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsDead", false);

        if(enemy.isAsleep){
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsDead", true);
        }
        else
        {
            if (state == "TakeDamage")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", true);
            }
            else if (state == "IsAttacking")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
            }
            else if (state == "IsCasting")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsCasting", true);
            }
            else if (state == "IsReady")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
            }
            else if (state == "IsChanting")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsChanting", true);
            }
            else if (state == "IsCritical")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsCritical", true);
            }
            else if (state == "IsDead")
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool("IsDead", true);
            }
            else if (enemy.constantAnimationStates.Count > 1)
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool(enemy.constantAnimationStates[1], true);
            }
            else if (enemy.constantAnimationStates.Count > 0)
            {
                enemy.battleSprite.GetComponent<Animator>().SetBool(enemy.constantAnimationStates[0], true);
            }
        }        
    }
}
