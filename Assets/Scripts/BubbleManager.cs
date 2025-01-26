using System;
using System.Collections.Generic;
using QFramework;
using Unity.Mathematics;
using UnityEngine;

public class BubbleManager: SingletonMono<BubbleManager>
{
    [SerializeField]
    private GameObject bubblePrefab;
    [SerializeField]
    private float bubbleSpawnTime;

    private List<Bubble> bubbles;
// public******************************************************************************
    public void SpawnBubble(float motherHealth, Vector3 position, bool isInitial = false)
    {
        var bubbleObj = Instantiate(bubblePrefab, position, quaternion.identity, transform);
        Bubble bubble = bubbleObj.GetComponent<Bubble>();
        bubble.Spawn(bubbleSpawnTime, motherHealth, isInitial);
        bubbles.Add(bubble);
    }

    public void RemoveBubble(Bubble bubble)
    {
        bubbles.Remove(bubble);
    }

    // 选择一个bubble，在其上复活
    public Bubble QueryAvailableBubble()
    {
        if(bubbles.Count == 0) return null;
        int choiceId = UnityEngine.Random.Range(0, bubbles.Count);
        Bubble bestChoice = bubbles[choiceId];
        bubbles.RemoveAt(choiceId);
        return bestChoice;
    }

// private******************************************************************************
    private void Awake() {
        bubbles = new List<Bubble>();
        SpawnBubble(0, transform.position, true);
    }
}
