using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Spells : MonoBehaviour
{    
    public Spell Fire;
    public Spell Ice;
    public Spell Cure;
    public Spell Firaga;

    private void Start()
    {
        Fire.castTime = 34;
        Ice.castTime = 35;
        Cure.castTime = 46;
        Firaga.castTime = 30;
    }
}
