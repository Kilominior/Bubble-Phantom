using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour, IPointerClickHandler
{
    private List<Image> tutorials;
    private int curId;
    // public******************************************************************************
    public void OnPointerClick(PointerEventData eventData)
    {
        if(curId == tutorials.Count)
        {
            for(int i = 0; i < tutorials.Count; i++)
            {
                tutorials[i].enabled = false;
            }
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            tutorials[curId++].enabled = true;
        }
    }

    // private******************************************************************************
    private void OnEnable()
    {
        tutorials = new List<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            tutorials.Add(transform.GetChild(i).GetComponent<Image>());
            tutorials[i].enabled = false;
        }
        curId = 0;
    }
}
