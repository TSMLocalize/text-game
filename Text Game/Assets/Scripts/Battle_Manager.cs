using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager : MonoBehaviour
{
    //Animation Attempt      
    public float speed;
    public bool stepForward;    
    public bool isCasting;
    public bool attackAnimCoroutineIsPaused;
    public bool attackAnimIsDone;
    public bool castAnimCoroutineIsPaused;
    public bool castAnimIsDone;
    public bool rowSelected;
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
    public Spells SpellManager;     
    public bool coroutineIsPaused = false;
    public bool returningStarting = true;
    public bool enemyTurnCoroutineIsPaused = false;
    public float enemyTurnPauseCounter;
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
        SELECT_ENEMY_ACTION,
        SELECT_ENEMY_TARGET,
        RESOLVE_ENEMY_ACTION
    }

    public BattleStates battleStates;        

    // Start is called before the first frame update
    void Start()
    {
        setupCharacters();
        
        ColorUtility.TryParseHtmlString("#010078", out defaultBlueColor);
        defaultColor = ActionPanel.GetComponent<Image>().color;

        //Start SpeedBar coroutines for players and enemies
        for (int i = 0; i < PlayersInBattle.Count; i++)
        {
            StartCoroutine(updatePlayerSpeedBars(PlayersInBattle[i]));
        }
        for (int i = 0; i < EnemiesInBattle.Count; i++)
        {
            StartCoroutine(updateEnemySpeedBars(EnemiesInBattle[i]));
        }

        StartCoroutine(delayWhileEnemyTurn());        

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
            animationController(activePlayer, "IsWalking");
            
            float step = speed * Time.deltaTime;
            activePlayer.battleSprite.transform.position = Vector3.MoveTowards(activePlayer.battleSprite.transform.position, activePlayer.target, step);

            if (activePlayer.battleSprite.transform.position == activePlayer.target)
            {
                animationController(activePlayer);
                if (activePlayer.isCastingSpell)
                {
                    animationController(activePlayer, "IsChanting");
                }
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

        updateEnemyUIBars();

        updatePlayerUIBars();

        switch (battleStates)
        {
            case BattleStates.DEFAULT:

                attackAnimIsDone = false;
                castAnimIsDone = false;

                //Check if a player is above 100 Speed, and pause the Coroutine
                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    if (PlayersInBattle[i].speedTotal >= 100 || (PlayersInBattle[i].isCastingSpell && PlayersInBattle[i].castSpeedTotal <= 0))
                    {
                        coroutineIsPaused = true;
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
                        coroutineIsPaused = true;
                    }
                }

                if (ActivePlayers.Count > 0)
                {
                    battleStates = BattleStates.SELECT_PLAYER;
                } else if (ActiveEnemies.Count > 0)
                {
                    for (int i = 0; i < ActiveEnemies.Count; i++)
                    {
                        activeEnemy = ActiveEnemies[i];
                    }

                    battleStates = BattleStates.SELECT_ENEMY_TARGET;
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
                populateActionList();

                attackAnimIsDone = false;
                castAnimIsDone = false;

                //Instant redirect if not waiting for a mouse click
                redirectAction();

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
                                    populateSpellOptionList();
                                }
                                
                                //Chooses the switch statement based on whether Attack, Magic etc. selected
                                redirectAction();
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
                    clearSpellOptionList();

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
                                resetChoicePanel();
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
                                animationController(activePlayer);
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer = ActivePlayers[i];
                                OptionPanel.SetActive(false);
                                selectedCommand = null;
                                clearSpellOptionList();
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
                                    clearSpellOptionList();
                                    battleStates = BattleStates.SELECT_ACTION;
                                }                                                 
                            }
                        }
                    }
                }                

                break;
            case BattleStates.SELECT_TARGET:

                animationController(activePlayer, "IsReady");

                //Color all potential target options yellow
                for (int i = 0; i < EnemiesInBattle.Count; i++)
                {
                    EnemiesInBattle[i].enemyPanelBackground.color = Color.yellow;
                }

                //Right click to go back to select action
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    animationController(activePlayer);

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
                        clearSpellOptionList();
                        populateSpellOptionList();
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
                                resetChoicePanel();
                                activePlayer.playerPanel.GetComponent<Image>().color = Color.yellow;
                                animationController(activePlayer);
                                activePlayer.battleSprite.transform.position = activePlayer.position;
                                activePlayer = ActivePlayers[i];
                                stepForward = true;
                                selectedCommand = null;                                
                                activePlayer.activeSpell = null;
                                clearSpellOptionList();
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
                                clearSpellOptionList();
                                populateSpellOptionList();

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
                    animationController(activePlayer, "IsReady");

                    for (int i = 0; i < RowChangeIcons.Count; i++)
                    {
                        if (RowChangeIcons[i] != activePlayer.currentRowPositionIcon)
                        {
                            RowChangeIcons[i].SetActive(true);
                        }
                    }
                }                

                //If clicking a row icon, set that row icon as the target and start a hands up animation
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    animationController(activePlayer);

                    foreach (RaycastResult result in results)
                    {
                        for (int i = 0; i < RowChangeIcons.Count; i++)
                        {
                            if (result.gameObject == RowChangeIcons[i])
                            {
                                RowToSwitch = Rows[i];
                                animationController(activePlayer, "IsCasting");
                                rowSelected = true;
                            }
                        }                        
                    }            
                }

                //Once a row icon is clicked, check if there's a player there and switch with them
                if (rowSelected)
                {                    
                    speed = 8.0f;

                    float step = speed * Time.deltaTime;
                    activePlayer.battleSprite.transform.position = Vector3.MoveTowards(activePlayer.battleSprite.transform.position, RowToSwitch.transform.position, step);

                    for (int i = 0; i < PlayersInBattle.Count; i++)
                    {
                        if (PlayersInBattle[i].currentRowPosition == RowToSwitch)
                        {
                            playerToSwitchRowWith = PlayersInBattle[i];

                            //Set hands up animation
                            animationController(PlayersInBattle[i], "IsCasting");                            

                            PlayersInBattle[i].battleSprite.transform.position = Vector3.MoveTowards(PlayersInBattle[i].battleSprite.transform.position, 
                                activePlayer.position, step);
                        }                        
                    }

                    //Once switch is completed finish up
                    if (activePlayer.battleSprite.transform.position == RowToSwitch.transform.position)
                    {
                        if (playerToSwitchRowWith.name != "")
                        {
                            animationController(playerToSwitchRowWith);                            
                        }
                        
                        animationController(activePlayer);                                                
                        rowSelected = false;
                        battleStates = BattleStates.RESOLVE_ACTION;
                    }                    
                }


                break;
            case BattleStates.RESOLVE_ACTION:

                if (selectedCommand == "Attack")
                {
                    attackAnimCoroutineIsPaused = false;
                    StartCoroutine(waitForAttackAnimation());
                    animationController(activePlayer, "IsAttacking");                    
                
                    if (attackAnimIsDone)
                    {
                        attackAnimCoroutineIsPaused = true;
                        
                        standIdle(activePlayer);

                        animationController(activePlayer);                        
                        activePlayer.speedTotal -= 100f;
                        activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                        resetChoicePanel();
                        clearSpellOptionList();
                        ActionPanel.SetActive(false);
                        OptionPanel.SetActive(false);
                        ActivePlayers.Remove(activePlayer);
                        activePlayer = null;
                        selectedCommand = null;

                        if (ActivePlayers.Count == 0)
                        {
                            returningStarting = true;

                            for (int i = 0; i < PlayersInBattle.Count; i++)
                            {
                                StartCoroutine(updatePlayerSpeedBars(PlayersInBattle[i]));
                            }

                            coroutineIsPaused = false;
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
                    animationController(activePlayer, "IsChanting");                    
                    activePlayer.constantAnimationState = "IsChanting";
                    activePlayer.playerCastBar.SetActive(true);
                    activePlayer.castSpeedTotal = activePlayer.activeSpell.castTime;
                    
                    standIdle(activePlayer);
                    
                    //reset attack animation to idle
                    animationController(activePlayer);

                    activePlayer.speedTotal -= 100f;
                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    resetChoicePanel();
                    clearSpellOptionList();
                    ActionPanel.SetActive(false);
                    OptionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        for (int i = 0; i < PlayersInBattle.Count; i++)
                        {
                            StartCoroutine(updatePlayerSpeedBars(PlayersInBattle[i]));
                        }

                        coroutineIsPaused = false;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }                    
                }
                else if (selectedCommand == "Wait")
                {
                    animationController(activePlayer);
                    standIdle(activePlayer);                    
                    activePlayer.speedTotal = (100f - activePlayer.speed);
                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    resetChoicePanel();
                    clearSpellOptionList();
                    ActionPanel.SetActive(false);
                    OptionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        for (int i = 0; i < PlayersInBattle.Count; i++)
                        {
                            StartCoroutine(updatePlayerSpeedBars(PlayersInBattle[i]));
                        }

                        coroutineIsPaused = false;
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
                StartCoroutine(waitForCastAnimation());
                animationController(activePlayer, "IsCasting");

                if (castAnimIsDone)
                {
                    castAnimCoroutineIsPaused = true;
                    activePlayer.constantAnimationState = null;
                    animationController(activePlayer);
                    standIdle(activePlayer);
                    activePlayer.isCastingSpell = false;
                    activePlayer.playerCastBar.SetActive(false);
                    activePlayer.castSpeedTotal = 0f;
                    activePlayer.playerOptions.Remove("Cast");

                    activePlayer.playerPanel.GetComponent<Image>().color = defaultColor;
                    resetChoicePanel();
                    ActionPanel.SetActive(false);
                    ActivePlayers.Remove(activePlayer);
                    activePlayer = null;
                    selectedCommand = null;

                    if (ActivePlayers.Count == 0)
                    {
                        returningStarting = true;

                        for (int i = 0; i < PlayersInBattle.Count; i++)
                        {
                            StartCoroutine(updatePlayerSpeedBars(PlayersInBattle[i]));
                        }

                        coroutineIsPaused = false;
                        battleStates = BattleStates.DEFAULT;
                    }
                    else
                    {
                        battleStates = BattleStates.SELECT_PLAYER;
                    }
                }                

                break;
            case BattleStates.SELECT_ENEMY_ACTION:

                break;
            case BattleStates.SELECT_ENEMY_TARGET:

                int selectedNumber = Random.Range(0, PlayersInBattle.Count);

                for (int i = 0; i < PlayersInBattle.Count; i++)
                {
                    if (selectedNumber == i)
                    {
                        activeEnemy.enemyTarget = PlayersInBattle[i];
                        battleStates = BattleStates.RESOLVE_ENEMY_ACTION;
                    }                    
                }
                
                break;
            case BattleStates.RESOLVE_ENEMY_ACTION:
                break;
            default:
                break;
        }
    }
    
    // IENUMERATORS   

    IEnumerator waitForAttackAnimation()
    {
        while (attackAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (attackAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            attackAnimIsDone = true;            
        }
    }

    IEnumerator waitForCastAnimation()
    {
        while (castAnimCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (castAnimCoroutineIsPaused == false)
        {
            yield return new WaitForSeconds(1f);
            castAnimIsDone = true;
        }
    }

    IEnumerator updatePlayerSpeedBars(Player player)
    {
        while (coroutineIsPaused == true)
        {
            yield return null;            
        }

        while (coroutineIsPaused == false)
        {
            if (returningStarting == true)
            {
                yield return new WaitForSeconds(0.3f);
                returningStarting = false;
            }
            //Forces scale to max when speed is 100+
            if (player.speedTotal >= 100 || (player.isCastingSpell && player.castSpeedTotal <= 0))
            {
                
                
                if (player.isCastingSpell)
                {
                    player.playerCastBar.GetComponent<Image>().transform.localScale = new Vector3(0f, player.playerCastBar.GetComponent<Image>().transform.localScale.y);
                } else
                {
                    player.playerSpeedBar.GetComponent<Image>().transform.localScale = new Vector3(1f, player.playerSpeedBar.GetComponent<Image>().transform.localScale.y);
                }
            }            
            else
            {
                
                if (player.isCastingSpell)
                {
                    player.castSpeedTotal -= player.castSpeed;
                } else
                {
                    player.speedTotal += player.speed;
                }
                
                yield return new WaitForSeconds(0.5f);
            }            
        }        
    }

    IEnumerator updateEnemySpeedBars(Enemy enemy)
    {
        while (coroutineIsPaused == true)
        {
            yield return null;
        }

        while (coroutineIsPaused == false)
        {
            if (returningStarting == true)
            {
                yield return new WaitForSeconds(0.3f);
                returningStarting = false;
            }
            //Forces scale to max when speed is 100+
            if (enemy.speedTotal >= 100)
            {
                enemy.enemySpeedBar.GetComponent<Image>().transform.localScale = new Vector3(1f, enemy.enemySpeedBar.GetComponent<Image>().transform.localScale.y);
            }
            else
            {
                enemy.speedTotal += enemy.speed;
                yield return new WaitForSeconds(0.5f);
            }
        }        
    }

    IEnumerator delayWhileEnemyTurn()
    {
        while (enemyTurnCoroutineIsPaused == true)
        {
            yield return null;
        }

        while (enemyTurnCoroutineIsPaused == false)
        {
            if (enemyTurnPauseCounter <= 10f)
            {
                enemyTurnPauseCounter += 1f;
                yield return new WaitForSeconds(0.5f);
            }

            if (enemyTurnPauseCounter == 10f)
            {
                enemyTurnPauseCounter = 0;
                enemyTurnCoroutineIsPaused = true;
            }
        }
    }
    // SET UP

    public void setupCharacters()
    {        
        //Add Players from PlayerManager to PlayersInBattle
        for (int i = 0; i < PartyManager.GetComponent<Party_Manager>().partyMembers.Count; i++)
        {
            PlayersInBattle.Add(PartyManager.GetComponent<Party_Manager>().partyMembers[i]);
        }

        //Add Enemies from EnemyManager to EnemiesInBattle
        for (int i = 0; i < EnemyManager.GetComponent<Enemy_Manager>().enemiesInBattle.Count; i++)
        {
            EnemiesInBattle.Add(EnemyManager.GetComponent<Enemy_Manager>().enemiesInBattle[i]);
        }

        for (int i = 0; i < PlayersInBattle.Count; i++)
        {
            //Assign Panels and populate
            PlayersInBattle[i].playerPanel = PlayerPanels[i];
            PlayersInBattle[i].playerPanelBackground = PlayerPanels[i].GetComponent<Image>();
            playerPanelArray = PlayerPanels[i].GetComponentsInChildren<Image>();
            playerPanelArray[3].overrideSprite = PlayersInBattle[i].PlayerPortrait;
            //Speed Bar setup
            PlayersInBattle[i].playerSpeedBarText = PlayerSpeedBarTexts[i];
            PlayersInBattle[i].playerSpeedBar = PlayerSpeedBars[i];
            //Cast Bar setup
            PlayersInBattle[i].playerCastBar = PlayerCastBars[i];
            PlayersInBattle[i].playerCastBarText = PlayerCastBarTexts[i];
            PlayersInBattle[i].playerCastBarFill = PlayerCastBarFills[i];            
            PlayerCastBars[i].SetActive(false);
            //Set battle sprites to their correct row
            AssignRows();
            //Transforms for moving
            PlayersInBattle[i].target = new Vector3(PlayersInBattle[i].battleSprite.transform.position.x - 1f, PlayersInBattle[i].battleSprite.transform.position.y,
                PlayersInBattle[i].battleSprite.transform.position.z);            
            PlayersInBattle[i].position = PlayersInBattle[i].battleSprite.transform.position;            
        }

        for (int i = 0; i < EnemiesInBattle.Count; i++)
        {
            enemyPanelArray = EnemyPanels[i].GetComponentsInChildren<Image>();
            enemyPanelArray[1].overrideSprite = EnemiesInBattle[i].EnemyPortrait;
            EnemiesInBattle[i].enemySpeedBarText = EnemySpeedBarTexts[i];
            EnemiesInBattle[i].enemySpeedBar = EnemySpeedBars[i];
            EnemiesInBattle[i].enemyPanel = EnemyPanels[i];
            EnemiesInBattle[i].enemyPanelBackground = EnemyPanels[i].GetComponent<Image>();
        }                        
    }   

    void AssignRows()
    {
        //Assign battle sprites to rows
        for (int i = 0; i < PlayersInBattle.Count; i++)
        {
            for (int y = 0; y < Rows.Count; y++)
            {
                if (Rows[y].gameObject.name == PlayersInBattle[i].currentRowPositionID)
                {
                    //Setup new movement position for the sprite
                    PlayersInBattle[i].battleSprite.transform.position = Rows[y].gameObject.transform.position;
                    PlayersInBattle[i].position = Rows[y].gameObject.transform.position;
                    //Assign the player with a physical row position
                    PlayersInBattle[i].currentRowPosition = Rows[y].gameObject;
                    PlayersInBattle[i].currentRowPositionIcon = RowChangeIcons[y];

                    if (PlayersInBattle[i].currentRowPositionID == "Front Row 1" || PlayersInBattle[i].currentRowPositionID ==  "Front Row 1")
                    {
                        PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    } else if (PlayersInBattle[i].currentRowPositionID == "Front Row 2" || PlayersInBattle[i].currentRowPositionID == "Back Row 2")
                    {
                        PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    } else if (PlayersInBattle[i].currentRowPositionID == "Front Row 3" || PlayersInBattle[i].currentRowPositionID == "Back Row 3")
                    {
                        PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    } else if (PlayersInBattle[i].currentRowPositionID == "Front Row 4" || PlayersInBattle[i].currentRowPositionID == "Back Row 4")
                    {
                        PlayersInBattle[i].battleSprite.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    }
                }
            }
        }        
    }

    //Refactoring Functions

    void resetChoicePanel()
    {
        for (int i = 0; i < PlayerOptions.Count; i++)
        {
            PlayerOptions[i].GetComponent<Image>().color = defaultBlueColor;
            PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            PlayerOptions[i].SetActive(false);
        }
    }

    void clearSpellOptionList()
    {
        for (int i = 0; i < SpellOptions.Count; i++)
        {
            SpellOptions[i].GetComponentInChildren<Image>().color = defaultBlueColor;
            SpellOptions[i].SetActive(false);
        }
    }

    void populateSpellOptionList()
    {
        for (int i = 0; i < activePlayer.spellBook.Count; i++)
        {            
            optionsArray = SpellOptions[i].GetComponentsInChildren<Image>();
            SpellOptions[i].SetActive(true);
            SpellOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = activePlayer.spellBook[i].name;
            optionsArray[1].overrideSprite = activePlayer.spellBook[i].spellIcon;
            SpellOptions[i].GetComponentInChildren<Image>().color = defaultBlueColor;
        }
    }

    void populateActionList()
    {
        for (int i = 0; i < PlayerOptions.Count; i++)
        {
            PlayerOptions[i].SetActive(false);
        }

        if (activePlayer.isCastingSpell == true)
        {
            activePlayer.playerOptions.Clear();
            
            activePlayer.playerOptions.Add("Cast");

            for (int i = 0; i < activePlayer.playerOptions.Count; i++)
            {
                PlayerOptions[i].SetActive(true);
                PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = activePlayer.playerOptions[i];
            }            
        }
        else if (activePlayer.isCastingSpell == false)
        {
            activePlayer.playerOptions.Clear();

            activePlayer.playerOptions.Add("Attack");
            activePlayer.playerOptions.Add("Magic");
            activePlayer.playerOptions.Add("Change Row");
            activePlayer.playerOptions.Add("Wait");

            for (int i = 0; i < activePlayer.playerOptions.Count; i++)
            {
                PlayerOptions[i].SetActive(true);
                PlayerOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = activePlayer.playerOptions[i];
            }                        
        }       
    }

    void updatePlayerUIBars()
    {
        //Update Player Speed bar data every frame
        for (int i = 0; i < PlayersInBattle.Count; i++)
        {
            PlayersInBattle[i].playerSpeedBarText.text = PlayersInBattle[i].speedTotal + "/100 " + "(" + PlayersInBattle[i].speed + ")";
            PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((PlayersInBattle[i].speedTotal / 100), 0, 1),
            PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale.y,
            PlayersInBattle[i].playerSpeedBar.GetComponent<Image>().transform.localScale.z);

            //Update and show cast bars while isCastingSpell is true for a player
            if (PlayersInBattle[i].isCastingSpell == true)
            {
                PlayersInBattle[i].playerCastBar.SetActive(true);
                PlayersInBattle[i].playerCastBarText.text = PlayersInBattle[i].castSpeedTotal + " (" + PlayersInBattle[i].castSpeed + ")";
                PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((PlayersInBattle[i].castSpeedTotal / 100), 0, 1),
                PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.y,
                PlayersInBattle[i].playerCastBarFill.GetComponent<Image>().transform.localScale.z);
            }
            else
            {
                PlayersInBattle[i].playerCastBar.SetActive(false);
            }

        }
    }

    void updateEnemyUIBars()
    {
        //Update Enemy Speed bar data every frame
        for (int i = 0; i < EnemiesInBattle.Count; i++)
        {
            EnemiesInBattle[i].enemySpeedBarText.text = EnemiesInBattle[i].speedTotal + "/100 " + "(" + EnemiesInBattle[i].speed + ")";
            EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale = new Vector3(Mathf.Clamp((EnemiesInBattle[i].speedTotal / 100), 0, 1),
            EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale.y,
            EnemiesInBattle[i].enemySpeedBar.GetComponent<Image>().transform.localScale.z);
        }
    }

    public void redirectAction()
    {
        if (selectedCommand != "")
        {
            if (selectedCommand == "Attack")
            {                
                battleStates = BattleStates.SELECT_TARGET;
            }
            else if (selectedCommand == "Magic")
            {
                populateSpellOptionList();
                battleStates = BattleStates.SELECT_OPTION;
            }
            else if (selectedCommand == "Wait")
            {
                battleStates = BattleStates.RESOLVE_ACTION;
            }
            else if (selectedCommand == "Cast")
            {
                battleStates = BattleStates.RESOLVE_SPELL;
            }
            else if (selectedCommand == "Change Row")
            {
                battleStates = BattleStates.CHANGE_ROW;
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

        if (state == "IsAttacking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsAttacking", true);
        }
        if (state == "IsCasting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsCasting", true);
        }
        if (state == "IsReady")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsReady", true);
        }
        if (state == "IsChanting")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsChanting", true);
        }
        if (state == "IsWalking")
        {
            player.battleSprite.GetComponent<Animator>().SetBool("IsWalking", true);
        }     
        if (player.constantAnimationState != "")
        {
            player.battleSprite.GetComponent<Animator>().SetBool(player.constantAnimationState, true);
        }
    }
}