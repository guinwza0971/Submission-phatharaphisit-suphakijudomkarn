using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareColorAdjustment : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    [SerializeField] Lanes myLane;

    void Start()
    {
        spr.color = myLane.noteColor;
    }

}
