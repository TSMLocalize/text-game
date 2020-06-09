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

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }    

    public void determineAttackTargetFromEnmity(Enemy enemyToAttack, Player playerTarget)
    {

    }

    public void showProvisionalEnmity(GameObject panel)
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (panel.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {
                    IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], BM.activePlayer.ProvisionalEnmity);
                    enmityFigures[i].EnmityPercentage.color = Color.red;
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], enmityFigures[i]);
            }
        }
    }

    public void endProvisionalEnmity(GameObject panel)
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (panel.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {
                    IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], -BM.activePlayer.ProvisionalEnmity);
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
                Debug.Log(enemyWhoHates.EnmityAgainstPlayersList[i]);
            }
        }
    }

    public void workOutProvisionalEnmity(Player playerToAddEnmity, string typeOfAttack)
    {
        switch (typeOfAttack)
        {
            case "Attack":
                playerToAddEnmity.ProvisionalEnmity = 10f;
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
                BM.playerTarget.EnmityAgainstPlayersList[i] -= BM.activePlayer.ProvisionalEnmity;
                BM.activePlayer.ProvisionalEnmity = 0;
            }
        }

        switch (typeOfAttack)
        {
            case "Attack":
                playerToAddEnmity.ActualEnmity = 20f;                
                break;
            default:                
                break;
        }

        IncreaseEnmity(playerToAddEnmity, enemyWhoHates, playerToAddEnmity.ActualEnmity);
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
