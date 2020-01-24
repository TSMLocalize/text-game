using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Battle_Manager_TimerLog : MonoBehaviour
{
    public Battle_Manager BM;
    public int maxBars;
    public List<Bar> barList;
    public List<GameObject> bars;
    public GameObject timerBarPanel;
    public GameObject timerLogBar;

    // Start is called before the first frame update
    void Start()
    {
        maxBars = 5;
        barList = new List<Bar>();
        BM = GetComponent<Battle_Manager>();
    }

    void Update()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].GetComponentInChildren<TextMeshProUGUI>().text = barList[i].playerID.speedTotal + "/100 " + "(" + barList[i].playerID.speed + ")";
            bars[i].GetComponentInChildren<Image>().transform.localScale = new Vector3(Mathf.Clamp((barList[i].playerID.speedTotal / 100), 0, 1),
            bars[i].GetComponentInChildren<Image>().transform.localScale.y,
            bars[i].GetComponentInChildren<Image>().transform.localScale.z);
        }
    }
    
    public class Bar
    {
        public Player playerID;
        public GameObject timerLogBar;        
    }

    // COMBAT LOG FUNCTION
    public void SendBarToTimerLog(Player player)
    {
        if (barList.Count >= maxBars)
        {
            Destroy(barList[0].timerLogBar.gameObject);
            barList.Remove(barList[0]);
        }

        Bar newBar = new Bar();

        newBar.playerID = player;
        newBar.timerLogBar = timerLogBar;        

        Instantiate(newBar.timerLogBar, timerBarPanel.transform);

        newBar.playerID = player;
        newBar.timerLogBar.GetComponentInChildren<TextMeshProUGUI>().text = player.speedTotal + "/100 " + "(" + player.speed + ")";
        newBar.timerLogBar.GetComponentInChildren<Image>().transform.localScale = new Vector3(Mathf.Clamp((player.speedTotal / 100), 0, 1),
        newBar.timerLogBar.GetComponentInChildren<Image>().transform.localScale.y,
        newBar.timerLogBar.GetComponentInChildren<Image>().transform.localScale.z);        

        barList.Add(newBar);
        bars.Add(newBar.timerLogBar);
    }
}
