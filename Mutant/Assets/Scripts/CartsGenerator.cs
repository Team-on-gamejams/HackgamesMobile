using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CartsGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> cartsCells;

    [SerializeField] private Monster currentMonster;
    [SerializeField] private GameObject cartPrefab;
    [SerializeField] private List<GameObject> readyCarts;
    [SerializeField] private Transform activePlace;

    void Start()
    {
        foreach (BodyPart part in currentMonster.placedParts)
        {
            Debug.Log(part.sr.sprite);
            GameObject item = Instantiate(cartPrefab);
            CartObject currentObject = item.GetComponent<CartObject>();
            currentObject.SetPartSprite(part.sr.sprite);
            // TODO throw stats on method 👇 🔽🔽🔽🔽🔽
            currentObject.SetCartStats(10f, 5f, 0f, 50, 5, "Custom text", (int) Random.Range(1f, 1000f));
            readyCarts.Add(item);
        }

        for (int index = 0; index < cartsCells.Count && readyCarts[index] != null; index++)
        {
            GameObject currentCart = readyCarts[index];
            currentCart.transform.SetParent(cartsCells[index].transform);
            currentCart.GetComponent<CartDragHandler>().toDefaultPosition = cartsCells[index].transform;
            currentCart.GetComponent<CartDragHandler>().toTopPosition = activePlace;
            currentCart.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}