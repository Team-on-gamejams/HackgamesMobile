using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
	const string equip = "Екіпірувати";
	const string equiped = "Екіпіровано";

	[SerializeField] Button[] buttons;
	[SerializeField] RectTransform[] tabs;

	[Header("Refs")] [Space]
	[SerializeField] PartsManager partsManager;
	[SerializeField] Monster playerMonster;
	[SerializeField] GameObject partCardPrefab;
	[SerializeField] Transform cardsCenter;
	[SerializeField] CanvasGroup bottomBar;
	[SerializeField] Button buttonEquip;
	[SerializeField] TMPro.TextMeshProUGUI buttonEquipText;

	int selectedId = 0;
	int[] selectedCard;

	float cardWidth = 350f;
	float currDrag = 0.0f;

	List<List<BodyPartCard>> cards = new List<List<BodyPartCard>>();
	public bool isChangePlayerEquipment = false;

	private void Awake() {
		buttons[selectedId].interactable = false;
		selectedCard = new int[tabs.Length];

		partsManager.OnNewPlayerPartsAvaliable += RecreateCards;
	}

	public void OpenTab(int id) {
		buttons[selectedId].interactable = true;
		tabs[selectedId].gameObject.SetActive(false);
		selectedId = id;
		buttons[selectedId].interactable = false;
		tabs[selectedId].gameObject.SetActive(true);
	}

	public void OnBeginDrag(PointerEventData eventData) {
		LeanTween.cancel(gameObject);
		bottomBar.blocksRaycasts = false;
		bottomBar.interactable = false;
	}

	public void OnEndDrag(PointerEventData eventData) {
		LeanTween.value(gameObject, currDrag, 0, Mathf.Abs(currDrag) / cardWidth / 3.0f)
			.setOnUpdate((float drag)=> {
				currDrag = drag;
				MoveCards(selectedId);
			})
			.setOnComplete(()=>{
				bottomBar.blocksRaycasts = true;
				bottomBar.interactable = true;
			});

		LeanTween.value(gameObject, 0, 1, Mathf.Abs(currDrag) / cardWidth / 3.0f)
			.setOnUpdate((float t) => {
				bottomBar.alpha = Mathf.Lerp(0, 1, t);
			})
			.setOnComplete(() => {
				bottomBar.blocksRaycasts = true;
				bottomBar.interactable = true;
			});
	}

	public void OnDrag(PointerEventData eventData) {
		currDrag += eventData.delta.x;
		if(bottomBar.alpha > 0.2f) {
			bottomBar.alpha -= currDrag / cardWidth / 4;
			if (bottomBar.alpha < 0.2f)
				bottomBar.alpha = 0.2f;
		}

		if (currDrag >= cardWidth / 2 && selectedCard[selectedId] != cards[selectedId].Count - 1) {
			currDrag -= cardWidth;
			selectedCard[selectedId]++;
		}
		else if (currDrag <= -cardWidth / 2 && selectedCard[selectedId] != 0) {
			currDrag += cardWidth;
			selectedCard[selectedId]--;
		}

		if(selectedCard[selectedId] == 0)
			currDrag = Mathf.Clamp(currDrag, -cardWidth / 4, cardWidth);
		else if(selectedCard[selectedId] == cards[selectedId].Count - 1)
			currDrag = Mathf.Clamp(currDrag, -cardWidth, cardWidth / 4);
		MoveCards(selectedId);
	}

	public void OnEquipClick() {
		BodyPart part = cards[selectedId][selectedCard[selectedId]].part;
		BodyPart curr = partsManager.GetUsedByPlayer(part.type);

		playerMonster.usedBodyParts.Remove(curr);
		curr.isEquipedByPlayer = false;
		
		playerMonster.usedBodyParts.Add(part);
		part.isEquipedByPlayer = true;

		isChangePlayerEquipment = true;
		playerMonster.RecreateBodyParts(true);

		buttonEquipText.text = equiped;
		buttonEquip.interactable = false;
	}

	public void OnUpgradeClick() {

	}

	void RecreateCards() {
		cards.Clear();
		for (int i = 0; i < tabs.Length; ++i) {
			cards.Add(new List<BodyPartCard>());
			tabs[i].gameObject.SetActive(i == selectedId);

			Transform[] childs = tabs[i].GetComponentsInChildren<Transform>();
			foreach (var child in childs) {
				if (child != tabs[i].transform)
					Destroy(child.gameObject);
			}

			BodyPart[] parts = partsManager.GetAllOwnedByPlayer((BodyPartType)i);

			for (int j = 0; j < selectedCard[i]; ++j) {
				BodyPartCard card = Instantiate(partCardPrefab, cardsCenter.position, Quaternion.identity, tabs[i].transform).GetComponent<BodyPartCard>();
				card.Init(parts[j]);
				cards[i].Add(card);
			}

			for (int j = parts.Length - 1; j >= selectedCard[i]; --j) {
				BodyPartCard card = Instantiate(partCardPrefab, cardsCenter.position, Quaternion.identity, tabs[i].transform).GetComponent<BodyPartCard>();
				card.Init(parts[j]);
				cards[i].Add(card);
			}

			MoveCards(i);
		}
	}

	void MoveCards(int selectedId) {
		for (int j = 0; j < cards[selectedId].Count; ++j) {
			cards[selectedId][j].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.33f, Mathf.Abs(selectedCard[selectedId] - j + currDrag / cardWidth) / 2.0f);
			cards[selectedId][j].transform.position = new Vector3(
				cardsCenter.position.x - (selectedCard[selectedId] - j + currDrag / cardWidth) * cards[selectedId][j].rectTransform.sizeDelta.x * Mathf.Pow(0.75f, Mathf.Abs(selectedCard[selectedId] - j + currDrag / cardWidth)),
				cardsCenter.position.y);

			cards[selectedId][j].transform.SetAsLastSibling();
		}

		for (int j = cards[selectedId].Count - 1; j >= selectedCard[selectedId]; --j) {
			cards[selectedId][j].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.33f, Mathf.Abs(selectedCard[selectedId] - j + currDrag / cardWidth) / 2.0f);
			cards[selectedId][j].transform.position = new Vector3(
				cardsCenter.position.x - (selectedCard[selectedId] - j + currDrag / cardWidth) * cards[selectedId][j].rectTransform.sizeDelta.x * Mathf.Pow(0.75f, Mathf.Abs(selectedCard[selectedId] - j + currDrag / cardWidth)),
				cardsCenter.position.y);

			cards[selectedId][j].transform.SetAsLastSibling();
		}

		cards[selectedId][selectedCard[selectedId]].transform.SetAsLastSibling();
		if (cards[selectedId][selectedCard[selectedId]].part.isEquipedByPlayer) {
			buttonEquipText.text = equiped;
			buttonEquip.interactable = false;
		}
		else {
			buttonEquipText.text = equip;
			buttonEquip.interactable = true;
		}
	}
}
