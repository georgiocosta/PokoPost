using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankPanel : MonoBehaviour
{
    private Image[] starSprites;
    public Text description;
    Timer timer;

    void Awake()
    {
        starSprites = GetComponentsInChildren<Image>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();

        foreach(Image sprite in starSprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    public void DisplayRank(int stars) {
        Debug.Log(stars);

        if (stars > 0)
        {
            starSprites[stars].gameObject.SetActive(true);
            description.text = "You completed the level in " + timer.GetTime() + " \n and earned the rank of " + stars + " star(s)!";
        }
        else
        {
            description.text = "You completed the level in " + timer.GetTime() + " \n and earned no star rank. \n Try again for a better score!";
        }

        if(stars > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, stars);

        StartCoroutine("ListenForInput");
    }

    public IEnumerator ListenForInput() 
    {
        float timePassed = 0f;

        while (timePassed < 10f)
        {
            if (Input.anyKeyDown && timePassed > 2f)
            {
                Debug.Log("Key Down");
                Time.timeScale = 1;
                SceneManager.LoadSceneAsync("LevelSelect");
            }
            timePassed += Time.unscaledDeltaTime;
            yield return 0;
        }

        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("LevelSelect");
    }
}
