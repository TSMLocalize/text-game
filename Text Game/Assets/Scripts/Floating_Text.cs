using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Floating_Text : MonoBehaviour
{
    public Transform BM;
    private void Start()
    {
        BM = FindObjectOfType<Battle_Manager>().pfDamagePopup;
    }
    public Floating_Text Create(Vector3 position, float damageAmount)
    {
        Transform damagePopupTransform = Instantiate(BM, position, Quaternion.identity);
        Floating_Text floatingText = damagePopupTransform.GetComponent<Floating_Text>();
        floatingText.Setup(damageAmount);

        return floatingText;
    }
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
    }
}
