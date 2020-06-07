using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour {
	public static PartsManager instance;

	public Action OnNewPlayerPartsAvaliable;

	[SerializeField] TypedBodyPartsList[] allParts;

	private void Awake() {
		instance = this;
	}

	private void OnDestroy() {
		SavePlayerData();
	}

	public BodyPart GetRandomPart(BodyPartType type) {
		return allParts[(int)type].parts.Random();
	}

	public BodyPart[] GetAllParts(BodyPartType type) {
		return allParts[(int)type].parts;
	}

	public BodyPart[] GetAllOwnedByPlayer(BodyPartType type) {
		List<BodyPart> parts = new List<BodyPart>();

		for(int i = 0; i < allParts[(int)type].parts.Length; ++i)
			if (allParts[(int)type].isOwnedByPlayer[i])
				parts.Add(allParts[(int)type].parts[i]);

		return parts.ToArray();
	}

	public BodyPart GetUsedByPlayer(BodyPartType type) {
		for (int i = 0; i < allParts[(int)type].parts.Length; ++i)
			if (allParts[(int)type].parts[i].isEquipedByPlayer)
				return allParts[(int)type].parts[i];
		return null;
	}

	public bool IsHavePlayerSave() {
		return PlayerPrefs.HasKey("Meat");
	}

	public void LoadPlayerSave(Monster playerMonster) {
		Debug.Log("Load save");
		for (int i = 0; i < allParts.Length; ++i) {
			allParts[i].playerlevel = PlayerPrefsX.GetIntArray($"PartsLevels_{allParts[i].type}");
			allParts[i].isOwnedByPlayer = PlayerPrefsX.GetBoolArray($"IsOwnedByPlayer_{allParts[i].type}");
		}

		LoadBodyPart(BodyPartType.Body);

		LoadBodyPart(BodyPartType.Arms);
		LoadBodyPart(BodyPartType.Legs);
		LoadBodyPart(BodyPartType.Tail);
		LoadBodyPart(BodyPartType.Wings);
		LoadBodyPart(BodyPartType.Fur);

		LoadBodyPart(BodyPartType.Head);
		LoadBodyPart(BodyPartType.Teeth);
		LoadBodyPart(BodyPartType.Eyes);
		LoadBodyPart(BodyPartType.Horns);

		void LoadBodyPart(BodyPartType type) {
			int id = PlayerPrefs.GetInt($"EquipedId_{type}");
			allParts[(int)type].parts[id].isEquipedByPlayer = true; 
			playerMonster.usedBodyParts.Add(GetAllParts(type)[id]);
		}

		OnNewPlayerPartsAvaliable?.Invoke();
	}

	public void SavePlayerData() {
		Debug.Log("Save game");
		for (int i = 0; i < allParts.Length; ++i) {
			PlayerPrefsX.SetIntArray($"PartsLevels_{allParts[i].type}", allParts[i].playerlevel);
			PlayerPrefsX.SetBoolArray($"IsOwnedByPlayer_{allParts[i].type}", allParts[i].isOwnedByPlayer);

			for(int j = 0; j < allParts[i].parts.Length; ++j) {
				if (allParts[i].parts[j].isEquipedByPlayer) {
					PlayerPrefs.SetInt($"EquipedId_{allParts[i].type}", j);
					break;
				}
			}
		}
	}

	public void InitStartPlayerParts(Monster playerMonster) {
		Debug.Log("Start new game");
		for (int i = 0; i < allParts.Length; ++i) {
			allParts[i].isOwnedByPlayer[0] = true;
			allParts[i].parts[0].isEquipedByPlayer = true;

			if(i != 0) {
				for (int j = 1; j < allParts[i].parts.Length; ++j) {
					allParts[i].isOwnedByPlayer[j] = false;
				}
			}
		}

		for (int i = 0; i < allParts.Length; ++i) {
			allParts[i].playerlevel = new int[allParts[i].parts.Length];
		}

		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Body)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Arms)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Legs)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Tail)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Wings)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Fur)[0]);

		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Head)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Teeth)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Eyes)[0]);
		playerMonster.usedBodyParts.Add(GetAllOwnedByPlayer(BodyPartType.Horns)[0]);

		OnNewPlayerPartsAvaliable?.Invoke();
	}

	public int GetPlayerLevel(BodyPart partToFind) {
		for(int i = 0; i < allParts[(int)partToFind.type].playerlevel.Length; ++i) {
			if(partToFind == allParts[(int)partToFind.type].parts[i]) {
				return allParts[(int)partToFind.type].playerlevel[i];
			}
		}


		return 2;
	}

	public int GetUpgradePrice(BodyPart partToFind) {
		for (int i = 0; i < allParts[(int)partToFind.type].playerlevel.Length; ++i) {
			if (partToFind == allParts[(int)partToFind.type].parts[i]) {
				float levelUpPrice = allParts[(int)partToFind.type].parts[i].levelUpPrice;
				return Mathf.RoundToInt(levelUpPrice + levelUpPrice * allParts[(int)partToFind.type].parts[i].levelUpPriceGrow * allParts[(int)partToFind.type].playerlevel[i]);
			}
		}

		return 100;
	}

	public void LevelUpPart(BodyPart partToFind) {
		for (int i = 0; i < allParts[(int)partToFind.type].playerlevel.Length; ++i) {
			if (partToFind == allParts[(int)partToFind.type].parts[i]) {
				++allParts[(int)partToFind.type].playerlevel[i];
			}
		}
	}
}

[Serializable]
public class TypedBodyPartsList {
	public BodyPartType type;
	public BodyPart[] parts;
	public bool[] isOwnedByPlayer;
	public int[] playerlevel;
}