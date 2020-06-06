using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillsActivator : MonoBehaviour, IDropHandler
{
   public void OnDrop(PointerEventData eventData)
   {
      if (eventData.pointerDrag != null)
      {
         eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
            GetComponent<RectTransform>().anchoredPosition;
         // eventData.pointerDrag.GetComponent<CanvasGroup>().alpha = 0.6F;
         eventData.pointerDrag.GetComponent<CartDragHandler>().StartCooldown();
      }
   }
}
