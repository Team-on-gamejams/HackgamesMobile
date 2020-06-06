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
            eventData.pointerDrag.GetComponent<CartDragHandler>().StartCooldown();
            //TODO Uncommit when used to monstEEEr
         // eventData.pointerDrag.GetComponent<Monster>().currHp -= GetComponent<CartDragHandler>().damage;
      }
   }
}
