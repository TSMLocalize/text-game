using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Party_Manager : MonoBehaviour
{
    public List<Player> partyMembers;
    public GameObject SpellManager;
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
        
        Josie.speed = 20f;
        Jemima.speed = 10f;
        Jennifer.speed = 15f;
        Jessica.speed = 18f;
        Jody.speed = 17f;
        Jenny.speed = 14f;

        Josie.castSpeed = 2f;
        Jemima.castSpeed = 3f;
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

        Josie.currentRowPositionID = 1;
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

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
