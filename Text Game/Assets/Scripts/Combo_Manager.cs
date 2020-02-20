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
    public bool WSTimerActivated;   
    public GameObject ComboPanel;
    public GameObject CurrentSkillChain;
    public WeaponSkill skillChainToCreate;    
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
        //Adds WS info to the Combolog
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

        //Creates a Skillchain if filling in the 2nd, 4th or 6th WS on the Combolog
        if (ComboEntries[0].activeSelf && ComboEntries [1].activeSelf && ComboEntries[2].activeSelf == false)
        {
            CurrentSkillChain.SetActive(true);

            // TEMP
            skillChainToCreate = weaponSkills.Scission;
            //

            SCEntryImageArray = CurrentSkillChain.GetComponentsInChildren<Image>();
            CurrentSkillChain.GetComponentInChildren<TextMeshProUGUI>().text = skillChainToCreate.name;
            SCEntryImageArray[1].overrideSprite = skillChainToCreate.weaponSkillIcon;
            BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = true;
        } 
        else if (ComboEntries[2].activeSelf && ComboEntries[3].activeSelf && ComboEntries[4].activeSelf == false)
        {
            // TEMP
            skillChainToCreate = weaponSkills.Fusion;
            //

            SCEntryImageArray = CurrentSkillChain.GetComponentsInChildren<Image>();
            CurrentSkillChain.GetComponentInChildren<TextMeshProUGUI>().text = skillChainToCreate.name;
            SCEntryImageArray[1].overrideSprite = skillChainToCreate.weaponSkillIcon;
            BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = true;
        }
        else if (ComboEntries[4].activeSelf && ComboEntries[5].activeSelf)
        {
            // TEMP
            skillChainToCreate = weaponSkills.Light;
            //

            SCEntryImageArray = CurrentSkillChain.GetComponentsInChildren<Image>();
            CurrentSkillChain.GetComponentInChildren<TextMeshProUGUI>().text = skillChainToCreate.name;
            SCEntryImageArray[1].overrideSprite = skillChainToCreate.weaponSkillIcon;
            BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = true;
        }

    }

    public void PlayerWeaponskill(WeaponSkill weaponSkill, Player attacker = null, Enemy target = null)
    {
        
        switch (weaponSkill.name)
        {
            case "Fast Blade":                               
                BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", true);
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                addWSToTheList(weaponSkill);
                break;

            case "Raging Axe":
                BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", true);
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                addWSToTheList(weaponSkill);
                break;

            case "Penta Thrust":
                BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", true);
                ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
                addWSToTheList(weaponSkill);
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
                for (int i = 0; i < ComboEntries.Count; i++)
                {
                    ComboEntries[i].SetActive(false);
                }
                CurrentSkillChain.SetActive(false);
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
