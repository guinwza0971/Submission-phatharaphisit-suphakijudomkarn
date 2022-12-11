using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBlink : MonoBehaviour
{

    [SerializeField] float blinkTime = 0.05f;
    [SerializeField] float displayTime = 0.2f;

    bool displaying = true;

    float blinkCounter;
    float displayCounter;
    // Update is called once per frame
    void Update()
    {
        if(displaying)
        {
            displayCounter += Time.deltaTime;
            if(displayCounter >= displayTime)
            {
                displayCounter = 0;
                displaying = false;
            }
        }
        else if(!displaying)
        {
            blinkCounter += Time.deltaTime;
            if(blinkCounter >= blinkTime)
            {
                blinkCounter = 0;
                displaying = true;
            }
        }
    }
}
