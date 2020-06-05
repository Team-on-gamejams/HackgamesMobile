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
	[SerializeField] TextMeshProUGUI levelTextField;

	Monster enemyMonster;

	bool isInBattle = false;
	int currLevel = 0;

	private void Start() {
		playerMonster.onDie += OnPlayerDie;
		playerMonster.RecreateBodyParts();

		CreateNewEnemy();

		isInBattle = true;
	}

	private void Update() {
		if (isInBattle) {
			playerMonster.RegenerateHealth();
			while (playerMonster.IsCanDoDamage())
				enemyMonster.TakeDamage(playerMonster.DoDamage());

			enemyMonster.RegenerateHealth();
			while (enemyMonster.IsCanDoDamage())
				playerMonster.TakeDamage(enemyMonster.DoDamage());
		}
	}

	public void Continue() {
		isInBattle = true;
	}

	public void Pause() {
		isInBattle = false;
	}

	public void RestartAll() {
		playerMonster.RecalcStats();
		enemyMonster.RecalcStats();
		isInBattle = true;
	}

	void CreateNewEnemy() {
		++currLevel;
		levelTextField.text = currLevel.ToString();

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
		enemyMonster = null;

		CreateNewEnemy();
	}

	void OnPlayerDie() {
		//TODO: respawn player
	}
}
