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

	int selectedId = 0;

	private void Awake() {
		buttons[selectedId].interactable = false;
		for(int i = 0; i < tabs.Length; ++i) {
			tabs[i].gameObject.SetActive(i == selectedId);

			Transform[] childs = tabs[i].GetComponentsInChildren<Transform>();
			foreach (var child in childs) {
				if(child != tabs[i].transform)
					Destroy(child.gameObject);
			}

			BodyPart[] parts = partsManager.GetAllOwnedByPlayer((BodyPartType)i);
			for(int j = parts.Length - 1; j >= 0; --j) {
				BodyPartCard card = Instantiate(partCardPrefab, tabs[i].position, Quaternion.identity, tabs[i].transform).GetComponent<BodyPartCard>();
				card.transform.localScale = Vector3.Lerp(Vector3.one , Vector3.one * 0.33f, (float)(j) / parts.Length);
				card.transform.localPosition += Vector3.right * ((j - parts.Length / 2) * 250 + 150); 
				card.Init(parts[j]);
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
