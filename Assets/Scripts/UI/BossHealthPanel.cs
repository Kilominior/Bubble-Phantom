using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthPanel : MonoBehaviour
{
    private List<GameObject> cells;
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private Transform cellsRoot;

    public void Init(int maxHealth)
    {
        cells = new List<GameObject>();
        for(int i = 0; i < maxHealth; i++)
        {
            var cell = Instantiate(cellPrefab, cellsRoot);
            cells.Add(cell);
        }
    }

    public void HealthDecrease()
    {
        if(cells.Count == 0) return;
        cells[^1].GetComponent<Animator>().enabled = true;
        cells.RemoveAt(cells.Count - 1);
    }
}
