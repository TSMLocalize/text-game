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
    public WeaponSkill Earth_Crusher;
    public WeaponSkill Blade_Tekko;
    public WeaponSkill Raiden_Thrust;
    public WeaponSkill Guillotine;
    public WeaponSkill Seraph_Blade;

    //returned when failing a SC
    public WeaponSkill FAIL;
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
    public Sprite WaterElement;
    public Sprite EarthElement;
    public Sprite ThunderElement;

    private void Start()
    {
        //FAIL
        FAIL.name = "fail";
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


        //!! The wsAnimTimer must be higher or the same as the SkillChain timer (currently 1.5f) NO IDEA WHY
        Fast_Blade.name = "Fast Blade";
        Fast_Blade.element = "Fire";
        Fast_Blade.TPCost = 100;        
        Fast_Blade.weaponSkillElement = FireElement;
        Fast_Blade.spentAttacks = 2;
        Fast_Blade.totalAttacks = 2;
        Fast_Blade.wsAnimTimer = 1.8f;
        Fast_Blade.wsMultiAttackReportTimer = 0.9f;

        Raging_Axe.name = "Raging Axe";
        Raging_Axe.element = "Ice";
        Raging_Axe.TPCost = 100;        
        Raging_Axe.weaponSkillElement = IceElement;
        Raging_Axe.spentAttacks = 1;
        Raging_Axe.totalAttacks = 1;
        Raging_Axe.wsAnimTimer = 1.5f;
        Raging_Axe.wsMultiAttackReportTimer = 1.5f;

        Raiden_Thrust.name = "Raiden Thrust";
        Raiden_Thrust.element = "Thunder";
        Raiden_Thrust.TPCost = 100;
        Raiden_Thrust.weaponSkillElement = ThunderElement;
        Raiden_Thrust.spentAttacks = 2;
        Raiden_Thrust.totalAttacks = 2;
        Raiden_Thrust.wsAnimTimer = 1.8f;
        Raiden_Thrust.wsMultiAttackReportTimer = 0.9f;

        Penta_Thrust.name = "Penta Thrust";
        Penta_Thrust.element = "Wind";
        Penta_Thrust.TPCost = 100;        
        Penta_Thrust.weaponSkillElement = WindElement;
        Penta_Thrust.spentAttacks = 5;
        Penta_Thrust.totalAttacks = 5;
        Penta_Thrust.wsAnimTimer = 2.5f;
        Penta_Thrust.wsMultiAttackReportTimer = 0.5f;

        Blade_Tekko.name = "Blade: Tekko";
        Blade_Tekko.element = "Water";
        Blade_Tekko.TPCost = 100;
        Blade_Tekko.weaponSkillElement = WaterElement;
        Blade_Tekko.spentAttacks = 2;
        Blade_Tekko.totalAttacks = 2;
        Blade_Tekko.wsAnimTimer = 1.8f;
        Blade_Tekko.wsMultiAttackReportTimer = 0.9f;

        Earth_Crusher.name = "Earth Crusher";
        Earth_Crusher.element = "Earth";
        Earth_Crusher.TPCost = 100;
        Earth_Crusher.weaponSkillElement = EarthElement;
        Earth_Crusher.spentAttacks = 2;
        Earth_Crusher.totalAttacks = 2;
        Earth_Crusher.wsAnimTimer = 1.8f;
        Earth_Crusher.wsMultiAttackReportTimer = 0.9f;
    }
}