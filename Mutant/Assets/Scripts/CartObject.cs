using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CartObject : MonoBehaviour
{
    
   [Multiline] [SerializeField] public string  description;
   [SerializeField] public float  damage;
   [SerializeField] public float  defence;
   [SerializeField] public float  heal;
   [SerializeField] public int  critChance;
   [SerializeField] public int  cooldown;
   [SerializeField] public int  gear;

   [Header("Text inputs")] [Space] [SerializeField]
   public TextMeshProUGUI descriptionField;
   public TextMeshProUGUI damageField;
   public TextMeshProUGUI defenceField;
   public TextMeshProUGUI healField;
   public TextMeshProUGUI critChanceField;
   public TextMeshProUGUI gearField;

   [Header("Images Sources")] [Space] [SerializeField]
   public Sprite cartOverlay;
   public Sprite cartBorder;
   public Sprite cartPart;

   [Header("Images Places")] [Space] [SerializeField] 
   public Image overlayPosition;
   public Image borderPosition;
   public Image partPosition;
   private void Awake()
   {
      descriptionField.text = description;
      damageField.text = damage.ToString(CultureInfo.InvariantCulture);
      healField.text = heal.ToString(CultureInfo.InvariantCulture);
      defenceField.text = defence.ToString(CultureInfo.InvariantCulture);
      critChanceField.text = critChance.ToString();
      gearField.text = gear.ToString();

        damageField.gameObject.SetActive(damage != 0);
        healField.gameObject.SetActive(heal != 0);
        defenceField.gameObject.SetActive(defence != 0);
        critChanceField.gameObject.SetActive(critChance != 0);

        overlayPosition.sprite = cartOverlay;
      borderPosition.sprite = cartBorder;
      partPosition.sprite = cartPart;

   }

   public void SetPartSprite(Sprite sprite)
   {
      this.cartPart = sprite;
   }

   public void SetCartStats(float damage, float heal, float deffence, int crit, int cd, string description, int gear)
   {
      this.gear = gear;
      this.description = description;
      this.cooldown = cd;
      this.damage = damage;
      this.heal = heal;
      this.defence = deffence;
      this.critChance = crit;
      
      this.Awake();
   }
}
