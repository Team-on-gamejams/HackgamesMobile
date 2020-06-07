using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class BattleManager : MonoBehaviour {
	public System.Action<Monster> onEnemySpawnEvent;

	[SerializeField] Monster playerMonster;
	[SerializeField] PartsManager partsManager;
	[Space]
	[SerializeField] ScreenManager screenManager;
	[SerializeField] Monster enemyPrefab;
	[SerializeField] Transform enemyPos;
	[SerializeField] Transform enemyBottomPos;
	[SerializeField] Sprite meatSprite;
	[SerializeField] Transform meatCollectorPos;
	[SerializeField] Canvas canvas;
	[Space]
	[SerializeField] TextMeshProUGUI levelTextField1;
	[SerializeField] AudioClip counterSound;

	[Header("Balance")] [Space]
	[SerializeField] float baseMeat = 10;
	[SerializeField] float mathForLevelMin = 0.5f;
	[SerializeField] float mathForLevelMax = 2;
	[SerializeField] float meatIdleMinute = 1;
	[SerializeField] float meatIdlePerLevel = 0.2f;
	float idleMeat = 0;

	Monster enemyMonster;

	int isInBattle = 1;
	int currLevel = 0;

	private void Start() {
		if (partsManager.IsHavePlayerSave()) {
			partsManager.LoadPlayerSave(playerMonster);
		}
		else {
			partsManager.InitStartPlayerParts(playerMonster);
		}
		playerMonster.onDie += OnPlayerDie;
		playerMonster.RecreateBodyParts();

		CreateNewEnemy();

		currLevel = PlayerPrefs.GetInt("CurrBattleLevel", 0);
		levelTextField1.text = currLevel.ToString();

		if (PlayerPrefs.HasKey("Meat")) {
			long lastTicks = PlayerPrefsX.GetLong("TicksLastExit", 0);
			float minsAfk = (System.DateTime.Now - new System.DateTime(lastTicks)).Minutes;
			if (minsAfk <= 1)
				minsAfk = 1;
			idleMeat = minsAfk * (meatIdleMinute + meatIdlePerLevel * currLevel);
			if (idleMeat <= 1)
				idleMeat = 1;
			screenManager.ShowIldeWindow(idleMeat);
		}
		else {
			screenManager.HideIdleWIndow();
			LeanTween.delayedCall(0.33f, Continue);
		}
	}

	private void OnDestroy() {
		PlayerPrefs.SetInt("CurrBattleLevel", currLevel);
		PlayerPrefsX.SetLong("TicksLastExit", System.DateTime.Now.Ticks);
	}

	public void DropMeatForIdle() {
		int droppedMeat = Mathf.RoundToInt(idleMeat);
		int meatPieces = droppedMeat / 10 + (droppedMeat % 10 != 0 ? 1 : 0);
		AudioSource counteras = null;

		while (meatPieces != 0) {
			--meatPieces;

			GameObject meatgo = new GameObject("Flying meat");
			meatgo.transform.SetParent(canvas.transform);
			Image img = meatgo.AddComponent<Image>();
			img.sprite = meatSprite;
			Color c = img.color;
			c.a = 0.0f;
			img.color = c;
			img.SetNativeSize();

			img.rectTransform.position = GameManager.Instance.Camera.WorldToScreenPoint((Vector3)Random.insideUnitCircle);
			meatgo.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));

			LeanTween.value(0, 1, 0.5f)
				.setOnUpdate((float a) => {
					c = img.color;
					c.a = a;
					img.color = c;
				})
				.setOnComplete(() => {
					Vector3 startPos = meatgo.transform.position;
					Vector3 startAngle = meatgo.transform.localEulerAngles;
					float dist = (meatCollectorPos.position - startPos).magnitude;

					LeanTween.value(0, 1, dist / Screen.height * 0.8f)
					.setDelay(0.1f * meatPieces)
					.setOnUpdate((float t) => {
						meatgo.transform.position = Vector3.Lerp(startPos, meatCollectorPos.position, t);
						meatgo.transform.localEulerAngles = Vector3.Lerp(startAngle, Vector3.zero, t);
					});

					bool isLastLoop = meatPieces == 0;
					LeanTween.value(1, 0, 0.2f)
					.setDelay(0.1f * meatPieces + dist / Screen.height * 0.64f)
					.setOnStart(() => {
						if (counteras == null) {
							counteras = AudioManager.Instance.Play(counterSound, channel: AudioManager.AudioChannel.Sound);
						}
					})
					.setOnUpdate((float t) => {
						c = img.color;
						c.a = t;
						img.color = c;
						meatgo.transform.localScale = Vector3.one * t;
					})
					.setEase(LeanTweenType.easeInCubic)
					.setOnComplete(() => {
						if (droppedMeat >= 10) {
							playerMonster.AddStatBonus(StatType.Meat, 10);
							droppedMeat -= 10;
						}
						else {
							playerMonster.AddStatBonus(StatType.Meat, droppedMeat % 10);
							droppedMeat = 0;
						}

						if (counteras != null && isLastLoop) {
							counteras.Stop();
							counteras = null;
						}
					});
				});
		}
	}

	private void Update() {
		if (isInBattle == 0) {
			playerMonster.RegenerateHealth();

			if (enemyMonster != null) {
				while (playerMonster.IsCanDoDamage()) {
					float dmg = playerMonster.DoDamage();
					if(enemyMonster != null)
						enemyMonster.TakeDamage(dmg);
				}

				if (enemyMonster != null) {
					enemyMonster.RegenerateHealth();
					while (enemyMonster.IsCanDoDamage())
						playerMonster.TakeDamage(enemyMonster.DoDamage());
				}
			}
		}
	}

	public void Continue() {
		--isInBattle;
	}

	public void Pause() {
		++isInBattle;
	}

	public void RestartAll() {
		playerMonster.RecalcStats();
		enemyMonster.RecalcStats();
		playerMonster.ResetHealth(true);
		enemyMonster.ResetHealth(true);
		--isInBattle;
	}

	void CreateNewEnemy() {
		++currLevel;
		levelTextField1.text = currLevel.ToString();

		enemyMonster = Instantiate(enemyPrefab, enemyPos.position, Quaternion.identity, enemyPos);
		enemyMonster.onDie += OnEnemyDie;

		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Body));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Arms));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Legs));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Tail));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Wings));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Fur));

		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Head));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Teeth));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Eyes));
		enemyMonster.usedBodyParts.Add(partsManager.GetRandomPart(BodyPartType.Horns));
		enemyMonster.RecreateBodyParts();

		onEnemySpawnEvent?.Invoke(enemyMonster);
	}

	void OnEnemyDie() {
		enemyMonster.onDie -= OnEnemyDie;
		Destroy(enemyMonster.gameObject, 1.5f);
		enemyMonster = null;

		int droppedMeat = Mathf.RoundToInt(baseMeat + Random.Range(mathForLevelMin * currLevel, mathForLevelMax * currLevel));
		int meatPieces = droppedMeat / 10 + (droppedMeat % 10 != 0? 1 : 0);

		AudioSource counteras = null;

		while(meatPieces != 0) {
			--meatPieces;

			GameObject meatgo = new GameObject("Flying meat");
			meatgo.transform.SetParent(canvas.transform);
			Image img = meatgo.AddComponent<Image>();
			img.sprite = meatSprite;
			Color c = img.color;
			c.a = 0.0f;
			img.color = c;
			img.SetNativeSize();

			meatgo.transform.position = GameManager.Instance.Camera.WorldToScreenPoint(enemyBottomPos.position + (Vector3)Random.insideUnitCircle);
			meatgo.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));

			LeanTween.value(0, 1, 0.5f)
				.setOnUpdate((float a) => {
					c = img.color;
					c.a = a;
					img.color = c;
				})
				.setOnComplete(() => {
					Vector3 startPos = meatgo.transform.position;
					Vector3 startAngle = meatgo.transform.localEulerAngles;
					float dist = (meatCollectorPos.position - startPos).magnitude;

					LeanTween.value(0, 1, dist / Screen.height * 0.8f)
					.setDelay(0.1f * meatPieces)
					.setOnUpdate((float t) => {
						meatgo.transform.position = Vector3.Lerp(startPos,meatCollectorPos.position, t);
						meatgo.transform.localEulerAngles = Vector3.Lerp(startAngle, Vector3.zero, t);
					});

					bool isLastLoop = meatPieces == 0;
					LeanTween.value(1, 0, 0.2f)
					.setDelay(0.1f * meatPieces + dist / Screen.height * 0.64f)
					.setOnStart(()=> { 
						if(counteras == null) {
							counteras = AudioManager.Instance.Play(counterSound, channel: AudioManager.AudioChannel.Sound);
						}
					})
					.setOnUpdate((float t) => {
						c = img.color;
						c.a = t;
						img.color = c;
						meatgo.transform.localScale = Vector3.one * t;
					})
					.setEase(LeanTweenType.easeInCubic)
					.setOnComplete(()=> {
						if (droppedMeat >= 10) {
							playerMonster.AddStatBonus(StatType.Meat, 10);
							droppedMeat -= 10;
						}
						else {
							playerMonster.AddStatBonus(StatType.Meat, droppedMeat % 10);
							droppedMeat = 0;
						}

						if(counteras != null && isLastLoop) {
							counteras.Stop();
							counteras = null;
						}
					});
				});
		}

		playerMonster.ResetHealth();
		LeanTween.delayedCall(1.5f, CreateNewEnemy);
	}

	void OnPlayerDie() {
		--isInBattle;
		enemyMonster.RecalcStats();

		LeanTween.delayedCall(3.0f, () => {
			playerMonster.RecreateBodyParts();
			LeanTween.delayedCall(1.1f, () => {
				++isInBattle;
			});
		});
	}
}
