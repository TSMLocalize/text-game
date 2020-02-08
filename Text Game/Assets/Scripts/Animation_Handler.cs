using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Animation_Handler : MonoBehaviour
{
    public float speed;    
    public bool stepForward = false;
    public bool floatUp = false;
    public Battle_Manager BM;

    void Start()
    {
        BM = GetComponent<Battle_Manager>();
    }

    public void stepForwardFunc()
    {
        if (stepForward)
        {
            speed = 4.0f;

            //Transform the Sprite forward a set distance and set walking animation
            animationController(BM.activePlayer, "IsWalking");

            float step = speed * Time.deltaTime;
            BM.activePlayer.battleSprite.transform.position = Vector3.MoveTowards(BM.activePlayer.battleSprite.transform.position, BM.activePlayer.target, step);

            if (BM.activePlayer.battleSprite.transform.position == BM.activePlayer.target)
            {
                animationController(BM.activePlayer);

                stepForward = false;
            }
        }        
    }

    public void createFloatingText(Vector3 position, string amount)
    {
        BM.instantiatedFloatingDamage = Instantiate(BM.floatingDamage, position, Quaternion.identity);

        BM.instantiatedFloatingDamage.GetComponent<TextMeshPro>().text = amount;

        BM.floatingNumberTarget = new Vector3(BM.instantiatedFloatingDamage.transform.position.x, BM.instantiatedFloatingDamage.transform.position.y + 1f);

        floatUp = true;
    }

    public void floatUpFunc()
    {
        if (floatUp)
        {
            speed = 2.0f;

            float step = speed * Time.deltaTime;

            BM.instantiatedFloatingDamage.gameObject.transform.position = Vector3.MoveTowards(BM.instantiatedFloatingDamage.gameObject.transform.position, BM.floatingNumberTarget, step);

            if (BM.instantiatedFloatingDamage.transform.position == BM.floatingNumberTarget)
            {
                Destroy(BM.instantiatedFloatingDamage);
                floatUp = false;
            }
        }        
    }

    public void SwitchPlayers()
    {
        speed = 8.0f;

        float step = speed * Time.deltaTime;
        BM.activePlayer.battleSprite.transform.position = Vector3.MoveTowards(BM.activePlayer.battleSprite.transform.position, BM.RowToSwitch.transform.position, step);

        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            if (BM.PlayersInBattle[i].currentRowPosition == BM.RowToSwitch)
            {
                BM.isSwitchingWithOtherPlayer = true;

                BM.playerToSwitchRowWith = BM.PlayersInBattle[i];

                //Set hands up animation
                animationController(BM.PlayersInBattle[i], "IsCasting");

                BM.PlayersInBattle[i].battleSprite.transform.position = Vector3.MoveTowards(BM.PlayersInBattle[i].battleSprite.transform.position,
                    BM.activePlayer.position, step);
            }
        }

        //Ensure both players are in position (if switching places)
        if (BM.isSwitchingWithOtherPlayer)
        {
            if (BM.activePlayer.battleSprite.transform.position == BM.RowToSwitch.transform.position &&
                BM.playerToSwitchRowWith.battleSprite.transform.position == BM.activePlayer.position)
            {
                animationController(BM.playerToSwitchRowWith);
                animationController(BM.activePlayer);
                BM.rowSelected = false;
            }
        }
        //Ensure one player is in position (if not switching places)
        else
        {
            if (BM.activePlayer.battleSprite.transform.position == BM.RowToSwitch.transform.position)
            {
                animationController(BM.activePlayer);
                BM.rowSelected = false;
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
        else if (player.hasConstantAnimationState)
        {
            player.battleSprite.GetComponent<Animator>().SetBool(player.constantAnimationState, true);
        }
    }

    public void enemyAnimationController(Enemy enemy, string state = null)
    {
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsCasting", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsChanting", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);

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
        else if (enemy.hasConstantAnimationState)
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool(enemy.constantAnimationState, true);
        }
    }
}
