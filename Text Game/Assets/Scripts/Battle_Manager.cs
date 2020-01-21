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
    public float speed;
    public bool startRoutinesGoingAgain;
    public bool stepForward;        
    public bool attackAnimIsDone;
    public bool castAnimIsDone;
    public bool attackAnimCoroutineIsPaused;
    public bool castAnimCoroutineIsPaused;
    public bool enemyReadyAnimIsDone;
    public bool enemyAttackAnimIsDone;
    public bool enemyCastAnimIsDone;
    public bool enemyReadyAnimCoroutineIsPaused;
    public bool enemyAttackAnimCoroutineIsPaused;
    public bool enemyCastAnimCoroutineIsPaused;
    public bool rowSelected;
    public bool isSwitchingWithOtherPlayer;
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
    public List<GameObject> EnemyPanels;
    public List<GameObject> EnemySpeedBars;
    public List<TextMeshProUGUI> EnemySpeedBarTexts;
    public List<GameObject> PlayerOptions;
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
    public Image[] optionsArray;
    public Image[] playerPanelArray;
    public Image[] enemyPanelArray;
    public Image[] actionPanelArray;    

    public enum BattleStates
    {
        DEFAULT,
        SELECT_PLAYER,
        SELECT_ACTION,
        SELECT_OPTION,
        SELECT_TARGET,   
        CHANGE_ROW,
        RESOLVE_ACTION,
        RESOLVE_SPELL,
        SELECT_ENEMY,
        SELECT_ENEMY_ACTION,
        SELECT_ENEMY_TARGET,
        ENEMY_ATTACK,
        RESOLVE_ENEMY_TURN        
    }

    public BattleStates battleStates;        

    // Start is called before the first frame update
    void Start()
    {
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();

        BM_Funcs.setupCharacters();
        
        ColorUtility.TryParseHtmlString("#010078", out defaultBlueColor);
        defaultColor = ActionPanel.GetComponent<Image>().color;

        //Start SpeedBar coroutines for players and enemies
        startSpeedCoroutines();

        castAnimCoroutineIsPaused = true;
        castAnimIsDone = false;
        attackAnimCoroutineIsPaused = true;
        attackAnimIsDone = false;

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

        void standIdle(Player playerToIdle)
        {
            playerToIdle.battleSprite.transform.position = playerToIdle.position;             
        }

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        BM_Funcs.updateEnemyUIBars();

        BM_Funcs.updatePlayerUIBars();

        switch (battleStates)
        {
            case BattleStates.DEFAULT:

                enemyReadyAnimIsDone = false;
                enemyAttackAnimIsDone = false;
                enemyCastAnimIsDone = false;
                attackAnimIsDone = false;
                castAnimIsDone = false;

                if (startRoutinesGoingAgain)
                {
                    startSpeedCoroutines();
                    startRoutinesGoingAgain = false;
                }

                //Check if a player is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    if (PlayersInBattle[i].speedTotal >= 100 || (PlayersInBattle[i].isCastingSpell && PlayersInBattle[i].castSpeedTotal <= 0))
                    {
                                               
                        ActivePlayers.Add(PlayersInBattle[i]);
                    }
                }
                //Check if an enemy is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    if (EnemiesInBattle[i].speedTotal >= 100)
                    {                        
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

                                battleStates = BattleStates.SELECT_ACTION;
                                
                            }
                        }                        
                    }
                }

                break;
            case BattleStates.SELECT_ACTION:

                //Populate action panel with active player's actions
                BM_Funcs.populateActionList();

                attackAnimIsDone = false;
                castAnimIsDone = false;

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
                    standIdle(activePlayer);
                    activePlayer = null;
                    ActionPanel.SetActive(false);
                    selectedCommand = null;
                    battleStates = BattleStates.SELECT_PLAYER;
                }

                //LEFT CLICK TO CHOOSE ACTION
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < activePlayer.playerOptions.Count; i++)
                        {
                            if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.playerOptions[i])
                            {
                                PlayerOptions[i].GetComponent<Image>().color = Color.yellow;
                                selectedCommand = activePlayer.playerOptions[i];

                                if (selectedCommand == "Magic")
                                {
                                    BM_Funcs.populateSpellOptionList();
                                    //populateSpellOptionList();
                                }

                                //Chooses the switch statement based on whether Attack, Magic etc. selected
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
                    for (int i = 0; i < PlayerOptions.Count; i++)
                    {
                        PlayerOptions[i].GetComponent<Image>().color = defaultBlueColor;
                    }

                    OptionPanel.SetActive(false);
                    selectedCommand = null;
                    BM_Funcs.clearSpellOptionList();

                    battleStates = BattleStates.SELECT_ACTION;
                }

                //LEFT CLICK TO SELECT AN OPTION
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    foreach (RaycastResult result in results)
                    {
                        //Left click a spell option to pick it
                        for (int i = 0; i < SpellOptions.Count; i++)
                        {
                            if (result.gameObject == SpellOptions[i])
                            {
                                for (int y = 0; y < activePlayer.spellBook.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.spellBook[y].name)
                                    {
                                        activePlayer.activeSpell = activePlayer.spellBook[y];
                                    }
                                }

                                SpellOptions[i].GetComponentInChildren<Image>().color = Color.magenta;                                
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
                                OptionPanel.SetActive(false);
                                selectedCommand = null;
                                BM_Funcs.clearSpellOptionList();
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead
                        for (int i = 0; i < PlayerOptions.Count; i++)
                        {
                            if (result.gameObject == PlayerOptions[i])
                            {
                                if (PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text != "Magic")
                                {
                                    for (int y = 0; y < PlayerOptions.Count; y++)
                                    {
                                        PlayerOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                    }
                                    PlayerOptions[i].GetComponent<Image>().color = Color.yellow;
                                    OptionPanel.SetActive(false);
                                    selectedCommand = PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                    BM_Funcs.clearSpellOptionList();
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

                //Right click to go back to select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    BM_Funcs.animationController(activePlayer);

                    for (int i = 0; i < EnemiesInBattle.Count; i++)
                    {
                        EnemiesInBattle[i].enemyPanelBackground.color = defaultColor;
                    }
                    for (int i = 0; i < PlayerOptions.Count; i++)
                    {
                        PlayerOptions[i].GetComponent<Image>().color = defaultBlueColor;
                    }
                    selectedCommand = null;
                                        
                    battleStates = BattleStates.SELECT_ACTION;

                    //Right click to cancel selected spell option
                    if (OptionPanel.activeSelf == true)
                    {
                        activePlayer.activeSpell = null;
                        BM_Funcs.clearSpellOptionList();
                        BM_Funcs.populateSpellOptionList();
                        battleStates = BattleStates.SELECT_OPTION;
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
                                if (selectedCommand == "Magic")
                                {                                    
                                    activePlayer.isCastingSpell = true;                                    
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
                                BM_Funcs.clearSpellOptionList();
                                OptionPanel.SetActive(false);
                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead
                        for (int i = 0; i < PlayerOptions.Count; i++)
                        {
                            if (result.gameObject == PlayerOptions[i])
                            {
                                for (int y = 0; y < PlayerOptions.Count; y++)
                                {
                                    PlayerOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                    selectedCommand = PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                }
                                PlayerOptions[i].GetComponent<Image>().color = Color.yellow;

                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);

                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another option to select that instead
                        for (int i = 0; i < SpellOptions.Count; i++)
                        {
                            if (result.gameObject == SpellOptions[i])
                            {
                                BM_Funcs.clearSpellOptionList();
                                BM_Funcs.populateSpellOptionList();

                                for (int y = 0; y < activePlayer.spellBook.Count; y++)
                                {
                                    if (result.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == activePlayer.spellBook[y].name)
                                    {
                                        activePlayer.activeSpell = activePlayer.spellBook[y];                                        
                                    }
                                }

                                SpellOptions[i].GetComponentInChildren<Image>().color = Color.magenta;                                
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
                    
                    for (int i = 0; i < PlayerOptions.Count; i++)
                    {
                        PlayerOptions[i].GetComponent<Image>().color = defaultBlueColor;
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
                                BM_Funcs.clearSpellOptionList();
                                OptionPanel.SetActive(false);
                                
                                for (int y = 0; y < RowChangeIcons.Count; y++)
                                {
                                    RowChangeIcons[y].SetActive(false);
                                }

                                battleStates = BattleStates.SELECT_ACTION;
                            }
                        }
                        //Left click another action to select that instead
                        for (int i = 0; i < PlayerOptions.Count; i++)
                        {
                            if (result.gameObject == PlayerOptions[i])
                            {
                                for (int y = 0; y < PlayerOptions.Count; y++)
                                {
                                    PlayerOptions[y].GetComponent<Image>().color = defaultBlueColor;
                                    selectedCommand = PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text;
                                }
                                PlayerOptions[i].GetComponent<Image>().color = Color.yellow;

                                activePlayer.activeSpell = null;
                                OptionPanel.SetActive(false);

                                for (int y = 0; y < RowChangeIcons.Count; y++)
                                {
                                    RowChangeIcons[y].SetActive(false);
                                }

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

                if (selectedCommand == "Attack")
                {
                    attackAnimCoroutineIsPaused = false;
                    StartCoroutine(BM_Enums.waitForAttackAnimation());
                    BM_Funcs.animationController(activePlayer, "IsAttacking");                    
                
                    if (attackAnimIsDone)
                    {
                        attackAnimCoroutineIsPaused = true;
                        
                        standIdle(activePlayer);

                        BM_Funcs.animationController(activePlayer);                        
                        activePlayer.speedTotal -= 100f;
                        activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                        BM_Funcs.resetChoicePanel();
                        BM_Funcs.clearSpellOptionList();
                        ActionPanel.SetActive(false);
                        OptionPanel.SetActive(false);
                        ActivePlayers.Remove(activePlayer);
                        activePlayer = null;
                        selectedCommand = null;

                        if (ActivePlayers.Count == 0)
                        {
                            returningStarting = true;

                            startRoutinesGoingAgain = true;
                            battleStates = BattleStates.DEFAULT;
                        }
                        else
                        {
                            battleStates = BattleStates.SELECT_PLAYER;
                        }
                    }
                }                                
                else if (activePlayer.isCastingSpell)
                {
                    BM_Funcs.animationController(activePlayer, "IsChanting");                    
                    activePlayer.constantAnimationState = "IsChanting";
                    activePlayer.hasConstantAnimationState = true;
                    activePlayer.playerCastBar.SetActive(true);
                    activePlayer.castSpeedTotal = activePlayer.activeSpell.castTime;
                    
                    standIdle(activePlayer);

                    //reset attack animation to idle
                    BM_Funcs.animationController(activePlayer);

                    activePlayer.speedTotal -= 100f;
                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    BM_Funcs.resetChoicePanel();
                    BM_Funcs.clearSpellOptionList();
                    ActionPanel.SetActive(false);
                    OptionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        startRoutinesGoingAgain = true;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }                    
                }
                else if (selectedCommand == "Wait")
                {
                    BM_Funcs.animationController(activePlayer);
                    standIdle(activePlayer);                    
                    activePlayer.speedTotal = (100f - activePlayer.speed);
                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    BM_Funcs.resetChoicePanel();
                    BM_Funcs.clearSpellOptionList();
                    ActionPanel.SetActive(false);
                    OptionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        startRoutinesGoingAgain = true;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }
                }
                else if (selectedCommand == "Change Row")
                {                    
                    standIdle(activePlayer);

                    BM_Funcs.animationController(activePlayer);
                    activePlayer.speedTotal -= 100f;
                    if (isSwitchingWithOtherPlayer)
                    {
                        playerToSwitchRowWith.speedTotal -= 100f;
                        isSwitchingWithOtherPlayer = false;
                    }
                    playerToSwitchRowWith = null;
                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    BM_Funcs.resetChoicePanel();
                    BM_Funcs.clearSpellOptionList();
                    ActionPanel.SetActive(false);
                    OptionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        startRoutinesGoingAgain = true;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }                    
                }

                break;

            case BattleStates.RESOLVE_SPELL:

                castAnimCoroutineIsPaused = false;
                StartCoroutine(BM_Enums.waitForCastAnimation());
                BM_Funcs.animationController(activePlayer, "IsCasting");

                if (castAnimIsDone)
                {
                    castAnimCoroutineIsPaused = true;
                    activePlayer.constantAnimationState = null;
                    activePlayer.hasConstantAnimationState = false;
                    BM_Funcs.animationController(activePlayer);
                    standIdle(activePlayer);
                    activePlayer.isCastingSpell = false;
                    activePlayer.playerCastBar.SetActive(false);
                    activePlayer.castSpeedTotal = 0f;
                    activePlayer.playerOptions.Remove("Cast");

                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    BM_Funcs.resetChoicePanel();
                    ActionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        startRoutinesGoingAgain = true;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }
                }                

                break;
            case BattleStates.SELECT_ENEMY:

                for (int i = 0; i < ActiveEnemies.Count; i++)
                {
                    activeEnemy = ActiveEnemies[i];
                }

                battleStates = BattleStates.SELECT_ENEMY_ACTION;
                break;
            case BattleStates.SELECT_ENEMY_ACTION:
                selectedCommand = "EnemyAttack";
                battleStates = BattleStates.SELECT_ENEMY_TARGET;
                break;
            case BattleStates.SELECT_ENEMY_TARGET:

                int selectedNumber = Random.Range(0, PlayersInBattle.Count);

                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    if (selectedNumber == i)
                    {
                        activeEnemy.enemyTarget = PlayersInBattle[i];                        
                    }
                }

                enemyReadyAnimCoroutineIsPaused = false;

                StartCoroutine(BM_Enums.waitForEnemyReadyAnimation());

                if (enemyReadyAnimIsDone == false)
                {
                    activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
                }                                

                if (enemyReadyAnimIsDone == true)
                {                    
                    enemyReadyAnimCoroutineIsPaused = true;                    
                    enemyReadyAnimIsDone = false;
                    activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsReady", false);                    
                    battleStates = BattleStates.ENEMY_ATTACK;                    
                }

                break;
            case BattleStates.ENEMY_ATTACK:
                
                activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);

                enemyAttackAnimCoroutineIsPaused = false;

                StartCoroutine(BM_Enums.waitForEnemyAttackAnimation());

                if (enemyAttackAnimIsDone == true)
                {
                    enemyAttackAnimCoroutineIsPaused = true;
                    battleStates = BattleStates.RESOLVE_ENEMY_TURN;                     
                }                

                break;
            case BattleStates.RESOLVE_ENEMY_TURN:

                if (selectedCommand == "EnemyAttack")
                {
                    activeEnemy.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", false);
                    enemyAttackAnimIsDone = false;
                    enemyReadyAnimIsDone = false;
                    enemyAttackAnimIsDone = false;
                    activeEnemy.speedTotal -= 100f;
                    activeEnemy.enemyPanel.GetComponent<Image>().color = defaultColor;
                    activeEnemy.enemyTarget = null;
                    ActiveEnemies.Clear();
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
                        //battleStates = BattleStates.SELECT_ENEMY;
                    }
                }                            

                break;
            default:
                break;
        }
    }  
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