using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BodyPartCard : MonoBehaviour {
	public RectTransform rectTransform;
	[System.NonSerialized] public BodyPart part;

	[SerializeField] Image image;
	[SerializeField] TextMeshProUGUI nameField;
	[SerializeField] TextMeshProUGUI descField;
	[SerializeField] TextMeshProUGUI level;

	[SerializeField] TextMeshProUGUI hpField;
	[SerializeField] TextMeshProUGUI armorField;
	[SerializeField] TextMeshProUGUI attackField;
	[SerializeField] TextMeshProUGUI attackRateField;

	public void Init(BodyPart _part, PartsManager parts) {
		part = _part;

		image.sprite = part.sr.sprite;

		nameField.text = part.gameName;
		descField.text = part.gameDescription;

		int lvl = parts.GetPlayerLevel(part);
		level.text = lvl.ToString();

		hpField.text = GetStatWithBonus(StatType.Hp).ToString();
		armorField.text = GetStatWithBonus(StatType.Armor).ToString();
		attackField.text = GetStatWithBonus(StatType.Attack).ToString();
		attackRateField.text = GetStatWithBonus(StatType.AttackSpeed).ToString();

		float GetStatWithBonus(StatType type) {
			return part.stats[(int)type].value + part.stats[(int)type].value * lvl * part.stats[(int)type].growPerLevel;
		}
	}
}
