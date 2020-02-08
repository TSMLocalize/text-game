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

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        ActionHandler = GetComponent<Action_Handler>();
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
}
