using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager : MonoBehaviour
{
    public Animation_Handler AnimHandler;
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;
    public Action_Handler ActionHandler;
    public Enmity_Manager EnmityManager;
    public Timers_Log Timers_Log;
    public Combo_Manager combo_Manager;    
    public float speed;
    public bool startRoutinesGoingAgain;
    public bool stepForward;            
    public bool panelChosen;
    public List<GameObject> Rows;
    public List<GameObject> EnemyRows;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public List<Player> PlayersInBattle;
    public List<Player> ActivePlayers;
    public Player activePlayer;
    public List<Enemy> EnemiesInBattle;
    public List<Enemy> ActiveEnemies;
    public Enemy activeEnemy;    
    public List<GameObject> PlayerSpeedBars;
    public List<TextMeshProUGUI> PlayerSpeedBarTexts;
    public List<GameObject> PlayerCastBars;
    public List<GameObject> PlayerCastBarFills;
    public List<TextMeshProUGUI> PlayerCastBarTexts;
    public List<GameObject> PlayerTPBars;
    public List<GameObject> PlayerTPBarFills;
    public List<TextMeshProUGUI> PlayerTPBarTexts;
    public List<GameObject> EnemyPanels;
    public List<GameObject> EnemySpeedBars;
    public List<TextMeshProUGUI> EnemySpeedBarTexts;
    public List<GameObject> EnemyCastBars;
    public List<GameObject> EnemyCastBarFills;
    public List<TextMeshProUGUI> EnemyCastBarTexts;    
    public List<TextMeshProUGUI> EnemyPanelTexts;    
    public List<GameObject> SpellOptions;
    public GameObject UICanvas;
    public GameObject ActionPanel;
    public GameObject OptionPanel;
    public GameObject PartyManager;
    public GameObject EnemyManager;    
    public Spells SpellManager;
    public Enemy_Spells EnemySpellManager;
    public bool coroutineIsPaused = false;
    public bool returningStarting = true;
    public bool enmityFiguresNeedSetting;
    public string selectedCommand = null;
    public Color defaultColor;
    public Color defaultBlueColor;
    public Image[] spellOptionsArray;
    public Image[] wsOptionsArray;
    public Image[] playerPanelArray;
    public Image[] enemyPanelArray;
    public Image[] actionPanelArray;
    public Enemy playerTarget;
    public Enemy enemySupportTarget;
    public Player enemyTarget;
    public Player supportTarget;
    
    public enum BattleStates
    {
        DEFAULT,
        SELECT_PLAYER,
        SELECT_ACTION,
        SELECT_OPTION,
        SELECT_FRIENDLY_TARGET,
        SELECT_TARGET,
        CHANGE_ROW,
        RESOLVE_ACTION,        
        SELECT_ENEMY,
        SELECT_ENEMY_ACTION,
        SELECT_ENEMY_TARGET,
        ENEMY_START_CAST,
        RESOLVE_ENEMY_TURN
    }

    public BattleStates battleStates;

    //TESTING
    public TextMeshProUGUI JosiePosText;

    // Start is called before the first frame update
    void Start()
    {
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        ActionHandler = GetComponent<Action_Handler>();
        EnmityManager = GetComponent<Enmity_Manager>();
        Timers_Log = GetComponent<Timers_Log>();
        combo_Manager = GetComponent<Combo_Manager>();
        AnimHandler = GetComponent<Animation_Handler>();
        EnemySpellManager = FindObjectOfType<Enemy_Spells>();

        BM_Funcs.setupCharacters();        

        ColorUtility.TryParseHtmlString("#010078", out defaultBlueColor);
        defaultColor = ActionPanel.GetComponent<Image>().color;

        //Start SpeedBar coroutines for players and enemies
        startSpeedCoroutines();                

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = UICanvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();        

        battleStates = BattleStates.DEFAULT;

    }

    // Update is called once per frame
    void Update()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        BM_Funcs.updateEnemyUIBars();

        BM_Funcs.updatePlayerUIBars();        

        switch (battleStates)
        {
            case BattleStates.DEFAULT:                

                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    EnemiesInBattle[i].enemyAttackAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyReadyAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyCastAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyAttackAnimIsDone = false;
                    EnemiesInBattle[i].enemyReadyAnimIsDone = false;
                    EnemiesInBattle[i].enemyCastAnimIsDone = false;
                }

                //Enumerators to start running again after game is paused
                if (startRoutinesGoingAgain)
                {
                    startSpeedCoroutines();
                    StartCoroutine(combo_Manager.updateTimeRemaining());
                    StartCoroutine(EnmityManager.decayEnmityOverTime());

                    if (ActionHandler.statusAilmentList.Count > 0)
                    {
                        for (int i = 0; i < ActionHandler.statusAilmentList.Count; i++)
                        {
                            if (ActionHandler.statusAilmentList[i].playerorenemy == "player")
                            {
                                StartCoroutine(EnemySpellManager.tickEnemyStatusAilmentCoroutine(ActionHandler.statusAilmentList[i]));
                            } 
                            
                            if (ActionHandler.statusAilmentList[i].playerorenemy == "enemy")
                            {
                                StartCoroutine(SpellManager.tickStatusAilmentCoroutine(ActionHandler.statusAilmentList[i]));
                            }                            
                        }
                    }                    
                    
                    startRoutinesGoingAgain = false;
                }

                //Check if a player is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < PlayersInBattle.Count; i++)
                {                   
                    if (PlayersInBattle[i].speedTotal >= 100 || (PlayersInBattle[i].isCastingSpell && PlayersInBattle[i].castSpeedTotal <= 0))
                    {
                        if (EnemiesInBattle[i].isCastingSpell != true)
                        {
                            AnimHandler.animationController(PlayersInBattle[i], "IsReady");                            
                        }

                        ActivePlayers.Add(PlayersInBattle[i]);
                    }
                }
                //Check if an enemy is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    if (EnemiesInBattle[i].speedTotal >= 100 || (EnemiesInBattle[i].isCastingSpell && EnemiesInBattle[i].castSpeedTotal <= 0))
                    {
                        if (EnemiesInBattle[i].isCastingSpell != true)
                        {
                            AnimHandler.enemyAnimationController(EnemiesInBattle[i], "IsReady");                                                
                        }

                        EnemiesInBattle[i].enemyPanelBackground.color = Color.yellow;
                        ActiveEnemies.Add(EnemiesInBattle[i]);
                    }
                }

                if (ActivePlayers.Count > 0)
                {                    
                    coroutineIsPaused = true;                    
                    battleStates = BattleStates.SELECT_PLAYER;
                }
                else if (ActiveEnemies.Count > 0)
                {
                    coroutineIsPaused = true;
                    battleStates = BattleStates.SELECT_ENEMY;
                }

                break;
            case BattleStates.SELECT_PLAYER:                

                for (int i = 0; i < ActivePlayers.Count; i++)
                {
                    ActivePlayers[i].playerPanelBackground.color = Color.yellow;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //Set the player to green and active if clicked
                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                BM_Funcs.populateActionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                    }
                }

                break;
            case BattleStates.SELECT_ACTION:

                AnimHandler.animationController(activePlayer, "IsReady");

                //Instant redirect if not waiting for a mouse click
                BM_Funcs.redirectAction();

                activePlayer.playerPanel.GetComponent<Image>().color = Color.green;
                ActionPanel.SetActive(true);

                //Set the action panel portrait to the current player
                actionPanelArray = ActionPanel.GetComponentsInChildren<Image>();
                actionPanelArray[1].overrideSprite = activePlayer.PlayerPortrait;

                //RIGHT CLICK TO GO BACK
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    BM_Funcs.standIdle(activePlayer);
                    activePlayer = null;
                    ActionPanel.SetActive(false);
                    selectedCommand = null;
                    battleStates = BattleStates.SELECT_PLAYER;
                }

                //LEFT CLICK TO CHOOSE ACTION
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    panelChosen = false;

                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < activePlayer.playerOptions.Count; i++)
                        {
                            if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.playerOptions[i])
                            {                                                                
                                if (panelChosen == false)
                                {
                                    selectedCommand = activePlayer.playerOptions[i];
                                    BM_Funcs.instantiatedOptions[i].gameObject.GetComponent<Image>().color = Color.yellow;
                                    panelChosen = true;
                                }

                                if (selectedCommand == "Attack")
                                {
                                    EnmityManager.workOutProvisionalEnmity(activePlayer, "Attack");
                                }
                                else if (selectedCommand == "Magic")
                                {
                                    BM_Funcs.populateSpellOptionList();
                                }
                                else if (selectedCommand == "Weapon Skill")
                                {
                                    BM_Funcs.populateWeaponSkillOptionList();
                                } 
                                else if (selectedCommand == "Cast")
                                {
                                    ActionHandler.reportOutcome("PlayerFinishCast");
                                }   
                                
                                BM_Funcs.redirectAction();                                                                
                            }                            
                        }

                        //Left click another player to select them instead
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                BM_Funcs.resetChoice(ActivePlayers[i]);                                
                                BM_Funcs.populateActionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                    }
                }

                break;
            case BattleStates.SELECT_OPTION:

                OptionPanel.SetActive(true);                

                //RIGHT CLICK TO GO BACK
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                    {
                        BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = defaultBlueColor;
                    }

                    OptionPanel.SetActive(false);
                    selectedCommand = null;                    

                    battleStates = BattleStates.SELECT_ACTION;
                }

                //LEFT CLICK TO SELECT AN OPTION
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    foreach (RaycastResult result in results)
                    {
                        //Left click a Spell/WS/Ability to pick it
                        for (int i = 0; i < BM_Funcs.instantiatedSpellOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedSpellOptions[i])
                            {
                                for (int y = 0; y < activePlayer.spellBook.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.spellBook[y].name)
                                    {
                                        activePlayer.activeSpell = activePlayer.spellBook[y];                                        
                                    }
                                }

                                for (int x = 0; x < activePlayer.weaponSkills.Count; x++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.weaponSkills[x].name)
                                    {
                                        activePlayer.selectedWeaponSkill = activePlayer.weaponSkills[x];                                        
                                    }
                                }

                                BM_Funcs.instantiatedSpellOptions[i].GetComponentInChildren<Image>().color = Color.yellow;

                                if (activePlayer.activeSpell != null && activePlayer.activeSpell.isSupport == true)
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_FRIENDLY_TARGET;                                    
                                } else
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_TARGET;
                                }                                
                            }
                        }
                        //Left click another player to select them instead
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                BM_Funcs.resetChoice(ActivePlayers[i]);
                                activePlayer.activeSpell = null;
                                activePlayer.selectedWeaponSkill = null;
                                OptionPanel.SetActive(false);                                
                                BM_Funcs.populateActionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead
                        for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedOptions[i])
                            {
                                if (BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text != selectedCommand)
                                {
                                    for (int y = 0; y < BM_Funcs.instantiatedOptions.Count; y++)
                                    {
                                        BM_Funcs.instantiatedOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                    }

                                    BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = Color.yellow;                                    
                                    OptionPanel.SetActive(false);
                                    selectedCommand = BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;                                    
                                    battleStates = BattleStates.SELECT_ACTION;
                                }
                            }
                        }
                    }
                }

                break;
            case BattleStates.SELECT_FRIENDLY_TARGET:

                AnimHandler.animationController(activePlayer, "IsReady");

                //Color all potential target options yellow
                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    PlayersInBattle[i].playerPanelBackground.color = Color.yellow;                    
                }

                //Set Enmity numbers to show potential enmity result of healing
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    if (enmityFiguresNeedSetting)
                    {
                        //Set Enmity Figures and what enmity will go to potentially
                        EnmityManager.CreateEnmityNumber(activePlayer, EnemiesInBattle[i]);
                    }
                }                

                //Right click to go back to select option or select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (activePlayer.activeSpell != null)
                    {
                        activePlayer.activeSpell = null;
                        BM_Funcs.populateSpellOptionList();
                    }

                    for (int i = 0; i < PlayersInBattle.Count; i++)
                    {
                        if (ActivePlayers.Contains(PlayersInBattle[i]) == false)
                        {
                            PlayersInBattle[i].playerPanelBackground.color = defaultColor;
                        }
                    }

                    battleStates = BattleStates.SELECT_OPTION;
                }

                //LEFT CLICK TO SELECT AN ALLY
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < PlayersInBattle.Count; i++)
                        {
                            if (!ActivePlayers.Contains(PlayersInBattle[i]))
                            {
                                PlayersInBattle[i].playerPanelBackground.GetComponent<Image>().color = defaultColor;
                            }                            

                            if (result.gameObject == PlayersInBattle[i].playerPanel)
                            {
                                for (int y = 0; y < PlayersInBattle.Count; y++)
                                {
                                    if (PlayersInBattle[y].playerPanel == PlayersInBattle[i].playerPanel)
                                    {
                                        activePlayer.PlayerTargetID = PlayersInBattle[y].name;
                                    }
                                }

                                if (selectedCommand == "Magic")
                                {
                                    activePlayer.isCastingSpell = true;
                                    //Timers_Log.addToTimersLog(activePlayer);
                                }                                

                                battleStates = BattleStates.RESOLVE_ACTION;
                            }
                        }

                        //Left click another action to select that instead
                        for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedOptions[i])
                            {
                                if (BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text == "Magic" && OptionPanel.activeSelf != true)
                                {
                                    activePlayer.selectedWeaponSkill = null;
                                    activePlayer.activeSpell = null;

                                    BM_Funcs.populateSpellOptionList();
                                    battleStates = BattleStates.SELECT_OPTION;
                                }
                                else if (BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text == "Weapon Skill" && OptionPanel.activeSelf != true)
                                {
                                    activePlayer.selectedWeaponSkill = null;
                                    activePlayer.activeSpell = null;

                                    BM_Funcs.populateWeaponSkillOptionList();
                                    battleStates = BattleStates.SELECT_OPTION;
                                }

                                for (int y = 0; y < BM_Funcs.instantiatedOptions.Count; y++)
                                {
                                    BM_Funcs.instantiatedOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                }

                                selectedCommand = BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = Color.yellow;

                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another option to select that instead
                        for (int i = 0; i < BM_Funcs.instantiatedSpellOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedSpellOptions[i])
                            {
                                for (int x = 0; x < BM_Funcs.instantiatedSpellOptions.Count; x++)
                                {
                                    BM_Funcs.instantiatedSpellOptions[x].GetComponentInChildren<Image>().color = defaultBlueColor;
                                }

                                for (int y = 0; y < activePlayer.weaponSkills.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.weaponSkills[y].name)
                                    {
                                        activePlayer.selectedWeaponSkill = activePlayer.weaponSkills[y];
                                    }
                                }
                                for (int y = 0; y < activePlayer.spellBook.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.spellBook[y].name)
                                    {
                                        activePlayer.activeSpell = activePlayer.spellBook[y];
                                    }
                                }

                                BM_Funcs.instantiatedSpellOptions[i].GetComponentInChildren<Image>().color = Color.yellow;
                                
                                if (activePlayer.activeSpell != null && activePlayer.activeSpell.isSupport == true)
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_FRIENDLY_TARGET;
                                }
                                else
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_TARGET;
                                }
                            }
                        }
                    }
                }

                break;

            case BattleStates.SELECT_TARGET:
                
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    EnemiesInBattle[i].enemyPanelBackground.color = Color.yellow;

                    if (enmityFiguresNeedSetting)
                    {
                        //Set Enmity Figures and what enmity will go to potentially
                        EnmityManager.CreateEnmityNumber(activePlayer, EnemiesInBattle[i]);
                        //Set Provisional Enmity figure for preview based on what the player intends to do
                        EnmityManager.workOutProvisionalEnmity(activePlayer, selectedCommand);
                    }                    
                }                

                enmityFiguresNeedSetting = false;

                //Right click to go back to select option or select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    AnimHandler.animationController(activePlayer);

                    if (OptionPanel.activeSelf == true)
                    {
                        for (int i = 0; i < EnemiesInBattle.Count; i++)
                        {
                            EnemiesInBattle[i].enemyPanelBackground.color = defaultColor;
                        }

                        //This checks whether a WS or Spell is selected for populating the options (just using null isn't working, hence TP and casttime > 0 check...)
                        if (activePlayer.selectedWeaponSkill != null && activePlayer.selectedWeaponSkill.wsAnimTimer > 0)
                        {                            
                            activePlayer.selectedWeaponSkill = null;                            
                            BM_Funcs.populateWeaponSkillOptionList();
                        }
                        if (activePlayer.activeSpell != null && activePlayer.activeSpell.castTime > 0)
                        {                            
                            activePlayer.activeSpell = null;                            
                            BM_Funcs.populateSpellOptionList();
                        }

                        battleStates = BattleStates.SELECT_OPTION;
                    }
                    else
                    {
                        AnimHandler.animationController(activePlayer);

                        for (int i = 0; i < EnemiesInBattle.Count; i++)
                        {
                            EnemiesInBattle[i].enemyPanelBackground.color = defaultColor;
                        }
                        for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                        {
                            BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = defaultBlueColor;
                        }
                        selectedCommand = null;

                        battleStates = BattleStates.SELECT_ACTION;
                    }                    
                }


                //LEFT CLICK TO SELECT ENEMY
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    EnmityManager.enmityFigures.Clear();

                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < EnemyPanels.Count; i++)
                        {
                            EnemyPanels[i].GetComponent<Image>().color = defaultColor;

                            if (result.gameObject == EnemyPanels[i])
                            {
                                for (int y = 0; y < EnemiesInBattle.Count; y++)
                                {
                                    if (EnemiesInBattle[y].enemyPanel == EnemyPanels[i])
                                    {
                                        activePlayer.PlayerTargetID = EnemiesInBattle[y].EnemyName;                                                                                
                                    }
                                }

                                if (selectedCommand == "Magic")
                                {
                                    activePlayer.isCastingSpell = true;
                                    //Timers_Log.addToTimersLog(activePlayer);
                                }
                                else if (selectedCommand == "Attack")
                                {
                                    ActionHandler.reportOutcome("PlayerAttack");
                                }
                                else if (selectedCommand == "Weapon Skill")
                                {
                                    combo_Manager.SetUpPanel();
                                    activePlayer.selectedWeaponSkill.spentAttacks = activePlayer.selectedWeaponSkill.totalAttacks;
                                    combo_Manager.PlayerWeaponskill(activePlayer.selectedWeaponSkill, activePlayer, playerTarget);
                                    ActionHandler.CreateDamagePopUp(activePlayer.battleSprite.transform.position, activePlayer.selectedWeaponSkill.name, Color.magenta);
                                    ActionHandler.reportOutcome("Weapon Skill");
                                }

                                EnemyPanels[i].GetComponent<Image>().color = Color.red;

                                battleStates = BattleStates.RESOLVE_ACTION;
                            }
                        }

                        //Left click another player to select them instead
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                BM_Funcs.resetChoice(ActivePlayers[i]);
                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);
                                BM_Funcs.populateActionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead
                        for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedOptions[i])
                            {
                                if (BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text == "Magic" && OptionPanel.activeSelf != true)
                                {
                                    activePlayer.selectedWeaponSkill = null;
                                    activePlayer.activeSpell = null;

                                    BM_Funcs.populateSpellOptionList();
                                    battleStates = BattleStates.SELECT_OPTION;
                                }
                                else if (BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text == "Weapon Skill" && OptionPanel.activeSelf != true)
                                {
                                    activePlayer.selectedWeaponSkill = null;
                                    activePlayer.activeSpell = null;

                                    BM_Funcs.populateWeaponSkillOptionList();
                                    battleStates = BattleStates.SELECT_OPTION;
                                }

                                for (int y = 0; y < BM_Funcs.instantiatedOptions.Count; y++)
                                {
                                    BM_Funcs.instantiatedOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                }

                                selectedCommand = BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = Color.yellow;

                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another option to select that instead
                        for (int i = 0; i < BM_Funcs.instantiatedSpellOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedSpellOptions[i])
                            {                                
                                for (int x = 0; x < BM_Funcs.instantiatedSpellOptions.Count; x++)
                                {
                                    BM_Funcs.instantiatedSpellOptions[x].GetComponentInChildren<Image>().color = defaultBlueColor;
                                }

                                for (int y = 0; y < activePlayer.weaponSkills.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.weaponSkills[y].name)
                                    {
                                        activePlayer.selectedWeaponSkill = activePlayer.weaponSkills[y];
                                    }
                                }
                                for (int y = 0; y < activePlayer.spellBook.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.spellBook[y].name)
                                    {
                                        activePlayer.activeSpell = activePlayer.spellBook[y];
                                    }
                                }

                                BM_Funcs.instantiatedSpellOptions[i].GetComponentInChildren<Image>().color = Color.yellow;
                                
                                if (activePlayer.activeSpell != null && activePlayer.activeSpell.isSupport == true)
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_FRIENDLY_TARGET;
                                }
                                else
                                {
                                    enmityFiguresNeedSetting = true;
                                    battleStates = BattleStates.SELECT_TARGET;
                                }
                            }
                        }
                    }
                }

                break;
            case BattleStates.CHANGE_ROW:

                //If clicking a row icon, set that row icon as the target and start a hands up animation
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    AnimHandler.animationController(activePlayer);

                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < RowChangeIcons.Count; i++)
                        {
                            if (result.gameObject == RowChangeIcons[i])
                            {
                                RowToSwitch = Rows[i];
                                AnimHandler.animationController(activePlayer, "IsCasting");

                                for (int y = 0; y < RowChangeIcons.Count; y++)
                                {
                                    RowChangeIcons[y].SetActive(false);
                                }

                                rowSelected = true;
                            }
                        }
                        //Left click another player to select them instead
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                BM_Funcs.resetChoice(ActivePlayers[i]);
                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);

                                for (int y = 0; y < RowChangeIcons.Count; y++)
                                {
                                    RowChangeIcons[y].SetActive(false);
                                }

                                BM_Funcs.populateActionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead                        
                        for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                        {
                            if (result.gameObject == BM_Funcs.instantiatedOptions[i])
                            {
                                for (int y = 0; y < BM_Funcs.instantiatedOptions.Count; y++)
                                {
                                    BM_Funcs.instantiatedOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                    selectedCommand = BM_Funcs.instantiatedOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                }

                                BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = Color.yellow;

                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);

                                for (int y = 0; y < RowChangeIcons.Count; y++)
                                {
                                    RowChangeIcons[y].SetActive(false);
                                }

                                BM_Funcs.populateSpellOptionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                    }
                }


                break;
            case BattleStates.RESOLVE_ACTION:                                

                AnimHandler.animationController(activePlayer);
                ActionHandler.resolveAction(selectedCommand);

                break;
            case BattleStates.SELECT_ENEMY:

                for (int i = 0; i < ActiveEnemies.Count; i++)
                {
                    activeEnemy = ActiveEnemies[i];
                }

                battleStates = BattleStates.SELECT_ENEMY_ACTION;

                break;
            case BattleStates.SELECT_ENEMY_ACTION:

                if (activeEnemy.isCastingSpell == false)
                {
                    int randomActionNo = Random.Range(3, 4);

                    switch (randomActionNo)
                    {
                        case 1:
                            selectedCommand = "EnemyAttack";
                            break;
                        case 2:
                            activeEnemy.activeSpell = SpellManager.Poison;
                            selectedCommand = "EnemySpell";
                            break;
                        case 3:
                            activeEnemy.activeSpell = SpellManager.Sleep;
                            selectedCommand = "EnemySpell";
                            break;
                        case 4:
                            activeEnemy.activeSpell = SpellManager.Poisonga;
                            selectedCommand = "EnemySpell";
                            break;
                        case 5:
                            activeEnemy.activeSpell = SpellManager.Firaga;
                            selectedCommand = "EnemySpell";
                            break;
                        default:
                            break;
                    }
                                        
                    battleStates = BattleStates.SELECT_ENEMY_TARGET;

                } else if (activeEnemy.isCastingSpell == true)
                {
                    selectedCommand = "EnemyResolveSpell";
                    ActionHandler.reportOutcome("EnemyFinishCast");                    
                    StartCoroutine(BM_Enums.waitForEnemyCastAnimation(activeEnemy));
                    battleStates = BattleStates.RESOLVE_ENEMY_TURN;
                }

                break;
            case BattleStates.SELECT_ENEMY_TARGET:
                
                StartCoroutine(BM_Enums.waitForEnemyReadyAnimation(activeEnemy));
                activeEnemy.enemyReadyAnimCoroutineIsPaused = false;                

                if (activeEnemy.enemyReadyAnimIsDone == true)
                {
                    EnmityManager.determineAttackTargetFromEnmity(activeEnemy);
                    activeEnemy.enemyReadyAnimCoroutineIsPaused = true;
                    activeEnemy.enemyReadyAnimIsDone = false;
                    AnimHandler.enemyAnimationController(activeEnemy);                    

                    if (selectedCommand == "EnemyAttack")
                    {                        
                        ActionHandler.reportOutcome("EnemyAttack");

                        battleStates = BattleStates.RESOLVE_ENEMY_TURN;
                    }
                    else if (selectedCommand == "EnemySpell")
                    {
                        ActionHandler.reportOutcome("EnemyStartCast");
                        battleStates = BattleStates.RESOLVE_ENEMY_TURN;
                    }
                }

                break;
            case BattleStates.RESOLVE_ENEMY_TURN:

                if (selectedCommand == "EnemyAttack")
                {
                    BM_Funcs.setPlayerOrEnemyTargetFromID(null, activeEnemy);
                    activeEnemy.enemyAttackAnimCoroutineIsPaused = false;
                    StartCoroutine(BM_Enums.waitForEnemyAttackAnimation(activeEnemy));
                    AnimHandler.enemyAnimationController(activeEnemy, "IsAttacking");
                    AnimHandler.animationController(enemyTarget, "TakeDamage");                    

                    if (activeEnemy.enemyAttackAnimIsDone == true)
                    {
                        activeEnemy.enemyAttackAnimIsDone = false;
                        activeEnemy.enemyAttackAnimCoroutineIsPaused = true;                        
                        AnimHandler.animationController(enemyTarget);
                        AnimHandler.enemyAnimationController(activeEnemy);
                        activeEnemy.speedTotal -= 100f;
                        activeEnemy.enemyPanel.GetComponent<Image>().color = defaultColor;
                        activeEnemy.EnemyTargetID = null;
                        enemyTarget = null;
                        ActiveEnemies.Remove(activeEnemy);
                        activeEnemy.enemyAttackAnimCoroutineIsPaused = true;
                        activeEnemy.enemyReadyAnimCoroutineIsPaused = true;
                        activeEnemy.enemyAttackAnimIsDone = false;
                        activeEnemy.enemyReadyAnimIsDone = false;
                        activeEnemy = null;
                        selectedCommand = null;

                        if (ActiveEnemies.Count == 0)
                        {
                            returningStarting = true;
                            startRoutinesGoingAgain = true;
                            battleStates = BattleStates.DEFAULT;
                        }
                        else
                        {
                            battleStates = BattleStates.SELECT_ENEMY;
                        }
                    }
                }

                if (selectedCommand == "EnemySpell")
                {                    
                    activeEnemy.constantAnimationStates.Add("IsChanting");
                    AnimHandler.enemyAnimationController(activeEnemy, "IsChanting");
                    activeEnemy.isCastingSpell = true;                    
                    activeEnemy.enemyCastBar.SetActive(true);
                    activeEnemy.castSpeedTotal = activeEnemy.activeSpell.castTime;
                    activeEnemy.speedTotal -= 100f;
                    activeEnemy.enemyPanel.GetComponent<Image>().color = defaultColor;
                    //Timers_Log.addToTimersLog(null, activeEnemy);                    
                    ActiveEnemies.Remove(activeEnemy);
                    activeEnemy = null;
                    selectedCommand = null;

                    if (ActiveEnemies.Count == 0)
                    {
                        returningStarting = true;
                        startRoutinesGoingAgain = true;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_ENEMY;
                    }
                }

                if (selectedCommand == "EnemyResolveSpell")
                {                    
                    AnimHandler.enemyAnimationController(activeEnemy, "IsCasting");
                    activeEnemy.enemyCastAnimCoroutineIsPaused = false;

                    if (activeEnemy.enemyCastAnimIsDone)
                    {
                        ActionHandler.enemySpellReportFinished = false;
                        activeEnemy.enemyCastAnimIsDone = false;
                        activeEnemy.enemyCastAnimCoroutineIsPaused = true;                                                
                        AnimHandler.enemyAnimationController(activeEnemy);
                        activeEnemy.isCastingSpell = false;
                        activeEnemy.enemyCastBar.SetActive(false);
                        activeEnemy.castSpeedTotal = 0f;
                        activeEnemy.enemyPanel.GetComponent<Image>().color = defaultColor;
                        ActiveEnemies.Remove(activeEnemy);
                        activeEnemy.EnemyTargetID = null;
                        enemyTarget = null;
                        activeEnemy = null;
                        selectedCommand = null;

                        if (ActiveEnemies.Count == 0)
                        {
                            returningStarting = true;
                            startRoutinesGoingAgain = true;
                            battleStates = BattleStates.DEFAULT;
                        }
                        else
                        {
                            battleStates = BattleStates.SELECT_ENEMY;
                        }
                    }
                }

                break;
            default:
                break;
        }
    }

    //Functions that have to be in Battle Manager because of motion/update functionality
    void startSpeedCoroutines()
    {
        coroutineIsPaused = false;

        for (int i = 0; i < PlayersInBattle.Count; i++)
        {
            StartCoroutine(BM_Enums.updatePlayerSpeedBars(PlayersInBattle[i]));
        }
        for (int i = 0; i < EnemiesInBattle.Count; i++)
        {
            StartCoroutine(BM_Enums.updateEnemySpeedBars(EnemiesInBattle[i]));
        }
    }
}