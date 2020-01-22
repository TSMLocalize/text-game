using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Enemy
{
    public string EnemyName;    
    public float speed;
    public float speedTotal;
    public float castSpeed;
    public float castSpeedTotal;
    public Player enemyTarget;

    //Spell & ability related
    public List<string> enemyOptions;
    public List<Spell> spellBook;
    public Spell activeSpell = null;
    public bool isCastingSpell;

    //Battle UI Player Elements
    public Sprite EnemyPortrait;
    public GameObject enemyPanel;
    public Image enemyPanelBackground;
    public GameObject enemySpeedBar;
    public TextMeshProUGUI enemySpeedBarText;
    public GameObject enemyCastBar;
    public GameObject enemyCastBarFill;
    public TextMeshProUGUI enemyCastBarText;    

    //Battle Sprite Elements
    public GameObject battleSprite;
    public Vector3 position;
    public Vector3 target;
    public bool hasConstantAnimationState;
    public string constantAnimationState = "";


    //Enemy Animation Coroutine Control Bools
    public bool enemyCastAnimCoroutineIsPaused;
    public bool enemyAttackAnimCoroutineIsPaused;
    public bool enemyReadyAnimCoroutineIsPaused;
    public bool enemyAttackAnimIsDone;
    public bool enemyCastAnimIsDone;
    public bool enemyReadyAnimIsDone;
}
