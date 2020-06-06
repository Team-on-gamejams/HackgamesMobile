using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {
	[Header("UI")][Space]
	[SerializeField] CanvasGroup battleCanvas;
	[SerializeField] CanvasGroup inventoryCanvas;
	[SerializeField] float changeTime = 0.2f;

	[Header("Refs")][Space]
	[SerializeField] BattleManager battle;
	[SerializeField] Monster playerMonster;

	RectTransform battleRect;
	RectTransform inventoryRect;

	Vector3 screenPosUp;
	Vector3 screenPosDown;
	float lastT = 1;

	private void Awake() {
		battleRect = battleCanvas.GetComponent<RectTransform>();
		inventoryRect = inventoryCanvas.GetComponent<RectTransform>();

		screenPosUp = GameManager.Instance.Camera.ViewportToScreenPoint(new Vector3(0, 1.0f, 0));
		screenPosDown = GameManager.Instance.Camera.ViewportToScreenPoint(new Vector3(0, -1.0f, 0));

		battleRect.anchoredPosition = Vector2.zero;
		inventoryRect.anchoredPosition = screenPosDown;
	}

	public void ShowBattleScreen() {
		LeanTween.cancel(gameObject, false);
		if (lastT == 0)
			lastT = 0.01f;
		LeanTween.value(gameObject, 1 - lastT, 1, changeTime / lastT)
			.setOnUpdate((float t) => {
				battleRect.anchoredPosition = Vector3.Lerp(screenPosUp, Vector3.zero, t);
				inventoryRect.anchoredPosition = Vector3.Lerp(Vector3.zero, screenPosDown, t);
				lastT = t;
			});

		battle.Continue();
		playerMonster.ShowHpBar(changeTime / lastT);
		// if any value in inventory changed
		// battle.RestartAll();
	}

	public void ShowInventoryScreen() {
		LeanTween.cancel(gameObject, false);
		if (lastT == 0)
			lastT = 0.01f;
		LeanTween.value(gameObject, 1 - lastT, 1, changeTime / (lastT))
			.setOnUpdate((float t) => {
				battleRect.anchoredPosition = Vector3.Lerp(Vector3.zero, screenPosUp, t);
				inventoryRect.anchoredPosition = Vector3.Lerp(screenPosDown, Vector3.zero, t);
				lastT = t;
			});

		battle.Pause();
		playerMonster.HideHpBar(changeTime / lastT);
	}
}
