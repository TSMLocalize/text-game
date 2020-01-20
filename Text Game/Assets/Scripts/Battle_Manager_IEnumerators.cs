using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager_IEnumerators : MonoBehaviour
{
    public Battle_Manager BM;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
    }

    // IENUMERATORS   
    public IEnumerator waitForAttackAnimation()
    {
        while (BM.attackAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (BM.attackAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            BM.attackAnimIsDone = true;
        }
    }

    public IEnumerator waitForEnemyAttackAnimation()
    {
        while (BM.enemyAttackAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (BM.enemyAttackAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            BM.enemyAttackAnimIsDone = true;
        }
    }

    public IEnumerator waitForCastAnimation()
    {
        while (BM.castAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (BM.castAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            BM.castAnimIsDone = true;
        }
    }

    public IEnumerator updatePlayerSpeedBars(Player player)
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
            //Forces scale to max when speed is 100+
            if (player.speedTotal >= 100 || (player.isCastingSpell && player.castSpeedTotal <= 0))
            {


                if (player.isCastingSpell)
                {
                    player.playerCastBar.GetComponent<Image>().transform.localScale = new Vector3(0f, player.playerCastBar.GetComponent<Image>().transform.localScale.y);
                }
                else
                {
                    player.playerSpeedBar.GetComponent<Image>().transform.localScale = new Vector3(1f, player.playerSpeedBar.GetComponent<Image>().transform.localScale.y);
                }
            }
            else
            {

                if (player.isCastingSpell)
                {
                    player.castSpeedTotal -= player.castSpeed;
                }
                else
                {
                    player.speedTotal += player.speed;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public IEnumerator updateEnemySpeedBars(Enemy enemy)
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
            //Forces scale to max when speed is 100+
            if (enemy.speedTotal >= 100)
            {
                enemy.enemySpeedBar.GetComponent<Image>().transform.localScale = new Vector3(1f, enemy.enemySpeedBar.GetComponent<Image>().transform.localScale.y);
            }
            else
            {
                enemy.speedTotal += enemy.speed;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    /*
    public IEnumerator delayWhileEnemyTurn()
    {
        while (BM.enemyTurnCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (BM.enemyTurnCoroutineIsPaused == false)
        {
            if (BM.enemyTurnPauseCounter <= 10f)
            {
                BM.enemyTurnPauseCounter += 1f;
                yield return new WaitForSeconds(0.5f);
            }

            if (BM.enemyTurnPauseCounter == 10f)
            {
                BM.enemyTurnPauseCounter = 0;
                BM.enemyTurnCoroutineIsPaused = true;
            }
        }
    }
    */
}
