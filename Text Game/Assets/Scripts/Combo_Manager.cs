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
    public Animation_Handler AnimHandler;
    public TextMeshProUGUI WStimer;
    public float timeRemaining = 5;
    public bool WSTimerActivated;   
    public GameObject ComboPanel;
    public GameObject CurrentSkillChain;
    public WeaponSkill skillChainToCreate = null;    
    public List<GameObject> ComboEntries;
    public List<WeaponSkill> weaponSkillsInList;
    public Image[] ComboEntryImageArray;
    public Image[] SCEntryImageArray;
    public WeaponSkill WStoBeAdded;
    public WeaponSkills weaponSkills;
    public WeaponSkill secondWS;
    public WeaponSkill thirdWS;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        ActionHandler = GetComponent<Action_Handler>();
        weaponSkills = FindObjectOfType<WeaponSkills>();
        AnimHandler = FindObjectOfType<Animation_Handler>();

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
                        skillChainToCreate = determineWhichSkillChain();

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
        secondWS = null;
        thirdWS = null;
        for (int i = 0; i < ComboEntries.Count; i++)
        {
            ComboEntries[i].SetActive(false);
        }
        CurrentSkillChain.SetActive(false);        
    }

    public void PlayerWeaponskill(WeaponSkill weaponSkill, Player attacker = null, Enemy target = null)
    {                
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

    public WeaponSkill determineWhichSkillChain()
    {        
        switch (weaponSkillsInList.Count)
        {
            case 2:
                return findLvl1SC(0, 1);
            case 4:
                secondWS = findLvl2SC(2, 3, skillChainToCreate);
                return secondWS;                
            case 6:
                thirdWS = findLvl2SC(4, 5, skillChainToCreate);
                return thirdWS;
            case 8:                
                return findLvl3SC();                
            default:
                return null;
        }
    } 

    public WeaponSkill findLvl1SC(int first, int second)
    {
        if (weaponSkillsInList[first].element == "Fire" || weaponSkillsInList[first].element == "Ice" || weaponSkillsInList[first].element == "Thunder")
        {
            if (weaponSkillsInList[second].element == "Earth")
                return weaponSkills.Scission;
            else if (weaponSkillsInList[second].element == "Water")
                return weaponSkills.Reverberation;
            else if (weaponSkillsInList[second].element == "Wind")
                return weaponSkills.Detonation;
            else return null;

        }
        else if (weaponSkillsInList[first].element == "Earth" || weaponSkillsInList[first].element == "Water" || weaponSkillsInList[first].element == "Wind")
        {
            if (weaponSkillsInList[second].element == "Fire")
                return weaponSkills.Liquefaction;
            else if (weaponSkillsInList[second].element == "Ice")
                return weaponSkills.Induration;
            else if (weaponSkillsInList[second].element == "Thunder")
                return weaponSkills.Impaction;
            else return null;
        }
        else
        {
            return null;
        }
    }

    public WeaponSkill findLvl2SC(int first, int second, WeaponSkill currentSC)
    {
        if (currentSC.alignment == "Dark")
        {
            if (weaponSkillsInList[first].element == "Wind" && weaponSkillsInList[second].element == "Earth")
                return weaponSkills.Gravitation;
            else if (weaponSkillsInList[first].element == "Ice" && weaponSkillsInList[second].element == "Water")
                return weaponSkills.Distortion;
            else if (weaponSkillsInList[first].element == "Thunder" && weaponSkillsInList[second].element == "Wind")
                return weaponSkills.Fragmentation;
            else return null;
        } else if (currentSC.alignment == "Light")
        {
            if (weaponSkillsInList[first].element == "Earth" && weaponSkillsInList[second].element == "Fire")
                return weaponSkills.Fusion;
            else if (weaponSkillsInList[first].element == "Water" && weaponSkillsInList[second].element == "Ice")
                return weaponSkills.Glaciation;
            else if (weaponSkillsInList[first].element == "Fire" && weaponSkillsInList[second].element == "Thunder")
                return weaponSkills.Fulmination;
            else return null;
        }
        else
        {
            return null;
        }
    }

    public WeaponSkill findLvl3SC()
    {
        if (secondWS.alignment == "Light" && thirdWS.alignment == "Dark")
        {
            if (findLvl2SC(6, 7, skillChainToCreate).alignment == "Light")            
                return weaponSkills.Radiance;            
            else            
                return null;            
        } else if (secondWS.alignment == "Dark" && thirdWS.alignment == "Light")
        {
            if (findLvl2SC(6, 7, skillChainToCreate).alignment == "Dark")            
                return weaponSkills.Umbra;            
            else            
                return null;            
        } else        
            return null;        
    }
}
