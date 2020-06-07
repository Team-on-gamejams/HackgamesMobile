using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform toTopPosition;
    [SerializeField] private Transform toDefaultPosition;

    public float damage = 200;
    private bool onCooldown = false;
    public void OnDrag(PointerEventData eventData)
    {
        if(!this.onCooldown)transform.position = Input.mousePosition;
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
        transform.localScale = Vector3.one;
        transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(-40f,155f);
        transform.SetParent(toTopPosition);

    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.SetParent(toDefaultPosition);
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.localScale *= 0.7f;

    }

    public void StartCooldown(int cooldownTime)
    {
        StartCoroutine(SetCooldown(transform.GetComponent<CanvasGroup>(), cooldownTime));
    }

    IEnumerator SetCooldown(CanvasGroup target, int timeout)
    {
        target.alpha = 0.6f;
        this.onCooldown = true;
        yield return new WaitForSeconds(timeout);
        this.onCooldown = false;
        target.alpha = 1;
    }
    
    
}
