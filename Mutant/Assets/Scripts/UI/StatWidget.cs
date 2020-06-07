using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatWidget : MonoBehaviour {
	[SerializeField] StatType type;
	[Header("Refs")] [Space]
	[SerializeField] TextMeshProUGUI fieldText;
	[SerializeField] Monster monster;
	[SerializeField] bool isJustStat = false;

	private void Awake() {
		if (type == StatType.Hp)
			monster.onHpChangeEvent += OnHpChange;
		else
			monster.onStatsChangeEvent += OnStatChange;
	}

	void OnStatChange() {
		if (type == StatType.CriticalChance)
			fieldText.text = $"{monster.Stats[(int)type].ToString("0")}%";
		else
			fieldText.text = $"{monster.Stats[(int)type].ToString("0.##")}";

	}

	void OnHpChange() {
		if(isJustStat)
			fieldText.text = $"{monster.Stats[(int)type].ToString("0")}";
		else
			fieldText.text = $"{monster.currHp.ToString("0")}/{monster.Stats[(int)type].ToString("0")}";
	}
}
