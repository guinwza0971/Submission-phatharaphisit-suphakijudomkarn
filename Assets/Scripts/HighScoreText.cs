using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreText : MonoBehaviour
{
    int highScore;
    [SerializeField] TMPro.TextMeshProUGUI myText;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore");
    }

    void Update()
    {
        myText.text = "High Score: " + highScore;
    }
}
