using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Result : MonoBehaviour
{
    public Text currentScore;
    public Text highScore;

    private void OnEnable()
    {
        currentScore.text = GameManager.instance.currentScore.ToString("N0");
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString("N0");
    }
}
