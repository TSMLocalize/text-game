using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Battle_Manager_Enemy_Turn : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;

    public enum BattleStates
    {        
        SELECT_ENEMY,
        SELECT_ENEMY_ACTION,
        SELECT_ENEMY_TARGET,
        ENEMY_ATTACK,
        RESOLVE_ENEMY_TURN
    }

    public BattleStates battleStates;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case BattleStates.SELECT_ENEMY:

                for (int i = 0; i < BM.ActiveEnemies.Count; i++)
                {
                    BM.activeEnemy = BM.ActiveEnemies[i];
                }

                battleStates = BattleStates.SELECT_ENEMY_ACTION;

                break;
            case BattleStates.SELECT_ENEMY_ACTION:

                BM.selectedCommand = "EnemyAttack";

                battleStates = BattleStates.SELECT_ENEMY_TARGET;

                break;
            case BattleStates.SELECT_ENEMY_TARGET:

                int selectedNumber = Random.Range(0, BM.PlayersInBattle.Count);

                for (int i = 0; i < BM.PlayersInBattle.Count; i++)
                {
                    if (selectedNumber == i)
                    {
                        BM.activeEnemy.enemyTarget = BM.PlayersInBattle[i];
                    }
                }

                StartCoroutine(BM_Enums.waitForEnemyReadyAnimation(BM.activeEnemy));
                BM.activeEnemy.enemyReadyAnimCoroutineIsPaused = false;

                BM.activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);

                if (BM.activeEnemy.enemyReadyAnimIsDone == true)
                {
                    BM.activeEnemy.enemyReadyAnimCoroutineIsPaused = true;
                    BM.activeEnemy.enemyReadyAnimIsDone = false;
                    BM.activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
                    battleStates = BattleStates.ENEMY_ATTACK;
                }

                break;
            case BattleStates.ENEMY_ATTACK:

                BM.activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
                BM_Funcs.animationController(BM.activeEnemy.enemyTarget, "TakeDamage");

                BM.activeEnemy.enemyAttackAnimCoroutineIsPaused = false;

                StartCoroutine(BM_Enums.waitForEnemyAttackAnimation(BM.activeEnemy));

                if (BM.activeEnemy.enemyAttackAnimIsDone == true)
                {
                    BM.activeEnemy.enemyAttackAnimIsDone = false;
                    BM_Funcs.animationController(BM.activeEnemy.enemyTarget);
                    BM.activeEnemy.enemyAttackAnimCoroutineIsPaused = true;
                    battleStates = BattleStates.RESOLVE_ENEMY_TURN;
                }

                break;
            case BattleStates.RESOLVE_ENEMY_TURN:

                if (BM.selectedCommand == "EnemyAttack")
                {
                    BM.activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
                    BM.activeEnemy.speedTotal -= 100f;
                    BM.activeEnemy.enemyPanel.GetComponent<Image>().color = BM.defaultColor;
                    BM.activeEnemy.enemyTarget = null;
                    BM.ActiveEnemies.Remove(BM.activeEnemy);
                    BM.activeEnemy.enemyAttackAnimCoroutineIsPaused = true;
                    BM.activeEnemy.enemyReadyAnimCoroutineIsPaused = true;
                    BM.activeEnemy.enemyAttackAnimIsDone = false;
                    BM.activeEnemy.enemyReadyAnimIsDone = false;
                    BM.activeEnemy = null;
                    BM.selectedCommand = null;

                    if (BM.ActiveEnemies.Count == 0)
                    {
                        BM.returningStarting = true;
                        BM.startRoutinesGoingAgain = true;
                        BM.battleStates = Battle_Manager.BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_ENEMY;
                    }
                }

                break;
            default:
                break;
        }
    }

}
