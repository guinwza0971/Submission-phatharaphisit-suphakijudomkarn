using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    int myScore;
    [SerializeField] TMPro.TextMeshProUGUI myText;

    void Update()
    {
        myScore = ScoreManager.score;
        myText.text = "Score: " + myScore;
    }
}
