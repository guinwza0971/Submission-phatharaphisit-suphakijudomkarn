using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaneText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] Lanes myLane;

    void Update()
    {
        myText.text = myLane.myLaneInputText;

        if(Input.GetKeyDown(myLane.input))
        {
            myText.fontStyle = TMPro.FontStyles.Bold;
            myText.color = Color.black;
        }
        if(Input.GetKeyUp(myLane.input))
        {
            myText.fontStyle = TMPro.FontStyles.Normal;
            myText.color = Color.white;
        }
    }
}
