using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class WeaponSkill
{
    public string name;
    public string element;
    public float TPCost;
    public Sprite weaponSkillIcon;
    public Sprite weaponSkillElement;    
    
    //Spent Attacks are used to store the amount of attacks used, which are refreshed back to totalAttacks after the WS
    public int spentAttacks;
    public int totalAttacks;
    //The first timer is used to time the delay between each attack that will creating floating damage etc.
    //The second is how long the animation will take
    public float wsMultiAttackReportTimer;
    public float wsAnimTimer;

    public bool willCreateSkillchain;
}
