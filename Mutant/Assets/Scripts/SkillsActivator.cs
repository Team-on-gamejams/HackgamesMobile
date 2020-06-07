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
   [SerializeField] private bool isEnemy = false;
   [SerializeField] private BattleManager battle;

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
        battle.onEnemySpawnEvent += OnEnemySpawn;
   }

    void OnEnemySpawn(Monster m) {
        if (isEnemy) {
            currentMonster = m;
        }
        else {
            targetEnemy = m;
        }
    }

   private Monster CheckMonsterIsPlayer(GameObject target)
   {
      return target.GetComponent<Monster>();
   }

   private void MakeKast(float damageMultiper, float heal, float defence, int crit)
   {
      bool critChance = RandomEx.GetEventWithChance(crit);
      float currentDamage = this.currentMonster.Stats[(int) StatType.Attack];
      this.currentMonster.TakeDamage((critChance) ?(currentDamage * damageMultiper * 1.5f) : currentDamage * damageMultiper);
      
      this.currentMonster.Heal((critChance) ? heal * crit : heal);
   }
   
}
