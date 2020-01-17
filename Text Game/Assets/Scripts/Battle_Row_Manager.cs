using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Row_Manager : MonoBehaviour
{

    public List<GameObject> rowPositions;    
    public Party_Manager partyManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initiateRowPositions()
    {
        for (int i = 0; i < partyManager.partyMembers.Count; i++)
        {
            for (int y = 0; y < rowPositions.Count; y++)
            {
                if (rowPositions[y].gameObject.name == partyManager.partyMembers[i].playerRowPosition)
                {
                    partyManager.partyMembers[i].battleSprite.transform.position = rowPositions[y].gameObject.transform.position;
                    partyManager.partyMembers[i].position = rowPositions[y].gameObject.transform.position;
                }
            }
        }
    }
}
