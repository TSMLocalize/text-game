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
    public List<WeaponSkill> weaponSkillsInList;
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
    
    public void addWSToTheList(WeaponSkill WStoAdd)
    {
        if (weaponSkillsInList.Count == 8)
        {
            resetWeaponSkills();
        }

        //Adds WS info to the Combolog
        for (int i = 0; i < ComboEntries.Count; i++)
        {            
            if (ComboEntries[i].activeSelf == false)
            {                
                ComboEntries[i].SetActive(true);
                weaponSkillsInList.Add(WStoAdd);
                ComboEntryImageArray = ComboEntries[i].GetComponentsInChildren<Image>();
                ComboEntryImageArray[1].overrideSprite = WStoAdd.weaponSkillIcon;
                ComboEntryImageArray[2].overrideSprite = WStoAdd.weaponSkillElement;
                ComboEntries[i].GetComponentInChildren<TextMeshProUGUI>().text = WStoAdd.name;

                //Runs the method to check if the new WS has created a Skillchain
                if (weaponSkillsInList.Count > 1)
                {
                    if (weaponSkillsInList.Count == 2 || weaponSkillsInList.Count == 4 || weaponSkillsInList.Count == 6 || weaponSkillsInList.Count == 8)
                    {
                        skillChainToCreate = determineWhichSkillChain(weaponSkillsInList[weaponSkillsInList.Count - 2], WStoAdd);

                        if (skillChainToCreate != null)
                        {
                            timeRemaining += 10;
                            WStimer.text = timeRemaining.ToString();
                            setUpSkillChain();
                            BM.activePlayer.selectedWeaponSkill.willCreateSkillchain = true;
                            break;
                        }
                        //If failing, reset the panel, and set the WS at the start
                        else if (skillChainToCreate == null)
                        {
                            resetWeaponSkills();
                            ComboEntries[0].SetActive(true);
                            weaponSkillsInList.Add(WStoAdd);
                            ComboEntryImageArray = ComboEntries[0].GetComponentsInChildren<Image>();
                            ComboEntryImageArray[1].overrideSprite = WStoAdd.weaponSkillIcon;
                            ComboEntryImageArray[2].overrideSprite = WStoAdd.weaponSkillElement;
                            ComboEntries[0].GetComponentInChildren<TextMeshProUGUI>().text = WStoAdd.name;
                            break;
                        }
                        //Don't reset the panel if starting a SC on a new layer
                    } else if (weaponSkillsInList.Count == 3 || weaponSkillsInList.Count == 5 || weaponSkillsInList.Count == 7)
                    {
                        break;
                    }                               
                }
                else 
                { 
                    break;
                }                           
            }
        }
    }

    //Create the successful skillchain
    public void setUpSkillChain()
    {
        CurrentSkillChain.SetActive(true);
        SCEntryImageArray = CurrentSkillChain.GetComponentsInChildren<Image>();
        CurrentSkillChain.GetComponentInChildren<TextMeshProUGUI>().text = skillChainToCreate.name;
        SCEntryImageArray[1].overrideSprite = skillChainToCreate.weaponSkillIcon;        
    }

    public void resetWeaponSkills()
    {
        weaponSkillsInList.Clear();

        for (int i = 0; i < ComboEntries.Count; i++)
        {
            ComboEntries[i].SetActive(false);
        }
        CurrentSkillChain.SetActive(false);        
    }

    public void PlayerWeaponskill(WeaponSkill weaponSkill, Player attacker = null, Enemy target = null)
    {
        BM.activePlayer.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", true);
        ActionHandler.SendMessagesToCombatLog(attacker.name + " uses " + weaponSkill.name + " on the " + target);
        addWSToTheList(weaponSkill);
    }

    public void SetUpPanel()
    {
        if (!ComboPanel.activeSelf)
        {
            timeRemaining = 10;
        }
        
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
                resetWeaponSkills();
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

    public WeaponSkill determineWhichSkillChain(WeaponSkill entryOne, WeaponSkill entryTwo, WeaponSkill currentSC = null)
    {        
        if (entryOne.element == "Earth" || entryOne.element == "Water" || entryOne.element == "Wind" || entryOne.element == "Light")
        {
            if (entryTwo.element == "Fire")
            {                
                return weaponSkills.Liquefaction;
            }
            else if (entryTwo.element == "Ice")
            {                
                return weaponSkills.Induration;
            }
            else if (entryTwo.element == "Thunder")
            {                
                return weaponSkills.Impaction;
            }
            else if (entryTwo.element == "Dark")
            {                
                return weaponSkills.Compression;
            }
            else
            {
                return null;
            }
        }
        else if (entryOne.element == "Fire" || entryOne.element == "Ice" || entryOne.element == "Thunder" || entryOne.element == "Dark")
        {
            if (entryTwo.element == "Earth")
            {                
                return weaponSkills.Scission;
            }
            else if (entryTwo.element == "Water")
            {                
                return weaponSkills.Reverberation;
            }
            else if (entryTwo.element == "Wind")
            {                
                return weaponSkills.Detonation;
            }
            else if (entryTwo.element == "Light")
            {                
                return weaponSkills.Transfixion;
            }
            else
            {
                return null;
            }
        }

        return null;
    } 
}
