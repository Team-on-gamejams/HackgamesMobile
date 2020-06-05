using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatWidget : MonoBehaviour {
	[SerializeField] StatType type;
	[Header("Refs")][Space]
	[SerializeField] TextMeshProUGUI fieldText;
	[SerializeField] Monster monster;

	private void Awake() {
		if(type == StatType.Hp)
			monster.onHpChangeEvent += OnHpChange;
		else
			monster.onStatsChangeEvent += OnStatChange;
	}

	void OnStatChange() {
		fieldText.text = $"{monster.Stats[(int)type]}";
	}

	void OnHpChange() {
		fieldText.text = $"{(int)monster.currHp}/{(int)monster.Stats[(int)type]}\n(+{monster.Stats[(int)StatType.HpRegen].ToString("0.0")}/sec)";
	}
}
