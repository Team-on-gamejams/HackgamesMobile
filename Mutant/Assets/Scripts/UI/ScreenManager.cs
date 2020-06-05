using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {
	[SerializeField] CanvasGroup battleCanvas;
	[SerializeField] CanvasGroup inventoryCanvas;

	RectTransform battleRect;
	RectTransform inventoryRect;

	private void Awake() {
		battleRect = battleCanvas.GetComponent<RectTransform>();
		inventoryRect = inventoryCanvas.GetComponent<RectTransform>();

		//inventoryRect.anchoredPosition
	}

	public void ShowBattleScree() {

	}

	public void ShowInventoryScreen() {

	}
}
