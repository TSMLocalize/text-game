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
        Warrior.speed = 20f;
        Thief.speed = 20f;
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
