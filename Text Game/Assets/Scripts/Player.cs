﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Player
{
    //Basic variables
    public string name;    
    public float speed;
    public float speedTotal;
    public float castSpeed;
    public float castSpeedTotal;

    //Spell & ability related
    public List<string> playerOptions;
    public List<Spell> spellBook;
    public Spell activeSpell = null;
    public bool isCastingSpell;

    //In-battle position
    public string playerRowPosition;

    //Battle UI Player Elements    
    public Sprite PlayerPortrait;
    public GameObject playerPanel;
    public Image playerPanelBackground;
    public GameObject playerSpeedBar;
    public TextMeshProUGUI playerSpeedBarText;
    public GameObject playerCastBar;
    public GameObject playerCastBarFill;
    public TextMeshProUGUI playerCastBarText;

    //In battle sprite
    public GameObject battleSprite;
    public Vector3 position;
    public Vector3 target;    

}
