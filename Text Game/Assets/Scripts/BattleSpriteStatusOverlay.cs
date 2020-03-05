using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSpriteStatusOverlay : MonoBehaviour
{
    public TextMeshProUGUI JosieText;
    public Image currentStatus1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = Camera.main.WorldToScreenPoint(this.transform.position);
        JosieText.transform.position = currentPos;
        currentStatus1.transform.position = currentPos;
    }
}
