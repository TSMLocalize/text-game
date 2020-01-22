using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Battle_Manager_Data : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Battle_Manager_IEnumerators BM_Enums;


    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
