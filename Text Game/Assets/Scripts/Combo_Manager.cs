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
    public float timeRemaining = 5;
    public bool WSTimerActivated = true;
    public bool wsCoroutineIsPaused;
    public bool wsReturningStarting;
    public bool pauseForSC;
    public GameObject ComboPanel;
    public GameObject CurrentSkillChain;
    public List<GameObject> ComboEntries;
    public Image[] ComboEntryImageArray;
    public Image[] SCEntryImageArray;
    public WeaponSkill WStoBeAdded;
    public WeaponSkills weaponSkills;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        ActionHandler = GetComponent<Action_Handler>();
        weaponSkills = FindObjectOfType<WeaponSkills>();

        StartCoroutine(updateTimeRemaining());
    }

    void Update()
    {

    }
    
    public void addWSToTheList(WeaponSkill WStoAdd)
    {        
        for (int i = 0; i < ComboEntries.Count; i++)
        {
            if (ComboEntries[i].activeSelf == false)
            {
                ComboEntries[i].SetActive(true);
                ComboEntryImageArray = ComboEntries[i].GetComponentsInChildren<Image>();
                ComboEntryImageArray[1].overrideSprite = WStoAdd.weaponSkillIcon;
                ComboEntryImageArray[2].overrideSprite = WStoAdd.weaponSkillElement;
                ComboEntries[i].GetComponentInChildren<TextMeshProUGUI>().text = WStoAdd.name;
                break;
            }
        }

        if (ComboEntries[0].activeSelf && ComboEntries [1].activeSelf)
        {
            CurrentSkillChain.SetActive(true);
            SCEntryImageArray = CurrentSkillChain.GetComponentsInChildren<Image>();
            CurrentSkillChain.GetComponentInChildren<TextMeshProUGUI>().text = weaponSkills.Scission.name;
            SCEntryImageArray[1].overrideSprite = weaponSkills.Scission.weaponSkillIcon;
            pauseForSC = true;
        }

    }

    public void PlayerWeaponskill(WeaponSkill weaponSkill, Player attacker = null, Enemy target = null)
    {
        
        switch (weaponSkill.name)
        {
            case "Fast Blade":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                addWSToTheList(weaponSkill);                                                        
                break;

            case "Raging Axe":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                break;

            case "Penta Thrust":
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                break;

            default:
                break;
        }
    }

    public void SetUpPanel()
    {
        timeRemaining = 10;
        WStimer.text = timeRemaining.ToString();
        ComboPanel.SetActive(true);        
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

            if (timeRemaining < 1)
            {
                ComboPanel.SetActive(false);
                yield return null;
            }
            else if (timeRemaining > 0)
            {
                timeRemaining -= 1f;
                WStimer.text = timeRemaining.ToString();
                yield return new WaitForSeconds(0.5f);
            }            
        }
    }
}
