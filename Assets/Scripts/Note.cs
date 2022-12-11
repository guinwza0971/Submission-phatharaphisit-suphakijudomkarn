using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    float noteSpeed;
    float noteLife = 10f;
    float noteTimer;
    //Color noteColor;

    [SerializeField] SpriteRenderer spr;

    void Start()
    {
        noteTimer = 0;
    }

    void Update()
    {


        transform.Translate(new Vector3(0, Time.deltaTime*-noteSpeed, 0));
        noteTimer += Time.deltaTime;
        if(noteTimer >= noteLife)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(Color color, float speed, float life)
    {
        noteSpeed = speed;
        noteLife = life;
        spr.color = color;
    }
}
