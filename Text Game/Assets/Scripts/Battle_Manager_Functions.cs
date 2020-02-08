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
    public Battle_Manager_UIFunctions BM_UIFuncs;
    public Battle_Manager_IEnumerators BM_Enums;
    public Combat_Log combat_Log;

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
        BM_UIFuncs = GetComponent<Battle_Manager_UIFunctions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        combat_Log = GetComponent<Combat_Log>();
    }

    public void standIdle(Player playerToIdle)
    {
        playerToIdle.battleSprite.transform.position = playerToIdle.position;
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

    public void resolveAction(string Command)
    {
        switch (Command)
        {
            case "Attack":
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.attackAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForAttackAnimation());
                animationController(BM.activePlayer, "IsAttacking");
                enemyAnimationController(BM.playerTarget, "TakeDamage");

                if (BM.attackAnimIsDone == true)
                {
                    BM.attackAnimCoroutineIsPaused = true;
                    enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.speedTotal -= 100f;
                    resolveAction(default);
                }

                break;
            case "Magic":
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                combat_Log.reportToLog("PlayerStartCast");
                animationController(BM.activePlayer, "IsChanting");
                BM.activePlayer.constantAnimationState = "IsChanting";
                BM.activePlayer.hasConstantAnimationState = true;
                BM.activePlayer.playerCastBar.SetActive(true);
                BM.activePlayer.castSpeedTotal = BM.activePlayer.activeSpell.castTime;                
                BM.activePlayer.speedTotal -= 100f;
                resolveAction(default);
                break;
            case "Wait":
                combat_Log.reportToLog("PlayerWait");                
                BM.activePlayer.speedTotal = (100f - BM.activePlayer.speed);
                resolveAction(default);
                break;
            case "Change Row":                                
                if (BM.isSwitchingWithOtherPlayer)
                {
                    BM.playerToSwitchRowWith.speedTotal -= 100f;
                    BM.isSwitchingWithOtherPlayer = false;
                }
                BM.playerToSwitchRowWith = null;
                BM.activePlayer.speedTotal -= 100f;
                resolveAction(default);
                break;
            case "Weapon Skill":
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.attackAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForAttackAnimation());
                animationController(BM.activePlayer, "IsAttacking");
                enemyAnimationController(BM.playerTarget, "TakeDamage");

                if (BM.attackAnimIsDone == true)
                {
                    BM.attackAnimCoroutineIsPaused = true;
                    enemyAnimationController(BM.playerTarget);
                    BM.activePlayer.speedTotal -= 100f;
                    BM.activePlayer.tpTotal = 0;
                    resolveAction(default);
                }                
                break;
            case "Cast":
                setPlayerOrEnemyTargetFromID(BM.activePlayer, null);
                BM.castAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForCastAnimation());
                animationController(BM.activePlayer, "IsCasting");

                if (BM.castAnimIsDone)
                {
                    combat_Log.spellReportFinished = false;
                    BM.castAnimCoroutineIsPaused = true;
                    BM.activePlayer.constantAnimationState = null;
                    BM.activePlayer.hasConstantAnimationState = false;                    
                    BM.activePlayer.isCastingSpell = false;                    
                    BM.activePlayer.castSpeedTotal = 0f;
                    BM.activePlayer.playerOptions.Remove("Cast");
                    resolveAction(default);
                }
                break;
            default:
                standIdle(BM.activePlayer);
                animationController(BM.activePlayer);
                BM.activePlayer.playerPanel.GetComponent<Image>().color = BM.defaultColor;
                BM_UIFuncs.resetChoicePanel();
                BM.ActionPanel.SetActive(false);
                BM.OptionPanel.SetActive(false);
                BM.ActivePlayers.Remove(BM.activePlayer);
                BM.activePlayer = null;
                BM.selectedCommand = null;

                if (BM.ActivePlayers.Count == 0)
                {
                    BM.returningStarting = true;
                    BM.startRoutinesGoingAgain = true;
                    redirectAction();
                }
                else
                {
                    BM.battleStates = Battle_Manager.BattleStates.SELECT_PLAYER;
                }
                break;
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
                BM_UIFuncs.populateSpellOptionList();
                BM.battleStates = Battle_Manager.BattleStates.SELECT_OPTION;
            }
            else if (BM.selectedCommand == "Weapon Skill")
            {
                BM_UIFuncs.populateWeaponSkillOptionList();
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
