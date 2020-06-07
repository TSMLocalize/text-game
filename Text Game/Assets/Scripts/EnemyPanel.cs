using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyPanel : MonoBehaviour    
    , IPointerEnterHandler
    , IPointerExitHandler

{
    public Battle_Manager BM;
    public Battle_Manager_Functions BM_Funcs;
    public Action_Handler Act_Handlr;

    void Start()
    {
        BM = FindObjectOfType<Battle_Manager>();
        BM_Funcs = FindObjectOfType<Battle_Manager_Functions>();
        Act_Handlr = FindObjectOfType<Action_Handler>();
    }

    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showProvisionalEnmity();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        endProvisionalEnmity();
    }

    public void showProvisionalEnmity()
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < Act_Handlr.enmityFigures.Count; i++)
            {
                Act_Handlr.enmityFigures[i].EnmityPercentage.color = Color.green;
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (this.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {                    
                    Act_Handlr.IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], 1f);
                    Act_Handlr.enmityFigures[i].EnmityPercentage.color = Color.red;
                }
            }            

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                Act_Handlr.UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], Act_Handlr.enmityFigures[i]);
            }
        }
    }

    public void endProvisionalEnmity()
    {
        if (BM.battleStates == Battle_Manager.BattleStates.SELECT_TARGET)
        {
            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                if (this.gameObject == BM.EnemiesInBattle[i].enemyPanel)
                {
                    Act_Handlr.IncreaseEnmity(BM.activePlayer, BM.EnemiesInBattle[i], -1f);
                }
            }

            for (int i = 0; i < BM.EnemiesInBattle.Count; i++)
            {
                Act_Handlr.UpdateEnmityNumber(BM.activePlayer, BM.EnemiesInBattle[i], Act_Handlr.enmityFigures[i]);
                Act_Handlr.enmityFigures[i].EnmityPercentage.color = Color.white;
            }
        }
    }
}