using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private Image[] starSprites;

    void Awake()
    {
        starSprites = GetComponentsInChildren<Image>();

        int stars = PlayerPrefs.GetInt(GetComponent<Button>().name, 0);

        for(int i=1;i<4;i++)
        {
            starSprites[i].gameObject.SetActive(false);
        }

        if (stars > 0)
        {
            starSprites[stars].gameObject.SetActive(true);
        }
    }
}
