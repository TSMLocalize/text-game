using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager : MonoBehaviour
{       
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;
    public Action_Handler ActionHandler;
    public Timers_Log Timers_Log;
    public Combo_Manager combo_Manager;
    public float speed;
    public bool startRoutinesGoingAgain;
    public bool stepForward;
    public bool attackAnimIsDone;
    public bool WSAnimIsDone;
    public bool SCAnimIsDone;
    public bool castAnimIsDone;
    public bool attackAnimCoroutineIsPaused;
    public bool WSAnimCoroutineIsPaused;
    public bool SCAnimCoroutineIsPaused;
    public bool castAnimCoroutineIsPaused;
    public bool rowSelected;
    public bool isSwitchingWithOtherPlayer;
    public bool panelChosen;
    public List<GameObject> Rows;
    public List<GameObject> RowChangeIcons;
    public GameObject RowToSwitch;
    public Player playerToSwitchRowWith;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public List<Player> PlayersInBattle;
    public List<Player> ActivePlayers;
    public Player activePlayer;
    public List<Enemy> EnemiesInBattle;
    public List<Enemy> ActiveEnemies;
    public Enemy activeEnemy;
    public List<GameObject> PlayerPanels;
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
    public List<TextMeshProUGUI> PlayerPanelTexts;
    public List<TextMeshProUGUI> EnemyPanelTexts;    
    public List<GameObject> SpellOptions;
    public GameObject UICanvas;
    public GameObject ActionPanel;
    public GameObject OptionPanel;
    public GameObject PartyManager;
    public GameObject EnemyManager;
    public GameObject RowManager;
    public Spells SpellManager;
    public bool coroutineIsPaused = false;
    public bool returningStarting = true;
    public string selectedCommand = null;
    public Color defaultColor;
    public Color defaultBlueColor;
    public Image[] spellOptionsArray;
    public Image[] wsOptionsArray;
    public Image[] playerPanelArray;
    public Image[] enemyPanelArray;
    public Image[] actionPanelArray;
    public Enemy playerTarget;
    public Player enemyTarget;

    public enum BattleStates
    {
        DEFAULT,
        SELECT_PLAYER,
        SELECT_ACTION,
        SELECT_OPTION,
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

    // Start is called before the first frame update
    void Start()
    {
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        ActionHandler = GetComponent<Action_Handler>();
        Timers_Log = GetComponent<Timers_Log>();
        combo_Manager = GetComponent<Combo_Manager>();

        BM_Funcs.setupCharacters();

        ColorUtility.TryParseHtmlString("#010078", out defaultBlueColor);
        defaultColor = ActionPanel.GetComponent<Image>().color;

        //Start SpeedBar coroutines for players and enemies
        startSpeedCoroutines();

        castAnimCoroutineIsPaused = true;
        castAnimIsDone = false;
        attackAnimCoroutineIsPaused = true;
        attackAnimIsDone = false;
        WSAnimCoroutineIsPaused = true;
        WSAnimIsDone = false;

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = UICanvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        battleStates = BattleStates.DEFAULT;

    }

    // Update is called once per frame
    void Update()
    {
        if (stepForward)
        {
            speed = 4.0f;

            //Transform the Sprite forward a set distance and set walking animation
            BM_Funcs.animationController(activePlayer, "IsWalking");

            float step = speed * Time.deltaTime;
            activePlayer.battleSprite.transform.position = Vector3.MoveTowards(activePlayer.battleSprite.transform.position, activePlayer.target, step);

            if (activePlayer.battleSprite.transform.position == activePlayer.target)
            {
                BM_Funcs.animationController(activePlayer);

                stepForward = false;
            }
        }

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        BM_Funcs.updateEnemyUIBars();

        BM_Funcs.updatePlayerUIBars();
        
        if (Timers_Log.instantiatedTimersOptions.Count >= 1)
        {
            Timers_Log.updateTimersLog();
        }        

        switch (battleStates)
        {
            case BattleStates.DEFAULT:

                attackAnimIsDone = false;
                castAnimIsDone = false;

                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    EnemiesInBattle[i].enemyAttackAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyReadyAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyCastAnimCoroutineIsPaused = true;
                    EnemiesInBattle[i].enemyAttackAnimIsDone = false;
                    EnemiesInBattle[i].enemyReadyAnimIsDone = false;
                    EnemiesInBattle[i].enemyCastAnimIsDone = false;
                }

                if (startRoutinesGoingAgain)
                {
                    startSpeedCoroutines();
                    StartCoroutine(combo_Manager.updateTimeRemaining());
                    startRoutinesGoingAgain = false;
                }

                //Check if a player is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < PlayersInBattle.Count; i++)
                {                   
                    if (PlayersInBattle[i].speedTotal >= 100 || (PlayersInBattle[i].isCastingSpell && PlayersInBattle[i].castSpeedTotal <= 0))
                    {
                        if (EnemiesInBattle[i].isCastingSpell != true)
                        {
                            BM_Funcs.animationController(PlayersInBattle[i], "IsReady");
                            PlayersInBattle[i].constantAnimationState = "IsReady";
                            PlayersInBattle[i].hasConstantAnimationState = true;
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
                            BM_Funcs.enemyAnimationController(EnemiesInBattle[i], "IsReady");
                            EnemiesInBattle[i].constantAnimationState = "IsReady";
                            EnemiesInBattle[i].hasConstantAnimationState = true;
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

                attackAnimIsDone = false;
                castAnimIsDone = false;

                for (int i = 0; i < ActivePlayers.Count; i++)
                {
                    ActivePlayers[i].playerPanelBackground.color = Color.yellow;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
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

                attackAnimIsDone = false;
                castAnimIsDone = false;
                WSAnimIsDone = false;                

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

                                if (selectedCommand == "Magic")
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
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;                                
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                selectedCommand = null;
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

                                battleStates = BattleStates.SELECT_TARGET;
                            }
                        }
                        //Left click another player to select them instead
                        for (int i = 0; i < ActivePlayers.Count; i++)
                        {
                            if (result.gameObject == ActivePlayers[i].playerPanel)
                            {
                                BM_Funcs.resetChoicePanel();
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
                                BM_Funcs.animationController(activePlayer);
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                selectedCommand = null;
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
            case BattleStates.SELECT_TARGET:

                BM_Funcs.animationController(activePlayer, "IsReady");

                //Color all potential target options yellow
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    EnemiesInBattle[i].enemyPanelBackground.color = Color.yellow;
                }

                //Right click to go back to select option or select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (OptionPanel.activeSelf == true)
                    {
                        if (activePlayer.selectedWeaponSkill != null)
                        {
                            activePlayer.selectedWeaponSkill = null;
                            BM_Funcs.populateWeaponSkillOptionList();
                        }
                        else if (activePlayer.activeSpell != null)
                        {
                            activePlayer.activeSpell = null;
                            BM_Funcs.populateSpellOptionList();
                        }

                        for (int i = 0; i < EnemiesInBattle.Count; i++)
                        {
                            EnemiesInBattle[i].enemyPanelBackground.color = defaultColor;
                        }

                        battleStates = BattleStates.SELECT_OPTION;
                    }
                    else
                    {
                        BM_Funcs.animationController(activePlayer);

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
                                    Timers_Log.addToTimersLog(activePlayer);
                                }
                                else if (selectedCommand == "Attack")
                                {
                                    ActionHandler.reportOutcome("PlayerAttack");                                    
                                }
                                else if (selectedCommand == "Weapon Skill")
                                {
                                    ActionHandler.reportOutcome("Weapon Skill");
                                    combo_Manager.SetUpPanel();
                                    combo_Manager.PlayerWeaponskill(activePlayer.selectedWeaponSkill, activePlayer, playerTarget);
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
                                BM_Funcs.resetChoicePanel();
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
                                BM_Funcs.animationController(activePlayer);
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                selectedCommand = null;
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
                                battleStates = BattleStates.SELECT_TARGET;
                            }
                        }
                    }
                }

                break;
            case BattleStates.CHANGE_ROW:

                //Populate Row Change Icons, minus the activeplayer's
                if (!rowSelected)
                {
                    BM_Funcs.animationController(activePlayer, "IsReady");

                    for (int i = 0; i < RowChangeIcons.Count; i++)
                    {
                        if (RowChangeIcons[i] != activePlayer.currentRowPositionIcon)
                        {
                            RowChangeIcons[i].SetActive(true);
                        }
                    }
                }

                //Right click to go back to select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    BM_Funcs.animationController(activePlayer);

                    for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
                    {
                        BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = defaultBlueColor;
                    }

                    for (int y = 0; y < RowChangeIcons.Count; y++)
                    {
                        RowChangeIcons[y].SetActive(false);
                    }

                    selectedCommand = null;

                    battleStates = BattleStates.SELECT_ACTION;
                }

                //If clicking a row icon, set that row icon as the target and start a hands up animation
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    BM_Funcs.animationController(activePlayer);

                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < RowChangeIcons.Count; i++)
                        {
                            if (result.gameObject == RowChangeIcons[i])
                            {
                                RowToSwitch = Rows[i];
                                BM_Funcs.animationController(activePlayer, "IsCasting");

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
                                BM_Funcs.resetChoicePanel();
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
                                BM_Funcs.animationController(activePlayer);
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                selectedCommand = null;
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

                //Once a row icon is clicked, check if there's a player there and switch with them
                if (rowSelected == true)
                {
                    speed = 8.0f;

                    float step = speed * Time.deltaTime;
                    activePlayer.battleSprite.transform.position = Vector3.MoveTowards(activePlayer.battleSprite.transform.position, RowToSwitch.transform.position, step);

                    for (int i = 0; i < PlayersInBattle.Count; i++)
                    {
                        if (PlayersInBattle[i].currentRowPosition == RowToSwitch)
                        {
                            isSwitchingWithOtherPlayer = true;

                            playerToSwitchRowWith = PlayersInBattle[i];

                            //Set hands up animation
                            BM_Funcs.animationController(PlayersInBattle[i], "IsCasting");

                            PlayersInBattle[i].battleSprite.transform.position = Vector3.MoveTowards(PlayersInBattle[i].battleSprite.transform.position,
                                activePlayer.position, step);
                        }
                    }

                    //Ensure both players are in position (if switching places)
                    if (isSwitchingWithOtherPlayer)
                    {
                        if (activePlayer.battleSprite.transform.position == RowToSwitch.transform.position &&
                            playerToSwitchRowWith.battleSprite.transform.position == activePlayer.position)
                        {
                            BM_Funcs.animationController(playerToSwitchRowWith);
                            BM_Funcs.animationController(activePlayer);
                            rowSelected = false;
                        }
                    }
                    //Ensure one player is in position (if not switching places)
                    else
                    {
                        if (activePlayer.battleSprite.transform.position == RowToSwitch.transform.position)
                        {
                            BM_Funcs.animationController(activePlayer);
                            rowSelected = false;
                        }
                    }

                    //Finish up 
                    if (rowSelected == false)
                    {
                        //reassign 'position' to the new position(s), reset new display layer order priority
                        BM_Funcs.updateRowPositions();
                        BM_Funcs.AssignRows();
                        RowToSwitch = null;
                        battleStates = BattleStates.RESOLVE_ACTION;
                    }
                }


                break;
            case BattleStates.RESOLVE_ACTION:

                if (activePlayer.constantAnimationState == "IsReady")
                {
                    activePlayer.constantAnimationState = null;
                    activePlayer.hasConstantAnimationState = false;
                }

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
                    int randomActionNo = Random.Range(1, 3);

                    if (randomActionNo == 1)
                    {
                        selectedCommand = "EnemyAttack";
                    }
                    else if (randomActionNo == 2)
                    {
                        activeEnemy.activeSpell = SpellManager.Fire;
                        selectedCommand = "EnemySpell";
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

                int randomEnemyTargetNo = Random.Range(0, PlayersInBattle.Count);

                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    if (randomEnemyTargetNo == i)
                    {
                        activeEnemy.EnemyTargetID = PlayersInBattle[i].name;
                    }
                }

                StartCoroutine(BM_Enums.waitForEnemyReadyAnimation(activeEnemy));
                activeEnemy.enemyReadyAnimCoroutineIsPaused = false;

                if (activeEnemy.enemyReadyAnimIsDone == true)
                {
                    activeEnemy.enemyReadyAnimCoroutineIsPaused = true;
                    activeEnemy.enemyReadyAnimIsDone = false;
                    BM_Funcs.enemyAnimationController(activeEnemy);

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
                    BM_Funcs.enemyAnimationController(activeEnemy, "IsAttacking");
                    BM_Funcs.animationController(enemyTarget, "TakeDamage");                    

                    if (activeEnemy.enemyAttackAnimIsDone == true)
                    {
                        activeEnemy.enemyAttackAnimIsDone = false;
                        activeEnemy.enemyAttackAnimCoroutineIsPaused = true;
                        activeEnemy.constantAnimationState = null;
                        activeEnemy.hasConstantAnimationState = false;
                        BM_Funcs.animationController(enemyTarget);
                        BM_Funcs.enemyAnimationController(activeEnemy);
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
                    activeEnemy.constantAnimationState = null;
                    activeEnemy.hasConstantAnimationState = false;
                    activeEnemy.constantAnimationState = "IsChanting";
                    BM_Funcs.enemyAnimationController(activeEnemy, "IsChanting");
                    activeEnemy.isCastingSpell = true;
                    activeEnemy.hasConstantAnimationState = true;
                    activeEnemy.enemyCastBar.SetActive(true);
                    activeEnemy.castSpeedTotal = activeEnemy.activeSpell.castTime;
                    activeEnemy.speedTotal -= 100f;
                    activeEnemy.enemyPanel.GetComponent<Image>().color = defaultColor;
                    Timers_Log.addToTimersLog(null, activeEnemy);                    
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
                    activeEnemy.constantAnimationState = null;
                    activeEnemy.hasConstantAnimationState = false;
                    BM_Funcs.enemyAnimationController(activeEnemy, "IsCasting");
                    activeEnemy.enemyCastAnimCoroutineIsPaused = false;

                    if (activeEnemy.enemyCastAnimIsDone)
                    {
                        ActionHandler.enemySpellReportFinished = false;
                        activeEnemy.enemyCastAnimIsDone = false;
                        activeEnemy.enemyCastAnimCoroutineIsPaused = true;                                                
                        BM_Funcs.enemyAnimationController(activeEnemy);
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