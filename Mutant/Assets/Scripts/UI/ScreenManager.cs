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
	[SerializeField] Inventory inventory;
	[Header("IdleWindow")][Space]
	[SerializeField] RectTransform idleResultWindow;
	[SerializeField] TMPro.TextMeshProUGUI meatTextField;

	RectTransform battleRect;
	RectTransform inventoryRect;

	Vector3 screenPosUp;
	Vector3 screenPosDown;
	float lastT = 1;

	private void Awake() {
		battleRect = battleCanvas.GetComponent<RectTransform>();
		inventoryRect = inventoryCanvas.GetComponent<RectTransform>();
	}

	private void Start() {
		screenPosUp = Vector2.up * Screen.currentResolution.height;
		screenPosDown = Vector2.down * Screen.currentResolution.height;

		battleRect.anchoredPosition = Vector2.zero;
		inventoryRect.anchoredPosition = screenPosDown;
	}

	public void ShowIldeWindow(float meat) {
		meatTextField.text = ((int)(meat)).ToString();
	}

	public void HideIdleWIndow() {
		idleResultWindow.gameObject.SetActive(false);
	}

	public void OnIdleCloseClick() {
		battle.DropMeatForIdle();

		LeanTween.value(idleResultWindow.gameObject, (Vector3)idleResultWindow.anchoredPosition, GameManager.Instance.Camera.ViewportToScreenPoint(new Vector3(0, 2)), 1.0f)
			.setOnUpdate((Vector3 newPos)=> {
				idleResultWindow.anchoredPosition = newPos;
			})
			.setEase(LeanTweenType.easeInOutBack)
			.setOnComplete(()=> { 
				battle.Continue();
			});
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

		if (inventory.isChangePlayerEquipment) {
			battle.RestartAll();
			inventory.isChangePlayerEquipment = false;
		}
		else {
			battle.Continue();
		}
		playerMonster.ShowHpBar(changeTime / lastT);
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
