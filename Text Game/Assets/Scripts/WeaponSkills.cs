using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class WeaponSkills : MonoBehaviour
{
    public WeaponSkill Fast_Blade;
    public WeaponSkill Raging_Axe;
    public WeaponSkill Penta_Thrust;

    public Sprite FireElement;
    public Sprite IceElement;
    public Sprite WindElement;
    
    private void Start()
    {
        Fast_Blade.name = "Fast Blade";
        Fast_Blade.element = "Fire";
        Fast_Blade.TPCost = 100;
        Fast_Blade.weaponSkillElement = WindElement;

        Raging_Axe.name = "Raging Axe";
        Raging_Axe.element = "Ice";
        Raging_Axe.TPCost = 100;

        Penta_Thrust.name = "Penta Thrust";
        Penta_Thrust.element = "Wind";
        Penta_Thrust.TPCost = 100;        
    }
}