using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPanel : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler

{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Action_Handler Act_Handlr;
    public Enmity_Manager enmMngr;
    public bool panelSelected;

    void Start()
    {
        BM = FindObjectOfType<Battle_Manager>();
        BM_Funcs = FindObjectOfType<Battle_Manager_Functions>();
        Act_Handlr = FindObjectOfType<Action_Handler>();
        enmMngr = FindObjectOfType<Enmity_Manager>();
    }  

    public void OnPointerEnter(PointerEventData eventData)
    {        
        enmMngr.showProvisionalEnmity();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enmMngr.endProvisionalEnmity();
    }
}