using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Combo_Manager : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Action_Handler ActionHandler;
    public TextMeshProUGUI WStimer;
    public float timeRemaining = 50;
    public bool WSTimerActivated = true;
    public bool wsCoroutineIsPaused;
    public bool wsReturningStarting;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        ActionHandler = GetComponent<Action_Handler>();        

        StartCoroutine(updateTimeRemaining());
    }

    void Update()
    {
        
    }
    
    public void PlayerWeaponskill(string weaponSkill, Player attacker = null, Enemy target = null)
    {
        switch (weaponSkill)
        {
            case "Fast Blade":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill + " on the " + target);
                break;

            case "Raging Axe":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill + " on the " + target);
                break;

            case "Penta Thrust":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill + " on the " + target);
                break;

            default:
                break;
        }
    }

    public void AddTP(Player player, float amount)
    {
        player.tpTotal += amount;
    }

    public IEnumerator updateTimeRemaining()
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
            if (timeRemaining > 0)
            {
                timeRemaining -= 1f;
                WStimer.text = "Time Left: " + timeRemaining.ToString();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
