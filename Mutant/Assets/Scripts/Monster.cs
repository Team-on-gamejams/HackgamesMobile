using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
	public Action onStatsChangeEvent;
	public Action onHpChangeEvent;
	public Action onDie;

	public float[] Stats;
	public float currHp;

	[SerializeField] bool IsPlayer;
	public List<BodyPart> usedBodyParts;

	List<BodyPart> placedParts = new List<BodyPart>();
	float oneAttackTime = 0.0f;
	float currAttackTimer = 0.0f;

	private void Awake() {
		Stats = new float[(int)StatType.LAST_STAT];
	}

	public void RegenerateHealth() {
		if(currHp < Stats[(int)StatType.Hp]) {
			currHp += Stats[(int)StatType.HpRegen] * Time.deltaTime;
			if (currHp > Stats[(int)StatType.Hp])
				currHp = Stats[(int)StatType.Hp];
		onHpChangeEvent?.Invoke();
		}
	}

	public bool IsCanDoDamage() {
		currAttackTimer += Time.deltaTime;

		return currAttackTimer >= oneAttackTime;
	}

	public float DoDamage() {
		currAttackTimer -= oneAttackTime;

		float dmg = Stats[(int)StatType.Attack];
		bool isCritical = RandomEx.GetEventWithChance(Mathf.RoundToInt(Stats[(int)StatType.CriticalChance]));
		if (isCritical)
			dmg *= 1.5f;

		return dmg;
	}

	public void TakeDamage(float damage) {
		if (currHp <= 0 || damage <= 1)
			return;

		damage -= Stats[(int)StatType.Armor];
		if (damage <= 1)
			damage = 1;

		currHp -= damage;
		if(currHp <= 0) {
			currHp = 0;
			onDie?.Invoke();
		}
	}

	public void RecreateBodyParts() {
		foreach (var part in placedParts)
			Destroy(part.gameObject);
		placedParts.Clear();

		placedParts.Add(GetInstantiatedPart(BodyPartType.Body));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Arms));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Legs));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Tail));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Wings));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Fur));

		placedParts.Add(GetInstantiatedPart(BodyPartType.Head));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Horns));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Teeth));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Eyes));

		RecalcStats();
	}

	public void RecalcStats() {
		for (int i = 0; i < Stats.Length; ++i) {
			Stats[i] = 0.0f;
		}

		for (int i = 0; i < placedParts.Count; ++i) {
			for (int j = 0; j < placedParts[i].stats.Length; ++j) {
				Stats[j] += placedParts[i].stats[j].value;
			}
		}

		currHp = Stats[(int)StatType.Hp];
		oneAttackTime = 1.0f / (Stats[(int)StatType.AttackSpeed] != 0 ? Stats[(int)StatType.AttackSpeed] : 1);

		onStatsChangeEvent?.Invoke();
		onHpChangeEvent?.Invoke();
	}

	BodyPart GetInstantiatedPart(BodyPartType type) {
		for (int i = 0; i < usedBodyParts.Count; ++i) {
			BodyPart partPrefab = usedBodyParts[i];
			if (partPrefab.type == type) {
				Vector3 pos = transform.position;
				Transform parent = transform;

				for (int j = 0; j < placedParts.Count; ++j) {
					for (int k = 0; k < placedParts[j].connectionParts.Length; ++k) {
						if (partPrefab.type == placedParts[j].connectionParts[k]) {
							pos = placedParts[j].connectionPoints[k].position;
							parent = placedParts[j].connectionPoints[k];
							break;
						}
					}
					if (parent != transform)
						break;
				}

				return Instantiate(partPrefab, pos, Quaternion.identity, parent).GetComponent<BodyPart>();
			}
		}

		return null;
	}
}
