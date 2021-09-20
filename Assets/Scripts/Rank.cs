using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rank : MonoBehaviour
{
    public float oneStar;
    public float twoStar;
    public float threeStar;

    int stars;

    public RankPanel rankPanel;

    private void Start()
    {
        stars = 0;
        //rankPanel.gameObject.SetActive(false);
    }

    public void SetRanks(float seconds)
    {

        if(seconds <= threeStar)
        {
            stars = 3;
        }
        else if(seconds <= twoStar)
        {
            stars = 2;
        }
        else if(seconds <= oneStar)
        {
            stars = 1;
        }
        else
        {
            stars = 0;
        }

        rankPanel.gameObject.SetActive(true);
        rankPanel.SendMessage("DisplayRank", stars);
        Time.timeScale = 0;
    }
}
