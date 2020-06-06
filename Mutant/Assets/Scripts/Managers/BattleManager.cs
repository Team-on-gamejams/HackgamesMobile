using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour {
	[SerializeField] Monster playerMonster;
	[SerializeField] PartsManager partsManager;
	[Space]
	[SerializeField] Monster enemyPrefab;
	[SerializeField] Transform enemyPos;
	[Space]
	[SerializeField] TextMeshProUGUI levelTextField1;
	[SerializeField] TextMeshProUGUI levelTextField2;

	Monster enemyMonster;

	int isInBattle = 1;
	int currLevel = 0;

	private void Start() {
		partsManager.InitStartPlayerParts(playerMonster);
		playerMonster.onDie += OnPlayerDie;
		playerMonster.RecreateBodyParts();

		CreateNewEnemy();

		LeanTween.delayedCall(0.33f, Continue);
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
		--isInBattle;
	}

	void CreateNewEnemy() {
		++currLevel;
		levelTextField1.text = levelTextField2.text = currLevel.ToString();

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

	}

	void OnEnemyDie() {
		enemyMonster.onDie -= OnEnemyDie;
		Destroy(enemyMonster.gameObject, 1.5f);
		enemyMonster = null;

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
