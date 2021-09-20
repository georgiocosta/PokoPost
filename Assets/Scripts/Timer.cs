using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;

    private float timeElapsed;
    private bool isCounting;
    private Rank rank;

    float seconds;

    void Start()
    {
        timerText = GetComponent<Text>();
        rank = GameObject.Find("Rank").GetComponent<Rank>();
        timeElapsed = 0;
        isCounting = true;
    }

    void LateUpdate()
    {
        if (isCounting)
        {
            timeElapsed += Time.deltaTime;
        }
        DisplayTime(timeElapsed);
    }

    public void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (float)System.Math.Round((timeToDisplay % 1) * 1000, 2);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    public void StopCount()
    {
        isCounting = false;
        rank.SendMessage("SetRanks", seconds);
    }

    public float GetTime()
    {
        return timeElapsed;
    }
}
