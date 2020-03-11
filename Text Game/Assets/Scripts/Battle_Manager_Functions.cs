using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager_Functions : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_IEnumerators BM_Enums;
    public Action_Handler ActionHandler;

    public GameObject pfPlayerPanel;
    public List<GameObject> instantiatedPlayerPanels;
    public GameObject instantiatedPlayerPanel;
    public GameObject PlayerPanels;
    public Image[] instantiatedPlayerPanelImageArray;
    public TextMeshProUGUI[] instantiatedPlayerPanelTextArray;

    public GameObject pfOption;
    public List<GameObject> instantiatedOptions;
    public GameObject instantiatedOption;
    public GameObject OptionPanel;

    public GameObject pfSpellOption;
    public List<GameObject> instantiatedSpellOptions;
    public GameObject instantiatedSpellOption;
    public GameObject SpellOptionPanel;

    // Start is called before the first frame update
    void Start()
    {   
        BM = GetComponent<Battle_Manager>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        ActionHandler = GetComponent<Action_Handler>();
    }

    public void resetChoice(Player player)
    {
        resetChoicePanel();
        BM.activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
        animationController(BM.activePlayer);
        BM.activePlayer.battleSprite.transform.position = BM.activePlayer.position;
        BM.activePlayer = player;
        BM.stepForward = true;
        BM.selectedCommand = null;
    }    

    public void standIdle(Player playerToIdle)
    {
        playerToIdle.battleSprite.transform.position = playerToIdle.position;
    }

    //This method has been added because of serialization issues
    //players and enemies store their target by a name ID, which is translated into a public target variable in the BM
    public void setPlayerOrEnemyTargetFromID(Player player = null, Enemy enemy = null, Player supportPlayer = null) 
    {
        if (player != null)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (player.PlayerTargetID == BM.EnemiesInBattle[i].EnemyName)
                {
                    BM.playerTarget = BM.EnemiesInBattle[i];
                }
            }
        }
        else if (supportPlayer != null)
        {
            for (int i = 0; i < BM.PlayersInBattle.Count; i++)
            {
                if (supportPlayer.PlayerTargetID == BM.PlayersInBattle[i].name)
                {
                    BM.supportTarget = BM.PlayersInBattle[i];
                }
            }
        }
        else if (enemy != null)
        {
            for (int i = 0; i < BM.PlayersInBattle.Count; i++)
            {
                if (enemy.EnemyTargetID == BM.PlayersInBattle[i].name)
                {
                    BM.enemyTarget = BM.PlayersInBattle[i];
                }
            }
        }
    }

    // BATTLE MANAGER UI FUNCTION
    public void setupCharacters()
    {
        //Add Players from PlayerManager to PlayersInBattle
        for (int i = 0; i < BM.PartyManager.GetComponent<Party_Manager>().partyMembers.Count; i++)
        {
            BM.PlayersInBattle.Add(BM.PartyManager.GetComponent<Party_Manager>().partyMembers[i]);
        }

        //Add Enemies from EnemyManager to EnemiesInBattle
        for (int i = 0; i < BM.EnemyManager.GetComponent<Enemy_Manager>().enemiesInBattle.Count; i++)
        {
            BM.EnemiesInBattle.Add(BM.EnemyManager.GetComponent<Enemy_Manager>().enemiesInBattle[i]);
        }

        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            //Instantiate Panels
            instantiatedPlayerPanel = Instantiate(pfPlayerPanel, PlayerPanels.transform);
            instantiatedPlayerPanelImageArray = instantiatedPlayerPanel.GetComponentsInChildren<Image>();
            instantiatedPlayerPanelTextArray = instantiatedPlayerPanel.GetComponentsInChildren<TextMeshProUGUI>();
            //instantiatedPlayerPanels.Add(instantiatedPlayerPanel);
            //Assign Panels and populate
            BM.PlayersInBattle[i].playerPanel = instantiatedPlayerPanel;
            BM.PlayersInBattle[i].playerPanelText = instantiatedPlayerPanelTextArray[0];
            BM.PlayersInBattle[i].playerPanelBackground = instantiatedPlayerPanelImageArray[0];            
            instantiatedPlayerPanelImageArray[1].overrideSprite = BM.PlayersInBattle[i].PlayerPortrait;
            //Panel Text setup            
            BM.PlayersInBattle[i].playerPanelText.text = BM.PlayersInBattle[i].name;
            //Speed Bar setup
            BM.PlayersInBattle[i].playerSpeedBarText = instantiatedPlayerPanelTextArray[4];
            BM.PlayersInBattle[i].playerSpeedBar = instantiatedPlayerPanelImageArray[9].gameObject;
            //TP Bar setup
            BM.PlayersInBattle[i].playerTPBarText = instantiatedPlayerPanelTextArray[3];
            BM.PlayersInBattle[i].playerTPBar = instantiatedPlayerPanelImageArray[6].gameObject;
            BM.PlayersInBattle[i].playerTPBarFill = instantiatedPlayerPanelImageArray[7].gameObject;
            //Cast Bar setup
            BM.PlayersInBattle[i].playerCastBar = instantiatedPlayerPanelImageArray[10].gameObject;
            BM.PlayersInBattle[i].playerCastBarText = instantiatedPlayerPanelTextArray[5];
            BM.PlayersInBattle[i].playerCastBarFill = instantiatedPlayerPanelImageArray[11].gameObject;            
            //Set battle sprites to their correct row
            AssignRows();
            //Transforms for moving
            BM.PlayersInBattle[i].target = new Vector3(BM.PlayersInBattle[i].battleSprite.transform.position.x - 1f, BM.PlayersInBattle[i].battleSprite.transform.position.y,
                BM.PlayersInBattle[i].battleSprite.transform.position.z);
            BM.PlayersInBattle[i].position = BM.PlayersInBattle[i].battleSprite.transform.position;
        }

        for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
        {
            //Assign Panels and populate
            BM.EnemiesInBattle[i].enemyPanel = BM.EnemyPanels[i];
            BM.EnemiesInBattle[i].enemyPanelText = BM.EnemyPanelTexts[i];
            BM.EnemiesInBattle[i].enemyPanelText.fontSize = 12;
            BM.EnemiesInBattle[i].enemyPanelBackground = BM.EnemyPanels[i].GetComponent<Image>();
            BM.enemyPanelArray = BM.EnemyPanels[i].GetComponentsInChildren<Image>();
            BM.enemyPanelArray[1].overrideSprite = BM.EnemiesInBattle[i].EnemyPortrait;
            //Panel Text setup            
            BM.EnemiesInBattle[i].enemyPanelText.text =
                BM.EnemiesInBattle[i].EnemyName + "\n" +
                "HP: " + BM.EnemiesInBattle[i].currentHP + "/" + BM.EnemiesInBattle[i].maxHP;
            //Speed Bar setup
            BM.EnemiesInBattle[i].enemySpeedBarText = BM.EnemySpeedBarTexts[i];
            BM.EnemiesInBattle[i].enemySpeedBar = BM.EnemySpeedBars[i];
            //Cast Bar setup
            BM.EnemiesInBattle[i].enemyCastBar = BM.EnemyCastBars[i];
            BM.EnemiesInBattle[i].enemyCastBarText = BM.EnemyCastBarTexts[i];
            BM.EnemiesInBattle[i].enemyCastBarFill = BM.EnemyCastBarFills[i];
            BM.EnemyCastBars[i].SetActive(false);            
        }
    }

    public void AssignRows()
    {
        //Assign battle sprites to rows
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            for (int y = 0; y < BM.Rows.Count; y++)
            {
                if (BM.Rows[y].GetComponent<Row>().ID == BM.PlayersInBattle[i].currentRowPositionID)
                {
                    //Setup new movement position for the sprite
                    BM.PlayersInBattle[i].battleSprite.transform.position = BM.Rows[y].gameObject.transform.position;
                    BM.PlayersInBattle[i].position = BM.Rows[y].gameObject.transform.position;
                    //Assign the player with a physical row position
                    BM.PlayersInBattle[i].currentRowPosition = BM.Rows[y].gameObject;
                    BM.PlayersInBattle[i].currentRowPositionIcon = BM.RowChangeIcons[y];

                    if (BM.PlayersInBattle[i].currentRowPositionID == 1 || BM.PlayersInBattle[i].currentRowPositionID == 5)
                    {
                        BM.PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }
                    else if (BM.PlayersInBattle[i].currentRowPositionID == 2 || BM.PlayersInBattle[i].currentRowPositionID == 6)
                    {
                        BM.PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    }
                    else if (BM.PlayersInBattle[i].currentRowPositionID == 3 || BM.PlayersInBattle[i].currentRowPositionID == 7)
                    {
                        BM.PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    }
                    else if (BM.PlayersInBattle[i].currentRowPositionID == 4 || BM.PlayersInBattle[i].currentRowPositionID == 8)
                    {
                        BM.PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    }
                }
            }
        }
    }

    public void updateRowPositions()
    {
        for (int i = 0; i < BM.Rows.Count; i++)
        {
            for (int y = 0; y < BM.PlayersInBattle.Count; y++)
            {
                if (BM.PlayersInBattle[y].battleSprite.transform.position == BM.Rows[i].transform.position)
                {
                    BM.PlayersInBattle[y].position = BM.Rows[i].gameObject.transform.position;
                    BM.PlayersInBattle[y].target = new Vector3(BM.PlayersInBattle[y].battleSprite.transform.position.x - 1f, BM.PlayersInBattle[y].battleSprite.transform.position.y,
                BM.PlayersInBattle[y].battleSprite.transform.position.z);
                    BM.PlayersInBattle[y].currentRowPosition = BM.Rows[i].gameObject;
                    BM.PlayersInBattle[y].currentRowPositionIcon = BM.RowChangeIcons[i];
                    BM.PlayersInBattle[y].currentRowPositionID = BM.Rows[i].GetComponent<Row>().ID;
                }
            }
        }
    }

    public void resetChoicePanel()
    {
        for (int i = 0; i < instantiatedOptions.Count; i++)
        {
            instantiatedOptions[i].GetComponent<Image>().color = BM.defaultBlueColor;
            instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "";            
        }
    }

    public void populateActionList()
    {        
        for (int i = 0; i < instantiatedOptions.Count; i++)
        {            
            Destroy(instantiatedOptions[i]);            
        }

        instantiatedOptions.Clear();

        if (BM.activePlayer.isCastingSpell == true)
        {
            BM.activePlayer.playerOptions.Clear();

            BM.activePlayer.playerOptions.Add("Cast");

            for (int i = 0; i < BM.activePlayer.playerOptions.Count; i++)
            {
                pfOption.GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.playerOptions[i];
                instantiatedOption = Instantiate(pfOption, OptionPanel.transform);
                instantiatedOptions.Add(instantiatedOption);
            }
        } 
        if (BM.activePlayer.isCastingSpell == false)
        {
            BM.activePlayer.playerOptions.Clear();

            if (BM.activePlayer.tpTotal > 100)
            {
                BM.activePlayer.playerOptions.Add("Weapon Skill");
            }

            BM.activePlayer.playerOptions.Add("Attack");
            BM.activePlayer.playerOptions.Add("Magic");
            BM.activePlayer.playerOptions.Add("Change Row");
            BM.activePlayer.playerOptions.Add("Wait");

            for (int i = 0; i < BM.activePlayer.playerOptions.Count; i++)
            {
                pfOption.GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.playerOptions[i];
                instantiatedOption = Instantiate(pfOption, OptionPanel.transform);
                instantiatedOptions.Add(instantiatedOption);
            }
        }
    }

    public void populateSpellOptionList()
    {
        for (int i = 0; i < instantiatedSpellOptions.Count; i++)
        {
            instantiatedSpellOptions[i].GetComponentInChildren<Image>().color = BM.defaultBlueColor;            
            Destroy(instantiatedSpellOptions[i]);
        }

        instantiatedSpellOptions.Clear();

        for (int i = 0; i < BM.activePlayer.spellBook.Count; i++)
        {            
            instantiatedSpellOption = Instantiate(pfSpellOption, SpellOptionPanel.transform);

            BM.spellOptionsArray = instantiatedSpellOption.GetComponentsInChildren<Image>();
            BM.spellOptionsArray[1].overrideSprite = BM.activePlayer.spellBook[i].spellIcon;
            BM.spellOptionsArray[0].color = BM.defaultBlueColor;

            instantiatedSpellOption.GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.spellBook[i].name;
            
            instantiatedSpellOptions.Add(instantiatedSpellOption);
        }
    }

    public void populateWeaponSkillOptionList()
    {
        for (int i = 0; i < instantiatedSpellOptions.Count; i++)
        {
            instantiatedSpellOptions[i].GetComponentInChildren<Image>().color = BM.defaultBlueColor;
            Destroy(instantiatedSpellOptions[i]);
        }
        
        instantiatedSpellOptions.Clear();

        for (int i = 0; i < BM.activePlayer.weaponSkills.Count; i++)
        {
            instantiatedSpellOption = Instantiate(pfSpellOption, SpellOptionPanel.transform);

            BM.wsOptionsArray = instantiatedSpellOption.GetComponentsInChildren<Image>();
            BM.wsOptionsArray[1].overrideSprite = BM.activePlayer.weaponSkills[i].weaponSkillIcon;
            BM.wsOptionsArray[0].color = BM.defaultBlueColor;

            instantiatedSpellOption.GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.weaponSkills[i].name;

            instantiatedSpellOptions.Add(instantiatedSpellOption);
        }        
    }

    public void updatePlayerUIBars()
    {
        //Update Player Speed bar data every frame
        for (int i = 0; i < BM.PlayersInBattle.Count; i++)
        {
            BM.PlayersInBattle[i].playerSpeedBarText.text = BM.PlayersInBattle[i].speedTotal + "/100 " + "(" + BM.PlayersInBattle[i].speed + ")";
            BM.PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.PlayersInBattle[i].speedTotal / 100), 0, 1),
            BM.PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale.y,
            BM.PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale.z);

            //Update Player TP bar data every frame
            BM.PlayersInBattle[i].playerTPBarText.text = BM.PlayersInBattle[i].tpTotal + "/100 " + "(" + BM.PlayersInBattle[i].storeTP + ")";
            BM.PlayersInBattle[i].playerTPBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.PlayersInBattle[i].tpTotal / 100), 0, 1),
            BM.PlayersInBattle[i].playerTPBarFill.GetComponent<Image>().transform.localScale.y,
            BM.PlayersInBattle[i].playerTPBarFill.GetComponent<Image>().transform.localScale.z);

            //Update cast bars while isCastingSpell is true for a player, else "--" and scale 0
            if (BM.PlayersInBattle[i].isCastingSpell == true)
            {
                BM.PlayersInBattle[i].playerCastBar.SetActive(true);
                BM.PlayersInBattle[i].playerCastBarText.text = BM.PlayersInBattle[i].castSpeedTotal + " (" + BM.PlayersInBattle[i].castSpeed + ")";
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.PlayersInBattle[i].castSpeedTotal / BM.PlayersInBattle[i].activeSpell.castTime), 0, 1),
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.y,
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp(0, 0, 1),
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.y,
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.z);
                BM.PlayersInBattle[i].playerCastBarText.text = "--";                
            }

        }
    }


    public void updateEnemyUIBars()
    {
        //Update Enemy Speed bar data every frame
        for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
        {
            BM.EnemiesInBattle[i].enemySpeedBarText.text = BM.EnemiesInBattle[i].speedTotal + "/100 " + "(" + BM.EnemiesInBattle[i].speed + ")";
            BM.EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.EnemiesInBattle[i].speedTotal / 100), 0, 1),
            BM.EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale.y,
            BM.EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale.z);

            //Update and show cast bars while isCastingSpell is true for a enemy
            if (BM.EnemiesInBattle[i].isCastingSpell == true)
            {
                BM.EnemiesInBattle[i].enemyCastBar.SetActive(true);
                BM.EnemiesInBattle[i].enemyCastBarText.text = BM.EnemiesInBattle[i].castSpeedTotal + " (" + BM.EnemiesInBattle[i].castSpeed + ")";
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.EnemiesInBattle[i].castSpeedTotal / BM.EnemiesInBattle[i].activeSpell.castTime), 0, 1),
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale.y,
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                BM.EnemiesInBattle[i].enemyCastBar.SetActive(false);
            }
        }
    }

    public void redirectAction()
    {        
        if (BM.startRoutinesGoingAgain)
        {
            if (BM.ActiveEnemies.Count > 0)
            {
                BM.battleStates = Battle_Manager.BattleStates.SELECT_ENEMY;
            }
            else
            {
                BM.battleStates = Battle_Manager.BattleStates.DEFAULT;
            }
        }
        else if (BM.selectedCommand != "")
        {
            if (BM.selectedCommand == "Attack")
            {
                BM.battleStates = Battle_Manager.BattleStates.SELECT_TARGET;
            }
            else if (BM.selectedCommand == "Magic")
            {
                populateSpellOptionList();
                BM.battleStates = Battle_Manager.BattleStates.SELECT_OPTION;
            }
            else if (BM.selectedCommand == "Weapon Skill")
            {
                populateWeaponSkillOptionList();
                BM.battleStates = Battle_Manager.BattleStates.SELECT_OPTION;
            }
            else if (BM.selectedCommand == "Wait")
            {
                BM.battleStates = Battle_Manager.BattleStates.RESOLVE_ACTION;
            }
            else if (BM.selectedCommand == "Cast")
            {               
                BM.battleStates = Battle_Manager.BattleStates.RESOLVE_ACTION;
            }            
            else if (BM.selectedCommand == "Change Row")
            {
                BM.battleStates = Battle_Manager.BattleStates.CHANGE_ROW;
            }
        }        
    }

    //Sets the animation to idle, or to animation specified, or to what the previous animation was if a currentAnimationState is set
    public void animationController(Player player, string state = null)
    {
        player.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsCasting", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsChanting", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsWalking", false);
        player.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsFastBlade", false);
        player.battleSprite.GetComponent<Animator>().SetBool("IsCritical", false);

        if (state == "TakeDamage")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", true);
        }
        else if (state == "IsAttacking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
        }
        else if (state == "IsCasting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsCasting", true);
        }
        else if (state == "IsReady")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
        }
        else if (state == "IsChanting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsChanting", true);
        }
        else if (state == "IsWalking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsWalking", true);
        }
        else if (state == "IsCritical")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsCritical", true);
        }
        else if (player.hasConstantAnimationState)
        {
            player.battleSprite.GetComponent<Animator>().SetBool(player.constantAnimationState, true);
        }
    }

    public void enemyAnimationController(Enemy enemy, string state = null)
    {
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsCasting", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);
        enemy.battleSprite.GetComponent<Animator>().SetBool("IsChanting", false);        
        enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);

        if (state == "TakeDamage")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", true);
        }
        else if (state == "IsAttacking")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
        }
        else if (state == "IsCasting")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsCasting", true);
        }
        else if (state == "IsReady")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
        }
        else if (state == "IsChanting")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsChanting", true);
        }
        else if (enemy.hasConstantAnimationState)
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool(enemy.constantAnimationState, true);
        }
    }
}
