using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartObject : MonoBehaviour
{

  [SerializeField] Sprite cartSprite;
  [SerializeField] int dmg;
  [SerializeField] int defence;
  [SerializeField] int heal;
  [SerializeField] SpriteRenderer iconRenderer;
  [SerializeField] SpriteRenderer overlayRenderer;
  [SerializeField] int curentOverlay = 0;
  [SerializeField] List<Sprite> overlay;
  [SerializeField] Sprite icon;


  void Awake()
  {
      overlayRenderer.sprite = overlay[curentOverlay];
      iconRenderer.sprite = icon;
  }

}
