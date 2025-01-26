using UnityEngine;
using UnityEngine.EventSystems;

public class SotryClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.gameObject.SetActive(false);
    }
}
