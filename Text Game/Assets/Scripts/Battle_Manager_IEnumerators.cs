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
    public Action_Handler actionHandler;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        actionHandler = GetComponent<Action_Handler>();
    }

    /**
     * IENUMERATORS
     * #1. Player Wait for Animations
     * #2. Enemy Wait for Animations
     * #3. Speed Bar Ticker for Players
     * #4. Speed Bar Ticker for Enemies
     */

    //#1 Player Wait for Animations
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

    //#2 Enemy Wait for Animations
    public IEnumerator waitForEnemyReadyAnimation(Enemy enemy)
    {
        while (enemy.enemyReadyAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (enemy.enemyReadyAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            enemy.enemyReadyAnimIsDone = true;
        }
    }

    public IEnumerator waitForEnemyAttackAnimation(Enemy enemy)
    {
        while (enemy.enemyAttackAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (enemy.enemyAttackAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            enemy.enemyAttackAnimIsDone = true;
        }
    }

    public IEnumerator waitForEnemyCastAnimation(Enemy enemy)
    {
        while (enemy.enemyCastAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (enemy.enemyCastAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            enemy.enemyCastAnimIsDone = true;
        }
    }

    //#3 Speed bar ticker for Players
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

    //#4 Speed Bar ticker for Enemies
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
            if (enemy.speedTotal >= 100 || (enemy.isCastingSpell && enemy.castSpeedTotal <= 0))
            {

                if (enemy.isCastingSpell)
                {
                    enemy.enemyCastBar.GetComponent<Image>().transform.localScale = new Vector3(0f, enemy.enemyCastBar.GetComponent<Image>().transform.localScale.y);
                }
                else
                {
                    enemy.enemySpeedBar.GetComponent<Image>().transform.localScale = new Vector3(1f, enemy.enemySpeedBar.GetComponent<Image>().transform.localScale.y);
                }
            }
            else
            {
                if (enemy.isCastingSpell)
                {
                    enemy.castSpeedTotal -= enemy.castSpeed;
                }
                else
                {                    
                    enemy.speedTotal += enemy.speed;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
