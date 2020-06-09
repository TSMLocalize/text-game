using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowChange_Manager : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_IEnumerators BM_Enums;
    public Action_Handler ActionHandler;
    public Animation_Handler AnimHandler;
    public Battle_Manager_Functions BM_Funcs;

    public bool rowSelected;
    public List<GameObject> RowChangeIcons;
    public GameObject RowToSwitch;
    public Player playerToSwitchRowWith;
    public bool isSwitchingWithOtherPlayer;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        ActionHandler = GetComponent<Action_Handler>();
        AnimHandler = GetComponent<Animation_Handler>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    // Update is called once per frame
    void Update()
    {
        //Populate Row Change Icons, minus the activeplayer's
        if (!rowSelected)
        {
            AnimHandler.animationController(BM.activePlayer, "IsReady");

            for (int i = 0; i < RowChangeIcons.Count; i++)
            {
                if (RowChangeIcons[i] != BM.activePlayer.currentRowPositionIcon)
                {
                    RowChangeIcons[i].SetActive(true);
                }
            }
        }

        //Right click to go back to select action
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            AnimHandler.animationController(BM.activePlayer);

            for (int i = 0; i < BM_Funcs.instantiatedOptions.Count; i++)
            {
                BM_Funcs.instantiatedOptions[i].GetComponent<Image>().color = BM.defaultBlueColor;
            }

            for (int y = 0; y < RowChangeIcons.Count; y++)
            {
                RowChangeIcons[y].SetActive(false);
            }

            BM.selectedCommand = null;

            BM.battleStates = Battle_Manager.BattleStates.SELECT_ACTION;
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
                    AnimHandler.animationController(PlayersInBattle[i], "IsCasting");

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
                    AnimHandler.animationController(playerToSwitchRowWith);
                    AnimHandler.animationController(activePlayer);
                    rowSelected = false;
                }
            }
            //Ensure one player is in position (if not switching places)
            else
            {
                if (activePlayer.battleSprite.transform.position == RowToSwitch.transform.position)
                {
                    AnimHandler.animationController(activePlayer);
                    rowSelected = false;
                }
            }

            //Finish up 
            if (rowSelected == false)
            {
                //All this crap is for getting the sprite to turn round properly when in the infiltrate row or the other rows
                if (RowToSwitch.GetComponent<Row>().ID > 8 && activePlayer.currentRowPosition.GetComponent<Row>().ID <= 8)
                {
                    activePlayer.battleSprite.transform.localScale =
                            new Vector2(-activePlayer.battleSprite.transform.localScale.x, activePlayer.battleSprite.transform.localScale.y);

                    activePlayer.target = new Vector3(activePlayer.battleSprite.transform.position.x + 1f, activePlayer.battleSprite.transform.position.y,
                    activePlayer.battleSprite.transform.position.z);

                    if (isSwitchingWithOtherPlayer)
                    {
                        playerToSwitchRowWith.battleSprite.transform.localScale =
                                new Vector2(-playerToSwitchRowWith.battleSprite.transform.localScale.x, playerToSwitchRowWith.battleSprite.transform.localScale.y);
                        playerToSwitchRowWith.target = new Vector3(playerToSwitchRowWith.battleSprite.transform.position.x + 1f, playerToSwitchRowWith.battleSprite.transform.position.y,
                        playerToSwitchRowWith.battleSprite.transform.position.z);
                    }
                }
                else if (RowToSwitch.GetComponent<Row>().ID <= 8 && activePlayer.currentRowPosition.GetComponent<Row>().ID > 8)
                {
                    activePlayer.battleSprite.transform.localScale =
                            new Vector2(-activePlayer.battleSprite.transform.localScale.x, activePlayer.battleSprite.transform.localScale.y);

                    activePlayer.target = new Vector3(activePlayer.battleSprite.transform.position.x - 1f, activePlayer.battleSprite.transform.position.y,
                    activePlayer.battleSprite.transform.position.z);

                    if (isSwitchingWithOtherPlayer)
                    {
                        playerToSwitchRowWith.battleSprite.transform.localScale =
                                new Vector2(-playerToSwitchRowWith.battleSprite.transform.localScale.x, playerToSwitchRowWith.battleSprite.transform.localScale.y);
                        playerToSwitchRowWith.target = new Vector3(playerToSwitchRowWith.battleSprite.transform.position.x + 1f, playerToSwitchRowWith.battleSprite.transform.position.y,
                        playerToSwitchRowWith.battleSprite.transform.position.z);
                    }
                }
                else if (RowToSwitch.GetComponent<Row>().ID <= 8 && activePlayer.currentRowPosition.GetComponent<Row>().ID <= 8)
                {
                    activePlayer.battleSprite.transform.localScale =
                    new Vector2(activePlayer.battleSprite.transform.localScale.x, activePlayer.battleSprite.transform.localScale.y);

                    activePlayer.target = new Vector3(activePlayer.battleSprite.transform.position.x - 1f, activePlayer.battleSprite.transform.position.y,
                    activePlayer.battleSprite.transform.position.z);

                    if (isSwitchingWithOtherPlayer)
                    {
                        playerToSwitchRowWith.battleSprite.transform.localScale =
                                new Vector2(playerToSwitchRowWith.battleSprite.transform.localScale.x, playerToSwitchRowWith.battleSprite.transform.localScale.y);
                        playerToSwitchRowWith.target = new Vector3(playerToSwitchRowWith.battleSprite.transform.position.x - 1f, playerToSwitchRowWith.battleSprite.transform.position.y,
                        playerToSwitchRowWith.battleSprite.transform.position.z);
                    }
                }
                else if (RowToSwitch.GetComponent<Row>().ID > 8 && activePlayer.currentRowPosition.GetComponent<Row>().ID > 8)
                {
                    activePlayer.battleSprite.transform.localScale =
                    new Vector2(activePlayer.battleSprite.transform.localScale.x, activePlayer.battleSprite.transform.localScale.y);

                    activePlayer.target = new Vector3(activePlayer.battleSprite.transform.position.x + 1f, activePlayer.battleSprite.transform.position.y,
                    activePlayer.battleSprite.transform.position.z);

                    if (isSwitchingWithOtherPlayer)
                    {
                        playerToSwitchRowWith.battleSprite.transform.localScale =
                                new Vector2(playerToSwitchRowWith.battleSprite.transform.localScale.x, playerToSwitchRowWith.battleSprite.transform.localScale.y);
                        playerToSwitchRowWith.target = new Vector3(playerToSwitchRowWith.battleSprite.transform.position.x + 1f, playerToSwitchRowWith.battleSprite.transform.position.y,
                        playerToSwitchRowWith.battleSprite.transform.position.z);
                    }
                }

                //reassign 'position' to the new position(s), reset new display layer order priority
                BM_Funcs.updateRowPositions();
                BM_Funcs.AssignRows();

                RowToSwitch = null;
                battleStates = BattleStates.RESOLVE_ACTION;
            }
        }
    }
}
