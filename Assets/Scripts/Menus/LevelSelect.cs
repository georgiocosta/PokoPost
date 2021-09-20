using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void OnClicked(Button button)
    {
        Debug.Log("Clicked");
        SceneManager.LoadSceneAsync(button.name);
    }
}
