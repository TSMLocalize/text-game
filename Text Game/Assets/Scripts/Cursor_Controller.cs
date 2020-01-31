/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Cursor_Controller : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public GameObject instantiatedSelectPointer;
    public GameObject selectPointer = null;
    public Vector3 selectPointerDestination;
    public int newPos;
    public List<GameObject> activePlayerRowList;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
    }

    // Update is called once per frame
    void Update()
    {
        updateRows();

        if (BM.Rows.Count > 0 && instantiatedSelectPointer == null)
        {                        
            createSelectPointer(BM.Rows[0].transform.position);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < BM.Rows.Count; i++)
            {
                if (instantiatedSelectPointer.transform.position == new Vector3(BM.Rows[i].transform.position.x - 1, BM.Rows[i].transform.position.y))
                {
                    if (i < activePlayerRowList.Count)
                    {
                        if (activePlayerRowList.Contains(BM.Rows[i + 1]))
                        {
                            createSelectPointer(BM.Rows[i + 1].transform.position);
                            updateRows();
                            break;
                        }
                        else if (activePlayerRowList.Contains(BM.Rows[i + 2]))
                        {
                            createSelectPointer(BM.Rows[i + 2].transform.position);
                            updateRows();
                            break;
                        }
                        else if (activePlayerRowList.Contains(BM.Rows[i + 3]))
                        {
                            createSelectPointer(BM.Rows[i + 3].transform.position);
                            updateRows();
                            break;
                        }
                    }                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < BM.Rows.Count; i++)
            {
                if (instantiatedSelectPointer.transform.position == new Vector3(BM.Rows[i].transform.position.x - 1, BM.Rows[i].transform.position.y))
                {
                    if (i > 0)
                    {
                        if (activePlayerRowList.Contains(BM.Rows[i - 1]))
                        {
                            createSelectPointer(BM.Rows[i - 1].transform.position);
                            updateRows();
                            break;
                        }
                        else if (activePlayerRowList.Contains(BM.Rows[i - 2]))
                        {
                            createSelectPointer(BM.Rows[i - 2].transform.position);
                            updateRows();
                            break;
                        }
                        else if (activePlayerRowList.Contains(BM.Rows[i - 3]))
                        {
                            createSelectPointer(BM.Rows[i - 3].transform.position);
                            updateRows();
                            break;
                        }
                    }                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int i = 0; i < BM.Rows.Count; i++)
            {
                if (instantiatedSelectPointer.transform.position == new Vector3(BM.Rows[i].transform.position.x - 1, BM.Rows[i].transform.position.y))
                {
                    if (i < 3)
                    {
                        if (activePlayerRowList.Contains(BM.Rows[i + 4]))
                        {
                            createSelectPointer(BM.Rows[i + 4].transform.position);
                            updateRows();
                            break;
                        }
                    }                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int i = 0; i < BM.Rows.Count; i++)
            {
                if (instantiatedSelectPointer.transform.position == new Vector3(BM.Rows[i].transform.position.x - 1, BM.Rows[i].transform.position.y))
                {
                    if (i > 3)
                    {
                        if (activePlayerRowList.Contains(BM.Rows[i - 4]))
                        {
                            createSelectPointer(BM.Rows[i - 4].transform.position);
                            updateRows();
                            break;
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

    public void updateRows()
    {
        activePlayerRowList.Clear();

        if (BM.ActivePlayers.Count > 0 && activePlayerRowList.Count == 0)
        {
            for (int i = 0; i < BM.ActivePlayers.Count; i++)
            {
                for (int y = 0; y < BM.Rows.Count; y++)
                {
                    if (BM.ActivePlayers[i].battleSprite.transform.position == BM.Rows[y].transform.position)
                    {
                        activePlayerRowList.Add(BM.Rows[y]);
                    }
                }
            }
        }
    }
}

*/