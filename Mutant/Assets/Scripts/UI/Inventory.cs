using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	[SerializeField] Button[] buttons;
	[SerializeField] RectTransform[] tabs;

	[Header("Refs")] [Space]
	[SerializeField] PartsManager partsManager;
	[SerializeField] GameObject partCardPrefab;
	[SerializeField] Transform cardsCenter;

	int selectedId = 0;
	int[] selectedCard;

	private void Awake() {
		buttons[selectedId].interactable = false;

		selectedCard = new int[tabs.Length];
		float lastX = cardsCenter.position.x;

		for (int i = 0; i < tabs.Length; ++i) {
			tabs[i].gameObject.SetActive(i == selectedId);

			Transform[] childs = tabs[i].GetComponentsInChildren<Transform>();
			foreach (var child in childs) {
				if(child != tabs[i].transform)
					Destroy(child.gameObject);
			}

			BodyPart[] parts = partsManager.GetAllOwnedByPlayer((BodyPartType)i);
			for(int j = parts.Length - 1; j >= 0; --j) {
				BodyPartCard card = Instantiate(partCardPrefab, cardsCenter.position, Quaternion.identity, tabs[i].transform).GetComponent<BodyPartCard>();
				card.Init(parts[j]);

				card.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.33f, Mathf.Abs(selectedCard[i] - j) / 2.0f);
				card.transform.position = new Vector3(lastX, cardsCenter.position.y);
				lastX -= card.rectTransform.sizeDelta.x * card.rectTransform.lossyScale.x;
			}
		}
	}

	public void OpenTab(int id) {
		buttons[selectedId].interactable = true;
		tabs[selectedId].gameObject.SetActive(false);
		selectedId = id;
		buttons[selectedId].interactable = false;
		tabs[selectedId].gameObject.SetActive(true);
	}
}
