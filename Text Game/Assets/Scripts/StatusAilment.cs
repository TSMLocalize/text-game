using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusAilment : MonoBehaviour
{
    public Battle_Manager BM;
    public TextMeshPro StatusTimer;
    public float statusTimerNumber;    

    // Start is called before the first frame update
    void Start()
    {
        statusTimerNumber = 12;
        BM = FindObjectOfType<Battle_Manager>();
        StatusTimer = GetComponentInChildren<TextMeshPro>();
        StartCoroutine(updateStatusAilMentSpeedBars());
    }

    void Update()
    {
        StatusTimer.text = statusTimerNumber.ToString();

        if (BM.startRoutinesGoingAgain)
        {
            StartCoroutine(updateStatusAilMentSpeedBars());
        }
    }
    
    public IEnumerator updateStatusAilMentSpeedBars()
    {
        while (BM.coroutineIsPaused == true)
        {
            yield return null;
        }

        while (BM.coroutineIsPaused == false)
        {
            if (BM.returningStarting == true)
            {
                yield return new WaitForSeconds(0.3f);
                BM.returningStarting = false;
            }
            
            if (statusTimerNumber > 0)
            {
                statusTimerNumber -= 1f;

                yield return new WaitForSeconds(0.5f);
            }
            else if (statusTimerNumber == 0)
            {
                Destroy(this);
            }
        }
    }
}
