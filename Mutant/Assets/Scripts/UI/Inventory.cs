using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	[SerializeField] Button[] buttons;
	[SerializeField] RectTransform[] tabs;

	int selectedId = 0;

	private void Awake() {
		buttons[selectedId].interactable = false;
		for(int i = 0; i < tabs.Length; ++i)
			tabs[i].gameObject.SetActive(i == selectedId);
	}

	public void OpenTab(int id) {
		buttons[selectedId].interactable = true;
		tabs[id].gameObject.SetActive(false);
		selectedId = id;
		buttons[selectedId].interactable = false;
		tabs[id].gameObject.SetActive(true);
	}
}
