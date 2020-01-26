using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Player
{
    //Basic variables
    public string name;
    public float currentHP;
    public float maxHP;
    public float speed;
    public float speedTotal;
    public float castSpeed;
    public float castSpeedTotal;
    public string PlayerTargetID;

    //Battle variables
    public float Accuracy;
    public float Evasion;
    public float Attack;
    public float Armor;

    //Spell & ability related
    public List<string> playerOptions;
    public List<Spell> spellBook;
    public Spell activeSpell = null;
    public bool isCastingSpell;

    //Battle UI Player Elements    
    public Sprite PlayerPortrait;
    public GameObject playerPanel;
    public TextMeshProUGUI playerPanelText;
    public Image playerPanelBackground;
    public GameObject playerSpeedBar;
    public TextMeshProUGUI playerSpeedBarText;
    public GameObject playerCastBar;
    public GameObject playerCastBarFill;
    public TextMeshProUGUI playerCastBarText;

    //Battle sprite related
    public GameObject battleSprite;
    public Vector3 position;
    public Vector3 target;    
    public int currentRowPositionID;
    public GameObject currentRowPosition;
    public GameObject currentRowPositionIcon;
    public bool hasConstantAnimationState;
    public string constantAnimationState = "";
}
