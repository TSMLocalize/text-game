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
    public Spell Curaga;
    public Spell Firaga;

    private void Start()
    {
        Fire.castTime = 34;
        Ice.castTime = 35;
        Cure.castTime = 46;
        Cure.isSupport = true;
        Curaga.castTime = 32;
        Curaga.isAoE = true;
        Curaga.isSupport = true;
        Firaga.castTime = 30;
        Firaga.isAoE = true;
    }
}
