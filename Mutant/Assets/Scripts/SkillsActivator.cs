using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SkillsActivator : MonoBehaviour, IDropHandler
{
   [SerializeField] private Monster targetEnemy;
   [SerializeField] private Monster currentMonster;
   [SerializeField] private CartObject currentCart;
   public void OnDrop(PointerEventData eventData)
   {
      if (eventData.pointerDrag == null) return;
      eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
         GetComponent<RectTransform>().anchoredPosition;
      this.currentCart = eventData.pointerDrag.GetComponent<CartObject>();
      this.MakeKast(currentCart.damage, currentCart.heal, currentCart.defence, currentCart.critChance);
      eventData.pointerDrag.GetComponent<CartDragHandler>().StartCooldown(currentCart.cooldown);
   }

   public void Awake()
   {
      this.currentMonster = transform.GetComponent<Monster>();
   }

   private Monster CheckMonsterIsPlayer(GameObject target)
   {
      return target.GetComponent<Monster>();
   }

   private void MakeKast(float damageMultiper, float heal, float defence, int crit)
   {
      bool critChance = RandomEx.GetEventWithChance(crit);
      float currentDamage = this.currentMonster.Stats[(int) StatType.Attack];
      // TODO 
      // currentDamage = 10;
      this.currentMonster.TakeDamage((critChance) ?(currentDamage * damageMultiper) : currentDamage);
      Debug.Log((critChance) ?(currentDamage * damageMultiper) : currentDamage);
      
      this.targetEnemy.Heal((critChance) ? heal * crit : heal);
   }
   
}
