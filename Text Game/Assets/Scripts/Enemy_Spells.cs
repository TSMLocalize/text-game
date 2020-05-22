using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Enemy_Spells : MonoBehaviour
{
    public Battle_Manager BM;    
    public Action_Handler action_Handler;
    public Battle_Manager_Functions BM_Funcs;
    public Animation_Handler animHandler;
    public Spells spells;

    private void Start()
    {

    }


    public void CastEnemySpell(string spellID)
    {
        switch (spellID)
        {
            case "Cure":
                BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy, null);
                animHandler.enemyAnimationController(BM.enemySupportTarget, "IsCasting");
                action_Handler.CreateDamagePopUp(BM.enemySupportTarget.battleSprite.transform.position, "70", Color.green);
                action_Handler.SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " casts " + BM.activeEnemy.activeSpell.name + " on the " + BM.enemySupportTarget.EnemyName + "!");
                break;
            case "Curaga":
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    animHandler.enemyAnimationController(BM.EnemiesInBattle[i], "IsCasting");
                    action_Handler.CreateDamagePopUp(BM.EnemiesInBattle[i].battleSprite.transform.position, "70", Color.green);
                    action_Handler.SendMessagesToCombatLog(BM.activeEnemy.EnemyName + " heals " + BM.EnemiesInBattle[i].EnemyName + " for 70!");
                }
                break;
            case "Fire":
                BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);
                action_Handler.SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " casts " + BM.activeEnemy.activeSpell.name + " on the " + BM.enemyTarget.name + "!");
                break;
            case "Blizzard":
                BM_Funcs.setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);
                action_Handler.SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " casts " + BM.activeEnemy.activeSpell.name + " on the " + BM.enemyTarget.name + "!");
                break;
            case "Firaga":
                for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                {
                    animHandler.animationController(BM.PlayersInBattle[i], "TakeDamage");
                    action_Handler.CreateDamagePopUp(BM.PlayersInBattle[i].battleSprite.transform.position, "70", Color.white);
                    action_Handler.SendMessagesToCombatLog(BM.activeEnemy.EnemyName + " deals 70 Damage to " + BM.PlayersInBattle[i].name + "!");
                }
                break;
            case "Poison":
                action_Handler.CreateStatusAilment(BM.enemyTarget.battleSprite, 11, spells.poison, "Poison", null, BM.enemyTarget);
                animHandler.animationController(BM.enemyTarget, "IsCritical");
                BM.enemyTarget.constantAnimationStates.Add("IsCritical");
                EnemyTickStatus("Poison", null, BM.enemyTarget);
                break;
            case "Poisonga":
                for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                {
                    action_Handler.CreateStatusAilment(BM.PlayersInBattle[i].battleSprite, 11, spells.poison, "Poisonga", null, BM.PlayersInBattle[i]);
                    animHandler.animationController(BM.PlayersInBattle[i], "IsCritical");
                    BM.PlayersInBattle[i].constantAnimationStates.Add("IsCritical");
                    EnemyTickStatus("Poisonga", null, BM.PlayersInBattle[i]);
                }
                break;
            case "Sleep":
                for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                {
                    BM.PlayersInBattle[i].isAsleep = true;
                    action_Handler.CreateStatusAilment(BM.PlayersInBattle[i].battleSprite, 10, spells.sleep, "Sleep", null, BM.PlayersInBattle[i]);
                    BM.PlayersInBattle[i].preDebuffSpeed = BM.PlayersInBattle[i].speed;
                    animHandler.animationController(BM.PlayersInBattle[i]);
                    EnemyTickStatus("Sleep", null, BM.PlayersInBattle[i]);
                }
                break;
            default:
                Debug.Log("Invalid Spell ID Entered");
                break;
        }
    }

    //This is what happens each tick for a given status
    public void EnemyTickStatus(string statusID, Enemy targetEnemy = null, Player targetPlayer = null)
    {
        switch (statusID)
        {
            case "Sleep":
                targetPlayer.speed = 0;
                break;
            case "Poisonga":
                action_Handler.CreateDamagePopUp(targetPlayer.battleSprite.transform.position, "12", Color.gray);
                break;
            default:
                break;
        }
    }

    //This ticks the status ailment in line with the other coroutines stop/start
    public IEnumerator tickEnemyStatusAilmentCoroutine(StatusAilment statusAilment)
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

                    EnemyTickStatus(statusAilment.type, null, statusAilment.afflictedPlayer);
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
