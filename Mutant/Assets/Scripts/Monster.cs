using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour {
	public Action onStatsChangeEvent;
	public Action onHpChangeEvent;
	public Action onDie;

	[NonSerialized] public float[] Stats;
	[NonSerialized] public float currHp;

	[SerializeField] bool IsPlayer = false;
	public List<BodyPart> usedBodyParts;

	[Header("Health")] [Space]
	[SerializeField] float healBarTime = 0.2f;
	[SerializeField] Image healthBarImage;
	[SerializeField] Image healthBarImageLowAnim;
	[SerializeField] Image healthBarImageBack;

	List<BodyPart> placedParts = new List<BodyPart>();
	float oneAttackTime = 0.0f;
	float currAttackTimer = 0.0f;

	private void Awake() {
		Stats = new float[(int)StatType.LAST_STAT];

		onHpChangeEvent += OnHpChange;
	}

	public void ShowHpBar(float time) {
		LeanTween.value(gameObject, healthBarImage.color.a, 1.0f, time)
			.setOnUpdate((float a) => {
				Color c = healthBarImage.color;
				c.a = a;
				healthBarImage.color = c;
				c = healthBarImageLowAnim.color;
				c.a = a;
				healthBarImageLowAnim.color = c;
				c = healthBarImageBack.color;
				c.a = a;
				healthBarImageBack.color = c;
			});
	}

	public void HideHpBar(float time) {
		LeanTween.value(gameObject, healthBarImage.color.a, 0.0f, time)
			.setOnUpdate((float a) => {
				Color c = healthBarImage.color;
				c.a = a;
				healthBarImage.color = c;
				c = healthBarImageLowAnim.color;
				c.a = a;
				healthBarImageLowAnim.color = c;
				c = healthBarImageBack.color;
				c.a = a;
				healthBarImageBack.color = c;
			});
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
		if (currHp == 0)
			return false;

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
		if (currHp <= 0) {
			currHp = 0;
			foreach (var part in placedParts)
				part.OnDie();
			onDie?.Invoke();
		}

		onHpChangeEvent?.Invoke();
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

	public void ResetHealth() {
		currHp = Stats[(int)StatType.Hp];
		onHpChangeEvent?.Invoke();
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

	void OnHpChange() {
		if(currHp / Stats[(int)StatType.Hp] >= healthBarImage.fillAmount) {
			LeanTween.cancel(healthBarImage.gameObject, false);

			LeanTween.value(healthBarImage.gameObject, healthBarImage.fillAmount, currHp / Stats[(int)StatType.Hp], healBarTime)
			.setOnUpdate((float val) => {
				healthBarImage.fillAmount = val;
			});
		}
		else {
			LeanTween.cancel(healthBarImageLowAnim.gameObject, false);
			LeanTween.cancel(healthBarImage.gameObject, false);

			healthBarImage.fillAmount = currHp / Stats[(int)StatType.Hp];


			LeanTween.value(healthBarImageLowAnim.gameObject, healthBarImageLowAnim.fillAmount, healthBarImage.fillAmount, 0.2f)
			.setDelay(0.13f)
			.setOnUpdate((float val) => {
				healthBarImageLowAnim.fillAmount = val;
			});
		}
	}
}
