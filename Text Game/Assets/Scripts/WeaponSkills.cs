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

    //SkillChains 1
    public WeaponSkill Scission;
    public WeaponSkill Reverberation;
    public WeaponSkill Detonation;
    public WeaponSkill Liquefaction;
    public WeaponSkill Induration;
    public WeaponSkill Impaction;
    public WeaponSkill Transfixion;
    public WeaponSkill Compression;
    //SkillChains 2
    public WeaponSkill Fusion; //Fire & Light
    public WeaponSkill Distortion; //Water & Ice
    public WeaponSkill Fragmentation; //Wind & Thunder
    public WeaponSkill Gravitation; //Earth & Dark    
    //SkillChains 3
    public WeaponSkill Light;
    public WeaponSkill Dark;

    public Sprite FireElement;
    public Sprite IceElement;
    public Sprite WindElement;
    
    private void Start()
    {
        //Lv. 1 SCs
        Scission.name = "Scission";        
        Reverberation.name = "Reverberation";
        Detonation.name = "Detonation";
        Liquefaction.name = "Liquefaction";
        Induration.name = "Induration";
        Impaction.name = "Impaction";
        Transfixion.name = "Transfixion";
        Compression.name = "Compression";
        //Lv. 2 SCs
        Fusion.name = "Fusion";
        Distortion.name = "Distortion";
        Fragmentation.name = "Fragmentation";
        Gravitation.name = "Gravitation";
        //Lv. 3 SCs
        Light.name = "Light";
        Dark.name = "Dark";

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