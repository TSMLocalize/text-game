using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy_Manager : MonoBehaviour
{
    public List<Enemy> enemiesInBattle;
    public GameObject SpellManager;
    public Enemy Warrior;
    public Enemy Thief;
    public Enemy Wizard;
    public Enemy Priest;
    public Enemy Paladin;
    public Enemy Ranger;
    public Enemy Bard;
    public Enemy Machinist;

    // Start is called before the first frame update
    void Start()
    {        
        Warrior.speed = 2f;
        Thief.speed = 2f;
        Wizard.speed = 1f;
        Priest.speed = 1f;
        Paladin.speed = 1f;
        Ranger.speed = 1f;
        Bard.speed = 1f;
        Machinist.speed = 1f;

        Warrior.castSpeed = 10f;
        Thief.castSpeed = 10f;
        Wizard.castSpeed = 1f;
        Priest.castSpeed = 2f;
        Paladin.castSpeed = 2f;
        Ranger.castSpeed = 2f;
        Bard.castSpeed = 2f;
        Machinist.castSpeed = 2f;

        Warrior.maxHP = 10f;
        Thief.maxHP = 10f;
        Wizard.maxHP = 10f;
        Priest.maxHP = 10f;
        Paladin.maxHP = 10f;
        Ranger.maxHP = 10f;
        Bard.maxHP = 10f;
        Machinist.maxHP = 10f;

        Warrior.currentHP = 10f;
        Thief.currentHP = 10f;
        Wizard.currentHP = 10f;
        Priest.currentHP = 10f;
        Paladin.currentHP = 10f;
        Ranger.currentHP = 10f;
        Bard.currentHP = 10f;
        Machinist.currentHP = 10f;

        Warrior.Accuracy = 5f;
        Thief.Accuracy = 10f;
        Wizard.Accuracy = 15f;
        Priest.Accuracy = 20f;
        Paladin.Accuracy = 25f;
        Ranger.Accuracy = 30f;
        Bard.Accuracy = 10f;
        Machinist.Accuracy = 15f;

        Warrior.Evasion = 55f;
        Thief.Evasion = 50f;
        Wizard.Evasion = 65f;
        Priest.Evasion = 70f;
        Paladin.Evasion = 75f;
        Ranger.Evasion = 80f;
        Bard.Evasion = 90f;
        Machinist.Evasion = 100f;

        enemiesInBattle.Add(Warrior);
        enemiesInBattle.Add(Thief);
        enemiesInBattle.Add(Wizard);
        enemiesInBattle.Add(Priest);
        enemiesInBattle.Add(Paladin);
        enemiesInBattle.Add(Ranger);
        enemiesInBattle.Add(Bard);
        enemiesInBattle.Add(Machinist);        
    }
}
