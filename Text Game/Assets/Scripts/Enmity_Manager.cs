using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enmity_Manager : MonoBehaviour
{    
    public Spells spells;
    public Enemy_Spells enemySpells;
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Combo_Manager combo_Manager;
    public Animation_Handler animHandler;
    public Battle_Manager_IEnumerators BM_Enums;            

    // Start is called before the first frame update
    void Start()
    {
        BM = GetComponent<Battle_Manager>();
        BM_Funcs = GetComponent<Battle_Manager_Functions>();
        BM_Enums = GetComponent<Battle_Manager_IEnumerators>();
        combo_Manager = GetComponent<Combo_Manager>();
        animHandler = GetComponent<Animation_Handler>();
        enemySpells = FindObjectOfType<Enemy_Spells>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
