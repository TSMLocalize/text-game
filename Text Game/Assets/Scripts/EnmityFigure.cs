using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnmityFigure : MonoBehaviour
{
    public Battle_Manager BM;
    public Animation_Handler animHandler;
    public Action_Handler act_handler;
    public TextMeshPro EnmityPercentage;
    public float enemyPercentageNumber;
    public Sprite icon;
    public string type;
    public Player afflictedPlayer;
    public Enemy afflictedEnemy;
    public string playerorenemy;
    public string associatedAnimationState;


    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<Battle_Manager>();
        EnmityPercentage = GetComponentInChildren<TextMeshPro>();        
    }

    void Update()
    {
        
    }
}
