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
    public bool spellReportFinished;
    public bool enemySpellReportFinished;

    public int maxMessages;
    public List<Message> messageList;    
    public GameObject chatPanel;   
    public GameObject textObject;

    int m_IndexNumber;

    // Start is called before the first frame update
    void Start()
    {
        maxMessages = 25;
        messageList = new List<Message>();        
        BM = GetComponent<Battle_Manager>();
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TextMeshProUGUI textObject;
    }    

    //GAMEPLAY FUNCTIONS

    public void reportToLog(string report)
    {
        switch (report)
        {
            case "PlayerAttack":
                
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                float random = Random.Range(1, 101);
                float outcome = BM.activePlayer.Accuracy + random;

                SendMessagesToCombatLog(
                        BM.activePlayer.name + "'s hit score is " + outcome + " (" + random + " + " + BM.activePlayer.Accuracy + " acc)" +
                        " vs " + BM.playerTarget.EnemyName + "'s evasion of " + BM.playerTarget.Evasion + ".");

                if (outcome > BM.playerTarget.Evasion)
                {
                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " hits the enemy!");
                    createFloatingText(BM.playerTarget.battleSprite.transform.position, BM.activePlayer.Attack.ToString());

                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activePlayer.name + " misses the enemy...");
                    createFloatingText(BM.playerTarget.battleSprite.transform.position, "Miss!");
                }
                break;
            case "PlayerWait":
                SendMessagesToCombatLog(
                    BM.activePlayer.name + " waits.");
                break;
            case "PlayerStartCast":
                
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                SendMessagesToCombatLog(
                    BM.activePlayer.name + " starts casting " + BM.activePlayer.activeSpell.name + " on " + BM.playerTarget.EnemyName + ".");
                break;
            case "PlayerFinishCast":
                while (spellReportFinished == false)
                {
                    setPlayerOrEnemyTargetFromID(BM.activePlayer, null);

                    SendMessagesToCombatLog(
                        BM.activePlayer.name + " casts " + BM.activePlayer.activeSpell.name + " on the " + BM.playerTarget.EnemyName + "!");
                    spellReportFinished = true;
                }                
                break;
            case "EnemyAttack":

                setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                float enemyRandom = Random.Range(1, 101);
                float enemyOutcome = BM.activeEnemy.Accuracy + enemyRandom;

                SendMessagesToCombatLog(
                        BM.activeEnemy.EnemyName + "'s hit score is " + enemyOutcome + " (" + enemyRandom + " + " + BM.activeEnemy.Accuracy + " acc)" +
                        " vs " + BM.enemyTarget.name + "'s evasion of " + BM.enemyTarget.Evasion + ".");

                if (enemyOutcome > BM.enemyTarget.Evasion)
                {
                    SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " hits " + BM.enemyTarget.name + "...");
                    createFloatingText(BM.enemyTarget.battleSprite.transform.position, BM.activeEnemy.Attack.ToString());

                }
                else
                {
                    SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " misses " + BM.enemyTarget.name + "!");
                    createFloatingText(BM.enemyTarget.battleSprite.transform.position, "Miss!");
                }
                break;            
            case "EnemyStartCast":

                setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                SendMessagesToCombatLog(
                    BM.activeEnemy.EnemyName + " starts casting " + BM.activeEnemy.activeSpell.name + " on " + BM.enemyTarget.name + ".");
                break;
            case "EnemyFinishCast":
                while (enemySpellReportFinished == false)
                {
                    setPlayerOrEnemyTargetFromID(null, BM.activeEnemy);

                    SendMessagesToCombatLog(
                        BM.activeEnemy.EnemyName + " casts " + BM.activeEnemy.activeSpell.name + " on " + BM.enemyTarget.name + "!");
                    enemySpellReportFinished = true;
                }
                    
                break;
            default:
                break;
        }
    }

    //This method has been added because of serialization issues
    //players and enemies store their target by a name ID instead
    public void setPlayerOrEnemyTargetFromID(Player player = null, Enemy enemy = null) 
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

    // CREATE TEXT FUNCTIONS
    public void createFloatingText(Vector3 position, string amount)
    {
        BM.instantiatedFloatingDamage = Instantiate(BM.floatingDamage, position, Quaternion.identity);

        BM.instantiatedFloatingDamage.GetComponent<TextMeshPro>().text = amount;

        BM.floatingNumberTarget = new Vector3(BM.instantiatedFloatingDamage.transform.position.x, BM.instantiatedFloatingDamage.transform.position.y + 1f);

        BM.floatUp = true;
    }

    public void SendMessagesToCombatLog(string text)
    {        
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }        

        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }

    // BATTLE MANAGER UI FUNCTIONS

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
            //Assign Panels and populate
            BM.PlayersInBattle[i].playerPanel = BM.PlayerPanels[i];
            BM.PlayersInBattle[i].playerPanelText = BM.PlayerPanelTexts[i];
            BM.PlayersInBattle[i].playerPanelText.fontSize = 12;
            BM.PlayersInBattle[i].playerPanelBackground = BM.PlayerPanels[i].GetComponent<Image>();
            BM.playerPanelArray = BM.PlayerPanels[i].GetComponentsInChildren<Image>();
            BM.playerPanelArray[3].overrideSprite = BM.PlayersInBattle[i].PlayerPortrait;
            //Panel Text setup            
            BM.PlayersInBattle[i].playerPanelText.text =
                BM.PlayersInBattle[i].name + "\n" +
                "HP: " + BM.PlayersInBattle[i].currentHP + "/" + BM.PlayersInBattle[i].maxHP;
            //Speed Bar setup
            BM.PlayersInBattle[i].playerSpeedBarText = BM.PlayerSpeedBarTexts[i];
            BM.PlayersInBattle[i].playerSpeedBar = BM.PlayerSpeedBars[i];
            //Cast Bar setup
            BM.PlayersInBattle[i].playerCastBar = BM.PlayerCastBars[i];
            BM.PlayersInBattle[i].playerCastBarText = BM.PlayerCastBarTexts[i];
            BM.PlayersInBattle[i].playerCastBarFill = BM.PlayerCastBarFills[i];
            BM.PlayerCastBars[i].SetActive(false);
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
        for (int i = 0; i < BM.PlayerOptions.Count; i++)
        {
            BM.PlayerOptions[i].GetComponent<Image>().color = BM.defaultBlueColor;
            BM.PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            BM.PlayerOptions[i].SetActive(false);
        }
    }

    public void clearSpellOptionList()
    {
        for (int i = 0; i < BM.SpellOptions.Count; i++)
        {
            BM.SpellOptions[i].GetComponentInChildren<Image>().color = BM.defaultBlueColor;
            BM.SpellOptions[i].SetActive(false);
        }
    }

    public void populateActionList()
    {
        for (int i = 0; i < BM.PlayerOptions.Count; i++)
        {
            BM.PlayerOptions[i].SetActive(false);
        }

        if (BM.activePlayer.isCastingSpell == true)
        {
            BM.activePlayer.playerOptions.Clear();

            BM.activePlayer.playerOptions.Add("Cast");

            for (int i = 0; i < BM.activePlayer.playerOptions.Count; i++)
            {
                BM.PlayerOptions[i].SetActive(true);
                BM.PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.playerOptions[i];
            }
        }
        else if (BM.activePlayer.isCastingSpell == false)
        {
            BM.activePlayer.playerOptions.Clear();

            BM.activePlayer.playerOptions.Add("Attack");
            BM.activePlayer.playerOptions.Add("Magic");
            BM.activePlayer.playerOptions.Add("Change Row");
            BM.activePlayer.playerOptions.Add("Wait");

            for (int i = 0; i < BM.activePlayer.playerOptions.Count; i++)
            {
                BM.PlayerOptions[i].SetActive(true);
                BM.PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.playerOptions[i];
            }
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

            //Update and show cast bars while isCastingSpell is true for a player
            if (BM.PlayersInBattle[i].isCastingSpell == true)
            {
                BM.PlayersInBattle[i].playerCastBar.SetActive(true);
                BM.PlayersInBattle[i].playerCastBarText.text = BM.PlayersInBattle[i].castSpeedTotal + " (" + BM.PlayersInBattle[i].castSpeed + ")";
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.PlayersInBattle[i].castSpeedTotal / 100), 0, 1),
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.y,
                BM.PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                BM.PlayersInBattle[i].playerCastBar.SetActive(false);
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
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((BM.EnemiesInBattle[i].castSpeedTotal / 100), 0, 1),
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale.y,
                BM.EnemiesInBattle[i].enemyCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                BM.EnemiesInBattle[i].enemyCastBar.SetActive(false);
            }
        }
    }

    public void populateSpellOptionList()
    {
        for (int i = 0; i < BM.activePlayer.spellBook.Count; i++)
        {
            BM.optionsArray = BM.SpellOptions[i].GetComponentsInChildren<Image>();
            BM.SpellOptions[i].SetActive(true);
            BM.SpellOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = BM.activePlayer.spellBook[i].name;
            BM.optionsArray[1].overrideSprite = BM.activePlayer.spellBook[i].spellIcon;
            BM.SpellOptions[i].GetComponentInChildren<Image>().color = BM.defaultBlueColor;
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
                BM.battleStates = Battle_Manager.BattleStates.SELECT_OPTION;
            }
            else if (BM.selectedCommand == "Wait")
            {
                BM.battleStates = Battle_Manager.BattleStates.RESOLVE_ACTION;
            }
            else if (BM.selectedCommand == "Cast")
            {               
                BM.battleStates = Battle_Manager.BattleStates.RESOLVE_SPELL;
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
        //enemy.battleSprite.GetComponent<Animator>().SetBool("IsWalking", false);
        //enemy.battleSprite.GetComponent<Animator>().SetBool("TakeDamage", false);

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
        else if (state == "IsWalking")
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool("IsWalking", true);
        }
        /*else if (enemy.hasConstantAnimationState)
        {
            enemy.battleSprite.GetComponent<Animator>().SetBool(enemy.constantAnimationState, true);
        } */
    }
}
