using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusAilment : MonoBehaviour
{
    public Battle_Manager BM;
    public Battle_Manager_IEnumerators BM_Enums;
    public TextMeshPro StatusTimer;
    public float statusTimerNumber;    

    // Start is called before the first frame update
    void Start()
    {
        statusTimerNumber = 12;
        BM = FindObjectOfType<Battle_Manager>();
        StatusTimer = GetComponentInChildren<TextMeshPro>();        
    }

    void Update()
    {
        StatusTimer.text = statusTimerNumber.ToString();

        if (this.statusTimerNumber == 0)
        {
            Destroy(this.gameObject);
        }
        
    }   
}
