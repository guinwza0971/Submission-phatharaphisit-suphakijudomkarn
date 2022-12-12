using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class PlaymodeTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NoteSpawnPositionVectorTest()
    {
        GameObject noteIndicator = new GameObject();
        
        GameObject laneA = new GameObject();

        Lanes lanes = laneA.AddComponent<Lanes>();
        
        float travelTime = 3f;
        float speed = 5f;

        Vector3 spawnLocation = lanes.GetSpawnLocation(noteIndicator, travelTime, speed);

        Assert.IsTrue(spawnLocation == noteIndicator.transform.position + new Vector3(0,travelTime*speed,0));
    }

}
