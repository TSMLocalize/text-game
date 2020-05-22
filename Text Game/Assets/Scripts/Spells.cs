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
    public Enemy_Spells enemySpells;

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
        Fire.name = "Fire";
        Ice.castTime = 35;
        Ice.name = "Ice";
        Cure.castTime = 46;
        Cure.isSupport = true;
        Cure.name = "Cure";
        Curaga.castTime = 32;
        Curaga.isAoE = true;
        Curaga.isSupport = true;
        Curaga.name = "Curaga";
        Firaga.castTime = 30;
        Firaga.isAoE = true;
        Firaga.name = "Firaga";
        Poison.castTime = 2;
        Poisonga.castTime = 2;
        Poisonga.isAoE = true;
        Poisonga.name = "Poisonga";
        Sleep.castTime = 10;
        Sleep.name = "Sleep";      
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
                    action_Handler.CreateStatusAilment(BM.EnemiesInBattle[i].battleSprite, 11, poison, "Poisonga", BM.EnemiesInBattle[i]);
                    animHandler.enemyAnimationController(BM.EnemiesInBattle[i], "IsCritical");
                    BM.EnemiesInBattle[i].constantAnimationStates.Add("IsCritical");
                    TickStatus("Poisonga", BM.EnemiesInBattle[i]);
                }
                break;
            case "Sleep":
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    BM.EnemiesInBattle[i].isAsleep = true;
                    action_Handler.CreateStatusAilment(BM.EnemiesInBattle[i].battleSprite, 10, sleep, "Sleep", BM.EnemiesInBattle[i]);
                    BM.EnemiesInBattle[i].preDebuffSpeed = BM.EnemiesInBattle[i].speed;                    
                    animHandler.enemyAnimationController(BM.EnemiesInBattle[i]);
                    TickStatus("Sleep", BM.EnemiesInBattle[i]);                    
                }
                break;
            default:
                Debug.Log("Invalid Spell ID Entered");
                break;
        }
    }

    //This is what happens each tick for a given status
    public void TickStatus(string statusID, Enemy targetEnemy = null, Player targetPlayer = null)
    {
        switch (statusID)
        {
            case "Sleep":                                
                targetEnemy.speed = 0;                                            
                break;
            case "Poisonga":
                action_Handler.CreateDamagePopUp(targetEnemy.battleSprite.transform.position, "12", Color.gray);
                break;
            default:
                break;
        }
    }

    //This ticks the status ailment in line with the other coroutines stop/start
    public IEnumerator tickStatusAilmentCoroutine(StatusAilment statusAilment)
    {
        if (statusAilment != null)
        {
            while (BM.coroutineIsPaused == true)
            {
                yield return null;
            }

            while (BM.coroutineIsPaused == false)
            {
                if (BM.returningStarting == true)
                {
                    yield return new WaitForSeconds(0.3f);
                    BM.returningStarting = false;
                }

                if (statusAilment.statusTimerNumber > 0)
                {
                    statusAilment.statusTimerNumber -= 1f;

                    TickStatus(statusAilment.type, statusAilment.afflictedEnemy);
                }

                if (statusAilment.statusTimerNumber <= 0)
                {                    
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
