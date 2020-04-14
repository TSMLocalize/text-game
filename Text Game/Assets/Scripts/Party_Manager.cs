using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Party_Manager : MonoBehaviour
{
    public List<Player> partyMembers;
    public GameObject SpellManager;
    public GameObject WSManager;
    public Player Josie;
    public Player Jemima;
    public Player Jennifer;
    public Player Jessica;
    public Player Jody;
    public Player Jenny;

    void Start()
    {        
        partyMembers.Add(Josie);
        partyMembers.Add(Jemima);
        partyMembers.Add(Jennifer);
        partyMembers.Add(Jessica);
        partyMembers.Add(Jody);
        partyMembers.Add(Jenny);
        
        Josie.speed = 50f;
        Jemima.speed = 1f;
        Jennifer.speed = 1f;
        Jessica.speed = 1f;
        Jody.speed = 1f;
        Jenny.speed = 1f;

        Josie.storeTP = 100f;
        Jemima.storeTP = 1f;
        Jennifer.storeTP = 1f;
        Jessica.storeTP = 1f;
        Jody.storeTP = 1f;
        Jenny.storeTP = 1f;

        Josie.tpTotal = 120f;
        Jemima.tpTotal = 1f;
        Jennifer.tpTotal = 1f;
        Jessica.tpTotal = 1f;
        Jody.tpTotal = 1f;
        Jenny.tpTotal = 1f;

        Josie.castSpeed = 10f;
        Jemima.castSpeed = 10f;
        Jennifer.castSpeed = 2f;
        Jessica.castSpeed = 2f;
        Jody.castSpeed = 3f;
        Jenny.castSpeed = 1f;

        Josie.maxHP = 10f;
        Jemima.maxHP = 10f;
        Jennifer.maxHP = 10f;
        Jessica.maxHP = 10f;
        Jody.maxHP = 10f;
        Jenny.maxHP = 10f;

        Josie.currentHP = 10f;
        Jemima.currentHP = 10f;
        Jennifer.currentHP = 10f;
        Jessica.currentHP = 10f;
        Jody.currentHP = 10f;
        Jenny.currentHP = 10f;

        Josie.Accuracy = 100f;
        Jemima.Accuracy = 100f;
        Jennifer.Accuracy = 15f;
        Jessica.Accuracy = 20f;
        Jody.Accuracy = 25f;
        Jenny.Accuracy = 30f;

        Josie.Evasion = 5f;
        Jemima.Evasion = 10f;
        Jennifer.Evasion = 15f;
        Jessica.Evasion = 20f;
        Jody.Evasion = 25f;
        Jenny.Evasion = 30f;

        Josie.Attack = 5f;
        Jemima.Attack = 10f;
        Jennifer.Attack = 15f;
        Jessica.Attack = 20f;
        Jody.Attack = 25f;
        Jenny.Attack = 30f;

        Josie.Armor = 5f;
        Jemima.Armor = 10f;
        Jennifer.Armor = 15f;
        Jessica.Armor = 20f;
        Jody.Armor = 25f;
        Jenny.Armor = 30f;

        Josie.currentRowPositionID = 1;
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Firaga);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Curaga);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Fast_Blade);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Raging_Axe);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Penta_Thrust);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Earth_Crusher);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Blade_Tekko);
        Josie.weaponSkills.Add(WSManager.GetComponent<WeaponSkills>().Raiden_Thrust);

        Jemima.currentRowPositionID = 2;
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jennifer.currentRowPositionID = 3;
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jessica.currentRowPositionID = 4;
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jody.currentRowPositionID = 5;
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jenny.currentRowPositionID = 6;
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);
    }    
}
