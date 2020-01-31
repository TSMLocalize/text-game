using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cursor_Controller_UI : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public GameObject instantiatedSelectUIPointer;
    public GameObject selectUIPointer;
    public Vector3 selectUIPointerDestination;
    public List<GameObject> cursorPositions;

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        cursorPositions[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {                

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            for (int i = 0; i < cursorPositions.Count; i++)
            {
                if (cursorPositions[i].activeSelf)
                {                    
                    cursorPositions[i].SetActive(false);
                    cursorPositions[i + 1].SetActive(true);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < cursorPositions.Count; i++)
            {
                if (cursorPositions[i].activeSelf)
                {
                    cursorPositions[i].SetActive(false);
                    cursorPositions[i - 1].SetActive(true);
                    break;
                }
            }
        }

    }
}