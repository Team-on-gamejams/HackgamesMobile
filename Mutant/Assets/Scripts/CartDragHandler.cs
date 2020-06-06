using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform toTopPosition;
    [SerializeField] private Transform toDefaultPosition;
    [SerializeField] private float useRange = 2f;
    [SerializeField] private Vector2 EmemyPosition;
    [SerializeField] private Vector2 MonsterPosition;
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
