using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public float seconds;
    private Text countdownText;

    AudioSource goSound;
    GameObject timer;

    void Start()
    {
        timer = GameObject.Find("Timer");
        timer.SetActive(false);
        countdownText = GetComponentInChildren<Text>();
        Time.timeScale = 0;
        goSound = GetComponents<AudioSource>()[1];
        StartCoroutine("countdown", seconds);
    }

    public IEnumerator countdown(float seconds)
    {
        float timePassed = 0f;

        while(timePassed < seconds)
        {
            timePassed += Time.unscaledDeltaTime;

            if(timePassed >= seconds / 2 && countdownText.text == "Ready?")
            {
                countdownText.text = "GO!";
                goSound.Play();
            }

            yield return 0;
        }

        Time.timeScale = 1;
        timer.SetActive(true);
        gameObject.SetActive(false);
    }
}
