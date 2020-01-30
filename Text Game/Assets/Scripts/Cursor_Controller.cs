using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cursor_Controller : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public GameObject instantiatedSelectPointer;
    public GameObject selectPointer = null;
    public Vector3 selectPointerDestination;
    public int newPos;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BM.ActivePlayers.Count > 0 && instantiatedSelectPointer == null)
        {
            createSelectPointer(BM.PlayersInBattle[0].battleSprite.transform.position);
        }

        if (instantiatedSelectPointer != null)
        {
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                for (int i = 0; i < BM.ActivePlayers.Count; i++)
                {
                    if (instantiatedSelectPointer.transform.position == new Vector3(BM.ActivePlayers[i].battleSprite.transform.position.x - 1f, BM.ActivePlayers[i].battleSprite.transform.position.y))
                    {
                        selectPointerDestination = BM.ActivePlayers[i + 1].battleSprite.transform.position;
                        createSelectPointer(selectPointerDestination);
                        break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                for (int i = 0; i < BM.ActivePlayers.Count; i++)
                {                    
                    if (instantiatedSelectPointer.transform.position == new Vector3(BM.ActivePlayers[i].battleSprite.transform.position.x - 1f, BM.ActivePlayers[i].battleSprite.transform.position.y))
                    {
                        selectPointerDestination = BM.ActivePlayers[i - 1].battleSprite.transform.position;
                        createSelectPointer(selectPointerDestination);
                        break;
                    }                    
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                for (int i = 0; i < BM.ActivePlayers.Count; i++)
                {
                    if (BM.ActivePlayers[i].currentRowPositionID > 4)
                    {
                        if (instantiatedSelectPointer.transform.position.y == BM.ActivePlayers[i].battleSprite.transform.position.y)
                        {
                            selectPointerDestination = BM.ActivePlayers[i].battleSprite.transform.position;
                            createSelectPointer(selectPointerDestination);
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                for (int i = 0; i < BM.ActivePlayers.Count; i++)
                {
                    if (BM.ActivePlayers[i].currentRowPositionID <= 4)
                    {
                        if (instantiatedSelectPointer.transform.position.y == BM.ActivePlayers[i].battleSprite.transform.position.y)
                        {
                            selectPointerDestination = BM.ActivePlayers[i].battleSprite.transform.position;
                            createSelectPointer(selectPointerDestination);
                        }
                    }
                }
            }
        }        
    }

    public void createSelectPointer(Vector3 position)
    {
        Destroy(instantiatedSelectPointer);
        instantiatedSelectPointer = Instantiate(selectPointer, position, Quaternion.identity);
        instantiatedSelectPointer.transform.position = new Vector3(instantiatedSelectPointer.transform.position.x - 1f, instantiatedSelectPointer.transform.position.y);
    }
}
