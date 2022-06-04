using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public void ClickPlay() 
    {
        SceneManager.LoadScene("Game");
    }
    
    void Start() 
    {
        int highscore = PlayerPrefs.GetInt("highscore");
        GameObject.Find("Highscore").GetComponent<TextMeshProUGUI>().text = "HIGHSCORE: " + highscore;
    }
}
