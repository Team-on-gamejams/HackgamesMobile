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

	[Header("Audio")] [Space]
	[SerializeField] AudioClip[] appearSounds;
	[SerializeField] AudioClip[] dieSounds;
	[SerializeField] AudioClip[] takeDamageSounds;

	[Header("Health")] [Space]
	[SerializeField] float healBarTime = 0.2f;
	[SerializeField] Image healthBarImage;
	[SerializeField] Image healthBarImageLowAnim;
	[SerializeField] Image healthBarImageBack;
	[SerializeField] Image[] bodyPartsImages;

	public List<BodyPart> placedParts = new List<BodyPart>();
	float oneAttackTime = 0.0f;
	float currAttackTimer = 0.0f;

	private void Awake() {
		Stats = new float[(int)StatType.LAST_STAT];

		if (IsPlayer) {
			Stats[(int)StatType.Meat] = PlayerPrefs.GetFloat("Meat", 0);
			Stats[(int)StatType.Dna] = PlayerPrefs.GetFloat("Dna", 0);
		}

		onHpChangeEvent += OnHpChange;

		if(appearSounds != null && appearSounds.Length != 0)
			AudioManager.Instance.Play(appearSounds.Random(), 0.33f, channel: AudioManager.AudioChannel.Sound);
	}

	private void OnDestroy() {
		if (IsPlayer) {
			PlayerPrefs.SetFloat("Meat", Stats[(int)StatType.Meat]);
			PlayerPrefs.SetFloat("Dna", Stats[(int)StatType.Dna]);
		}
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

	public void AddStatBonus(StatType type, float value) {
		Stats[(int)type] += value;
		if(type == StatType.AttackSpeed)
			oneAttackTime = 1.0f / (Stats[(int)StatType.AttackSpeed] != 0 ? Stats[(int)StatType.AttackSpeed] : 1);
		onStatsChangeEvent?.Invoke();
	}

	public void Heal(float heal) {
		if (currHp < Stats[(int)StatType.Hp]) {
			currHp += heal;
			if (currHp > Stats[(int)StatType.Hp])
				currHp = Stats[(int)StatType.Hp];

			healthBarImage.fillAmount = currHp / Stats[(int)StatType.Hp];
			healthBarImageLowAnim.fillAmount = currHp / Stats[(int)StatType.Hp];

			onHpChangeEvent?.Invoke();
		}
	}

	public void InstaCheatAttack() {
		currAttackTimer += oneAttackTime;
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
			if(dieSounds != null && dieSounds.Length != 0) {
				AudioSource die = AudioManager.Instance.Play(dieSounds.Random(), 0.33f, channel: AudioManager.AudioChannel.Sound);
				LeanTween.value(0.33f, 0.0f, 1.5f)
					.setOnUpdate((float v)=> {
						die.volume = v;
					});
			}
			onDie?.Invoke();
		}

		if (takeDamageSounds != null && takeDamageSounds.Length != 0)
			AudioManager.Instance.Play(takeDamageSounds.Random(), 0.2f, channel: AudioManager.AudioChannel.Sound);

		onHpChangeEvent?.Invoke();
	}

	public void RecreateBodyParts(bool isForce = false) {
		foreach (var part in placedParts)
			Destroy(part.gameObject);
		placedParts.Clear();

		placedParts.Add(GetInstantiatedPart(BodyPartType.Body, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Arms, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Legs, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Tail, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Wings, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Fur, isForce));

		placedParts.Add(GetInstantiatedPart(BodyPartType.Head, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Horns, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Teeth, isForce));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Eyes, isForce));

		RecalcStats();
	}

	public void ResetHealth(bool isForce = false) {
		currHp = Stats[(int)StatType.Hp];
		
		if (isForce) {
			healthBarImage.fillAmount = 1.0f;
			healthBarImageLowAnim.fillAmount = 1.0f;
		}

		onHpChangeEvent?.Invoke();
	}

	public void RecalcStats() {
		for (int i = 0; i < Stats.Length; ++i) {
			if(i != (int)StatType.Meat && i != (int)StatType.Dna)
				Stats[i] = 0.0f;
		}

		for (int i = 0; i < placedParts.Count; ++i) {
			if(placedParts[i] == null) continue;
			for (int j = 0; j < placedParts[i].stats.Length; ++j) {
				Stats[j] += placedParts[i].stats[j].value;
			}
		}

		currHp = Stats[(int)StatType.Hp];
		oneAttackTime = 1.0f / (Stats[(int)StatType.AttackSpeed] != 0 ? Stats[(int)StatType.AttackSpeed] : 1);

		onStatsChangeEvent?.Invoke();
		onHpChangeEvent?.Invoke();
	}

	BodyPart GetInstantiatedPart(BodyPartType type, bool isForce = false) {
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

				BodyPart part = Instantiate(partPrefab, pos, Quaternion.identity, parent).GetComponent<BodyPart>();
				part.isForce = isForce;
				part.transform.localEulerAngles = Vector3.zero;

				if (IsPlayer) {
					part.level = PartsManager.instance.GetPlayerLevel(partPrefab);
					part.RecalcStatForLevelBonus();
				}
				else {
					part.level = BattleManager.instance.currLevel / 10;
					part.RecalcStatForLevelBonus();
				}

				if(bodyPartsImages != null && bodyPartsImages.Length > (int)type)
					bodyPartsImages[(int)type].sprite = partPrefab.sr.sprite;
				return part;
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
