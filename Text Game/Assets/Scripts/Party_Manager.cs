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
        
        Josie.speed = 3f;
        Jemima.speed = 5f;
        Jennifer.speed = 18f;
        Jessica.speed = 18f;
        Jody.speed = 4f;
        Jenny.speed = 2f;

        Josie.castSpeed = 2f;
        Jemima.castSpeed = 3f;
        Jennifer.castSpeed = 10f;
        Jessica.castSpeed = 2f;
        Jody.castSpeed = 3f;
        Jenny.castSpeed = 1f;

        Josie.currentRowPositionID = "Back Row 2";
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Josie.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jemima.currentRowPositionID = "Back Row 1";
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jemima.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jennifer.currentRowPositionID = "Front Row 4";
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jennifer.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jessica.currentRowPositionID = "Front Row 3";
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jessica.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jody.currentRowPositionID = "Front Row 2";
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jody.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);

        Jenny.currentRowPositionID = "Front Row 1";
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Fire);
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Ice);
        Jenny.spellBook.Add(SpellManager.GetComponent<Spells>().Cure);
    }    
}
