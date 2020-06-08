using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CartsGenerator : MonoBehaviour {
	[SerializeField] private List<GameObject> cartsCells;

	[SerializeField] private Monster currentMonster;
	[SerializeField] private GameObject cartPrefab;
	[SerializeField] private List<GameObject> readyCarts;
	[SerializeField] private Transform activePlace;

	void Start() {
		GameObject item = Instantiate(cartPrefab);
		CartObject currentObject = item.GetComponent<CartObject>();
		currentObject.SetPartSprite(currentMonster.placedParts[0].sr.sprite);
		// TODO throw stats on method 👇 🔽🔽🔽🔽🔽
		currentObject.SetCartStats(0f, 100f, 0f, 50, 5, "Heal <color=green>100</color> HP", 1);
		readyCarts.Add(item);

		item = Instantiate(cartPrefab);
		currentObject = item.GetComponent<CartObject>();
		currentObject.SetPartSprite(currentMonster.placedParts[1].sr.sprite);
		// TODO throw stats on method 👇 🔽🔽🔽🔽🔽
		currentObject.SetCartStats(2f, 0f, 0f, 50, 5, "Deal <color=red>х 2</color> damage", 1);
		readyCarts.Add(item);

		for (int index = 0; index < 2 && readyCarts[index] != null; index++) {
			GameObject currentCart = readyCarts[index];
			currentCart.transform.SetParent(cartsCells[index].transform);
			currentCart.GetComponent<CartDragHandler>().toDefaultPosition = cartsCells[index].transform;
			currentCart.GetComponent<CartDragHandler>().toTopPosition = activePlace;
			currentCart.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
	}
}