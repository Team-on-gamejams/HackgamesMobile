using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BodyPartCard : MonoBehaviour {
	[SerializeField] Image image;
	[SerializeField] TextMeshProUGUI nameField;
	[SerializeField] TextMeshProUGUI descField;
	[SerializeField] TextMeshProUGUI level;

	[SerializeField] TextMeshProUGUI hpField;
	[SerializeField] TextMeshProUGUI armorField;
	[SerializeField] TextMeshProUGUI attackField;
	[SerializeField] TextMeshProUGUI attackRateField;

	public void Init(BodyPart part) {
		image.sprite = part.sr.sprite;

		nameField.text = part.gameName;
		descField.text = part.gameDescription;

		level.text = part.stats[(int)StatType.GearLevel].value.ToString();

		hpField.text = part.stats[(int)StatType.Hp].value.ToString();
		armorField.text = part.stats[(int)StatType.Armor].value.ToString();
		attackField.text = part.stats[(int)StatType.Attack].value.ToString();
		attackRateField.text = part.stats[(int)StatType.AttackSpeed].value.ToString();
	}
}
