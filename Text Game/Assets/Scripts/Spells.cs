using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Spells : MonoBehaviour
{
    public Battle_Manager BM;
    public Action_Handler action_Handler;
    public Battle_Manager_Functions BM_Funcs;
    public Animation_Handler animHandler;

    public Spell Fire;
    public Spell Ice;
    public Spell Cure;
    public Spell Curaga;
    public Spell Firaga;
    public Spell Poison;
    public Spell Poisonga;
    public Spell Sleep;

    //Status Icons
    public Sprite poison;
    public Sprite sleep;

    private void Start()
    {
        Fire.castTime = 34;
        Fire.methodID = "Fire";
        Ice.castTime = 35;
        Ice.methodID = "Ice";
        Cure.castTime = 46;
        Cure.isSupport = true;
        Cure.methodID = "Cure";
        Curaga.castTime = 32;
        Curaga.isAoE = true;
        Curaga.isSupport = true;
        Curaga.methodID = "Curaga";
        Firaga.castTime = 30;
        Firaga.isAoE = true;
        Firaga.methodID = "Firaga";
        Poison.castTime = 2;
        Poisonga.castTime = 2;
        Poisonga.isAoE = true;
        Poisonga.methodID = "Poisonga";
        Sleep.castTime = 10;
        Sleep.methodID = "Sleep";      
    }


    public void CastSpell(string spellID)
    {
        switch (spellID)
        {
            case "Cure":
                BM_Funcs.setPlayerOrEnemyTargetFromID(null, null, BM.activePlayer);
                animHandler.animationController(BM.supportTarget, "IsCasting");
                action_Handler.CreateDamagePopUp(BM.supportTarget.battleSprite.transform.position, "70", Color.green);
                action_Handler.SendMessagesToCombatLog(
                    BM.activePlayer.name + " casts " + BM.activePlayer.activeSpell.name + " on the " + BM.supportTarget.name + "!");
                break;
            case "Curaga":
                for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                {
                    animHandler.animationController(BM.PlayersInBattle[i], "IsCasting");
                    action_Handler.CreateDamagePopUp(BM.PlayersInBattle[i].battleSprite.transform.position, "70", Color.green);
                    action_Handler.SendMessagesToCombatLog(BM.activePlayer.name + " heals " + BM.PlayersInBattle[i].name + " for 70!");
                }                
                break;
            case "Fire":
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                action_Handler.SendMessagesToCombatLog(
                    BM.activePlayer.name + " casts " + BM.activePlayer.activeSpell.name + " on the " + BM.playerTarget.EnemyName + "!");
                break;
            case "Blizzard":
                BM_Funcs.setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                action_Handler.SendMessagesToCombatLog(
                    BM.activePlayer.name + " casts " + BM.activePlayer.activeSpell.name + " on the " + BM.playerTarget.EnemyName + "!");
                break;
            case "Firaga":
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    animHandler.enemyAnimationController(BM.EnemiesInBattle[i], "TakeDamage");
                    action_Handler.CreateDamagePopUp(BM.EnemiesInBattle[i].battleSprite.transform.position, "70", Color.white);
                    action_Handler.SendMessagesToCombatLog(BM.activePlayer.name + " deals 70 Damage to " + BM.EnemiesInBattle[i].EnemyName + "!");
                }
                break;
            case "Poisonga":
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    action_Handler.CreateStatusAilment(BM.EnemiesInBattle[i].battleSprite, 11, poison, "poison");
                }
                break;
            case "Sleep":
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    action_Handler.CreateStatusAilment(BM.EnemiesInBattle[i].battleSprite, 5, sleep, "poison", BM.EnemiesInBattle[i]);  
                }
                break;
            default:
                Debug.Log("Invalid Spell ID Entered");
                break;
        }
    }

    public void TickStatus(string statusID, Enemy targetEnemy = null, Player targetPlayer = null)
    {
        switch (statusID)
        {
            case "Sleep":                
                targetEnemy.speed = 0;
                animHandler.enemyAnimationController(targetEnemy, "IsDead");
                break;
            default:
                break;
        }
    }
}
