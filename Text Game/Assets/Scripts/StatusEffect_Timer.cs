using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffect_Timer : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Renderer>().sortingLayerID =
        this.transform.parent.GetComponent<Renderer>().sortingLayerID;
    }
}
