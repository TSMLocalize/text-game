﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnmityFigure : MonoBehaviour
{
    public Battle_Manager BM;    
    public TextMeshPro EnmityPercentage;
    public float enemyPercentageNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<Battle_Manager>();
        EnmityPercentage = GetComponentInChildren<TextMeshPro>();        
    }
}
