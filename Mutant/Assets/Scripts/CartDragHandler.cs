using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform toTopPosition;
    [SerializeField] private Transform toDefaultPosition;
    [SerializeField] private float useRange = 2f;
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toTopPosition.transform.GetComponent<RectTransform>().anchoredPosition = toDefaultPosition.GetComponent<RectTransform>().anchoredPosition;
        transform.localScale *= 1.2f;
        transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(0,120f);
        transform.SetParent(toTopPosition);
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.SetParent(toDefaultPosition);
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.localScale = Vector3.one;
    }
    
}
