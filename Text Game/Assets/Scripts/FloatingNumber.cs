using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class FloatingNumber : MonoBehaviour
{    
    public Vector3 floatingNumberTarget;
    public TextMeshPro floatingNumberText;
    public GameObject floatingNumber;
    public float speed = 2.0f;


    // Update is called once per frame
    void Update()
    {                
        float step = speed * Time.deltaTime;

        floatingNumber.transform.position = Vector3.MoveTowards(floatingNumber.gameObject.transform.position,
            floatingNumberTarget, step);

         if (floatingNumber.transform.position == floatingNumberTarget)
         {
             Destroy(floatingNumber);
         }
    }
}
