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
			fieldText.text = $"{monster.Stats[(int)type]}%";
		else
			fieldText.text = $"{monster.Stats[(int)type]}";

	}

	void OnHpChange() {
		if(isJustStat)
			fieldText.text = $"{(int)monster.Stats[(int)type]}";
		else
			fieldText.text = $"{(int)monster.currHp}/{(int)monster.Stats[(int)type]}";
	}
}
