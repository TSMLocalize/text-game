using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enmity_Manager : MonoBehaviour
{        
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;    
    public Action_Handler act_Handler;        

    public float currentEnmity;
    public List<EnmityFigure> enmityFigures;
    public GameObject enmityNumberPF;

    public float enmityRandomizer;
    public float totalEnmity;    

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }    

    public void determineAttackTargetFromEnmity(Enemy enemyToAttack)
    {
        enmityRandomizer = Random.Range(1, 100);        

        for (int i = 0; i < enemyToAttack.EnmityAgainstPlayersList.Count; i++)
        {
            totalEnmity += enemyToAttack.EnmityAgainstPlayersList[i];
        }

        if(enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[0].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player1");
        }
        else if(enmityRandomizer > enemyToAttack.EnmityAgainstPlayersList[0] &&
            enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[1].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player2");
        }
        else if (enmityRandomizer > enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] &&
            enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] + enemyToAttack.EnmityAgainstPlayersList[2])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[2].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player3");
        }
        else if (enmityRandomizer > enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] + enemyToAttack.EnmityAgainstPlayersList[2] &&
            enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] + enemyToAttack.EnmityAgainstPlayersList[2] + enemyToAttack.EnmityAgainstPlayersList[3])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[3].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player4");
        }
        else if (enmityRandomizer > enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] + 
            enemyToAttack.EnmityAgainstPlayersList[2] + enemyToAttack.EnmityAgainstPlayersList[3] &&
            enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] + 
            enemyToAttack.EnmityAgainstPlayersList[2] + enemyToAttack.EnmityAgainstPlayersList[3] + enemyToAttack.EnmityAgainstPlayersList[4])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[4].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player5");
        }
        else if (enmityRandomizer > enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] +
            enemyToAttack.EnmityAgainstPlayersList[2] + enemyToAttack.EnmityAgainstPlayersList[3] + enemyToAttack.EnmityAgainstPlayersList[4] &&
            enmityRandomizer <= enemyToAttack.EnmityAgainstPlayersList[0] + enemyToAttack.EnmityAgainstPlayersList[1] +
            enemyToAttack.EnmityAgainstPlayersList[2] + enemyToAttack.EnmityAgainstPlayersList[3] + enemyToAttack.EnmityAgainstPlayersList[4] + enemyToAttack.EnmityAgainstPlayersList[5])
        {
            BM.activeEnemy.EnemyTargetID = BM.PlayersInBattle[5].name;
            Debug.Log(enmityRandomizer);
            Debug.Log("Player6");
        }
    }

    //This code shows the red preview numbers below enemies when hovering over them
    public void showProvisionalEnmity(GameObject panel)
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (panel.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {
                    if (BM.activePlayer.activeSpell != null)
                    {
                        if (BM.activePlayer.activeSpell.isAoE)
                        {
                            for (int y = 0; y < BM.EnemiesInBattle.Count; y++)
                            {
                                IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[y], BM.activePlayer.ProvisionalEnmity);
                                enmityFigures[y].EnmityPercentage.color = Color.red;
                            }
                        }
                        else
                        {
                            IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], BM.activePlayer.ProvisionalEnmity);
                            enmityFigures[i].EnmityPercentage.color = Color.red;
                        }
                    } 
                    else
                    {
                        IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], BM.activePlayer.ProvisionalEnmity);
                        enmityFigures[i].EnmityPercentage.color = Color.red;
                    }                                        
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], enmityFigures[i]);
            }
        }
        else if (BM.battleStates == Battle_Manager.BattleStates.SELECT_FRIENDLY_TARGET)
        {
            for (int i = 0; i < BM.PlayersInBattle.Count; i++)
            {
                if (panel.gameObject == BM.PlayersInBattle[i].playerPanel)
                {
                    for (int y = 0; y < BM.EnemiesInBattle.Count; y++)
                    {
                        IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[y], BM.activePlayer.ProvisionalEnmity);
                        enmityFigures[y].EnmityPercentage.color = Color.red;
                    }                    
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], enmityFigures[i]);
            }
        }
    }

    //This code removes the enmity preview numbers when moving the cursor off of a potential target
    public void endProvisionalEnmity(GameObject panel)
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {                
                if (panel.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {
                    if (BM.activePlayer.activeSpell != null)
                    {
                        if (BM.activePlayer.activeSpell.isAoE)
                        {
                            for (int y = 0; y < BM.EnemiesInBattle.Count; y++)
                            {
                                IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[y], -BM.activePlayer.ProvisionalEnmity);
                            }
                        }                        
                        else if (BM.activePlayer.activeSpell.isAoE == false)
                        {
                            IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], -BM.activePlayer.ProvisionalEnmity);
                        }
                    }
                    else
                    {
                        IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], -BM.activePlayer.ProvisionalEnmity);
                    }
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], enmityFigures[i]);
                enmityFigures[i].EnmityPercentage.color = Color.white;
            }
        }

        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_FRIENDLY_TARGET)
        {
            for (int i = 0; i < BM.PlayersInBattle.Count; i++)
            {
                if (panel.gameObject == BM.PlayersInBattle[i].playerPanel)
                {
                    for (int y = 0; y < BM.EnemiesInBattle.Count; y++)
                    {
                        IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[y], -BM.activePlayer.ProvisionalEnmity);
                    }                    
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], enmityFigures[i]);
                enmityFigures[i].EnmityPercentage.color = Color.white;
            }
        }

    }

    //Creates an enmity number beneath the enemy
    public void CreateEnmityNumber(Player playerToHate, Enemy enemyWhoHates)
    {
        GameObject enmityNumber = Instantiate(enmityNumberPF, enemyWhoHates.battleSprite.transform);
        EnmityFigure enmityNumberToAdd = enmityNumber.GetComponent<EnmityFigure>();

        //Add up all enemy hate vs active player
        for (int i = 0; i < enemyWhoHates.EnmityAgainstPlayersList.Count; i++)
        {
            currentEnmity += enemyWhoHates.EnmityAgainstPlayersList[i];
        }

        //update amount of the current iterated enemy's hate of the active player as a percentage of the total hate for that player
        for (int y = 0; y < BM.PlayersInBattle.Count; y++)
        {
            if (BM.PlayersInBattle[y] == playerToHate)
            {
                enmityNumberToAdd.EnmityPercentage.text = Mathf.Round((enemyWhoHates.EnmityAgainstPlayersList[y] / currentEnmity) * 100).ToString() + "%";
                currentEnmity = 0;
            }
        }

        enmityFigures.Add(enmityNumberToAdd);
    }

    public void destroyEnmityNumbers()
    {
        for (int i = 0; i < enmityFigures.Count; i++)
        {
            Destroy(enmityFigures[i].gameObject);
        }

        enmityFigures.Clear();
    }

    //Creates an enmity number beneath the enemy
    public void UpdateEnmityNumber(Player playerToHate, Enemy enemyWhoHates, EnmityFigure displayFigure)
    {
        //Add up all enemy hate vs active player
        for (int i = 0; i < enemyWhoHates.EnmityAgainstPlayersList.Count; i++)
        {
            currentEnmity += enemyWhoHates.EnmityAgainstPlayersList[i];
        }

        //update amount of the current iterated enemy's hate of the active player as a percentage of the total hate for that player
        for (int y = 0; y < BM.PlayersInBattle.Count; y++)
        {
            if (BM.PlayersInBattle[y] == playerToHate)
            {
                displayFigure.EnmityPercentage.text = Mathf.Round((enemyWhoHates.EnmityAgainstPlayersList[y] / currentEnmity) * 100).ToString() + "%";
                currentEnmity = 0;
            }
        }
    }

    //Increases enmity against the targetted player
    public void IncreaseEnmity(Player playerToHate, Enemy enemyWhoHates, float amountToIncrease)
    {
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            if (BM.PlayersInBattle[i] == playerToHate)
            {
                enemyWhoHates.EnmityAgainstPlayersList[i] += amountToIncrease;                
            }
        }
    }

    public void workOutProvisionalEnmity(Player playerToAddEnmity, string typeOfAttack)
    {
        switch (typeOfAttack)
        {
            case "Attack":
                //Eventually, this will be, like, if 10 ~ 20 attack then show enmity from 15
                playerToAddEnmity.ProvisionalEnmity = 10f;
                break;            
            case "Magic":
                playerToAddEnmity.ProvisionalEnmity = 20f;                
                break;
            default:
                break;
        }
    }

    public void workOutActualEnmity(Player playerToAddEnmity, Enemy enemyWhoHates, string typeOfAttack)
    {
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            if (BM.PlayersInBattle[i] == BM.activePlayer)
            {
                if (playerToAddEnmity.isCastingSpell)
                {
                    if (playerToAddEnmity.activeSpell.isAoE || playerToAddEnmity.activeSpell.isSupport)
                    {
                        for (int y = 0; y < BM.EnemiesInBattle.Count; y++)
                        {
                            BM.EnemiesInBattle[y].EnmityAgainstPlayersList[i] -= BM.activePlayer.ProvisionalEnmity;
                        }
                    }
                    else
                    {
                        BM.playerTarget.EnmityAgainstPlayersList[i] -= BM.activePlayer.ProvisionalEnmity;
                    }
                }
                else
                {
                    BM.playerTarget.EnmityAgainstPlayersList[i] -= BM.activePlayer.ProvisionalEnmity;
                }

                BM.activePlayer.ProvisionalEnmity = 0;
            }
        }

        switch (typeOfAttack)
        {
            case "Attack":
                //This will need to check the final score and associated enmity (check ActionLog report action)
                playerToAddEnmity.ActualEnmity = 20f;
                IncreaseEnmity(playerToAddEnmity, enemyWhoHates, playerToAddEnmity.ActualEnmity);
                break;
            case "StartCast":
                //Clear all provisional enmity until spell actually goes off                
                playerToAddEnmity.ActualEnmity = 0;                
                                
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    IncreaseEnmity(playerToAddEnmity, BM.EnemiesInBattle[i], playerToAddEnmity.ActualEnmity);
                }          

                break;
            case "FinishCast":
                //This will eventually contain all the enmity formulas for various spells.
                playerToAddEnmity.ActualEnmity = 10f; //VERY WEIRD: DOUBLES THIS NUMBER IN THE RESULT FOR SOME REASON!

                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    IncreaseEnmity(playerToAddEnmity, BM.EnemiesInBattle[i], playerToAddEnmity.ActualEnmity);
                }
                break;
            default:                
                break;
        }        
    }

    public IEnumerator decayEnmityOverTime()
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
            else
            {
                for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
                {
                    for (int y = 0; y < BM.EnemiesInBattle[i].EnmityAgainstPlayersList.Count; y++)
                    {
                        if (BM.EnemiesInBattle[i].EnmityAgainstPlayersList[y] > 1)
                        {
                            BM.EnemiesInBattle[i].EnmityAgainstPlayersList[y] -= 1;
                        }
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }            
        }
    }
}
