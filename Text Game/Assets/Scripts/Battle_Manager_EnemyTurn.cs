using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Battle_Manager_EnemyTurn : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;

    public bool EnemyTurnsComplete;

    public string enemySelectedCommand;

    public enum EnemyStates
    {
        DEFAULT,
        SELECT_ENEMY,
        SELECT_ENEMY_ACTION,
        SELECT_ENEMY_TARGET,
        RESOLVE_ENEMY_ACTION
    }
    public EnemyStates enemyStates;

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
        switch (enemyStates)
        {
            case EnemyStates.DEFAULT:
                //Wait here until SELECT_ENEMY is called from BATTLE_MANAGER
                
                break;
            case EnemyStates.SELECT_ENEMY:
                
                enemyStates = EnemyStates.SELECT_ENEMY_ACTION;

                break;
            case EnemyStates.SELECT_ENEMY_ACTION:                               

                //BM.activeEnemy.enemyPanel.GetComponent<Image>().color = Color.green;

                enemySelectedCommand = "Attack";

                if (enemySelectedCommand == "Attack")
                {
                    enemyStates = EnemyStates.RESOLVE_ENEMY_ACTION;
                }                

                break;
            case EnemyStates.SELECT_ENEMY_TARGET:
                break;
            case EnemyStates.RESOLVE_ENEMY_ACTION:

                if (enemySelectedCommand == "Attack")
                {
                    BM.enemyAttackAnimCoroutineIsPaused = false;
                    StartCoroutine(BM_Enums.waitForEnemyAttackAnimation());
                    BM_Funcs.enemyAnimationController(BM.activeEnemy, "IsAttacking");

                    if (BM.enemyAttackAnimIsDone)
                    {
                        BM.enemyAttackAnimIsDone = false;
                        BM.enemyAttackAnimCoroutineIsPaused = true;
                        BM_Funcs.enemyAnimationController(BM.activeEnemy);
                        BM.activeEnemy.speedTotal -= 100f;                                                                                           
                        BM.ActiveEnemies.Remove(BM.activeEnemy);
                        BM.activeEnemy = null;                        

                        BM.battleStates = Battle_Manager.BattleStates.DEFAULT;
                        
                        if (BM.ActiveEnemies.Count == 0)
                        {
                            BM.returningStarting = true;

                            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                            {
                                StartCoroutine(BM_Enums.updateEnemySpeedBars(BM.EnemiesInBattle[i]));
                            }                                                        
                        }

                        BM.coroutineIsPaused = false;
                        BM.battleStates = Battle_Manager.BattleStates.DEFAULT;
                        enemyStates = EnemyStates.DEFAULT;
                    }
                }

                break;            
            default:
                break;
        }
    }
}
