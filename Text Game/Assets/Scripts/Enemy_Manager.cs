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
