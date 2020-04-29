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
    public float storeTP;
    public float tpTotal;
    public float castSpeed;
    public float castSpeedTotal;
    public string PlayerTargetID;

    //Battle variables
    public float Accuracy;
    public float Evasion;
    public float Attack;
    public float Armor;

    //Weaponskill related
    public List<WeaponSkill> weaponSkills;
    public WeaponSkill selectedWeaponSkill;

    //Spell & ability related
    public List<string> playerOptions;
    public List<Spell> spellBook;
    public Spell activeSpell = null;
    public bool isReady;
    public bool isCastingSpell;
    public bool isCritical;    

    //Battle UI Player Elements    
    public Sprite PlayerPortrait;
    public GameObject playerPanel;
    public TextMeshProUGUI playerPanelText;
    public Image playerPanelBackground;
    public GameObject playerSpeedBar;
    public TextMeshProUGUI playerSpeedBarText;
    public GameObject playerTPBar;
    public GameObject playerTPBarFill;
    public TextMeshProUGUI playerTPBarText;
    public GameObject playerCastBar;
    public GameObject playerCastBarFill;
    public TextMeshProUGUI playerCastBarText;

    public GameObject playerTimersEntry;

    //Battle sprite related
    public GameObject battleSprite;
    public Vector3 position;
    public Vector3 target;    
    public int currentRowPositionID;
    public GameObject currentRowPosition;
    public GameObject currentRowPositionIcon;    
}
