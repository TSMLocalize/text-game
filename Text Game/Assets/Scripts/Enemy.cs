using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public Sprite EnemyPortrait;
    public float speed;
    public float speedTotal;
    public Player enemyTarget;

    //Battle UI Player Elements
    public GameObject enemyPanel;
    public Image enemyPanelBackground;
    public GameObject enemySpeedBar;
    public TextMeshProUGUI enemySpeedBarText;

    //Battle Sprite Elements
    public GameObject battleSprite;
    public Vector3 position;
    public Vector3 target;
}
