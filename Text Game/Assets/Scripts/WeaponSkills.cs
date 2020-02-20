using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class WeaponSkills : MonoBehaviour
{
    //Weaponskills
    public WeaponSkill Fast_Blade;
    public WeaponSkill Raging_Axe;
    public WeaponSkill Penta_Thrust;

    //SkillChains
    public WeaponSkill Scission;
    public WeaponSkill Fusion;
    public WeaponSkill Light;

    public Sprite FireElement;
    public Sprite IceElement;
    public Sprite WindElement;
    
    private void Start()
    {
        Scission.name = "Scission";
        Fusion.name = "Fusion";
        Light.name = "Light";

        Fast_Blade.name = "Fast Blade";
        Fast_Blade.element = "Fire";
        Fast_Blade.TPCost = 100;
        Fast_Blade.animationTimer = 1.8f;
        Fast_Blade.weaponSkillElement = FireElement;

        Raging_Axe.name = "Raging Axe";
        Raging_Axe.element = "Ice";
        Raging_Axe.TPCost = 100;
        Raging_Axe.animationTimer = 1.8f;
        Raging_Axe.weaponSkillElement = IceElement;

        Penta_Thrust.name = "Penta Thrust";
        Penta_Thrust.element = "Wind";
        Penta_Thrust.TPCost = 100;
        Penta_Thrust.animationTimer = 1.8f;
        Penta_Thrust.weaponSkillElement = WindElement;
    }
}