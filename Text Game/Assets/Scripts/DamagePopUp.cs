using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

        private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        
    }

    public void Setup(string damageAmount, Color color)
    {
        textMesh.SetText(damageAmount.ToString());
        textMesh.color = color;
        textColor = color;
        disappearTimer = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            //Start disappearing
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
