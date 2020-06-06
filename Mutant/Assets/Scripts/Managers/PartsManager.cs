using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour {
	public Action OnNewPlayerPartsAvaliable;

	[SerializeField] TypedBodyPartsList[] allParts;

	public BodyPart GetRandomPart(BodyPartType type) {
		return allParts[(int)type].parts.Random();
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

	public void InitStartPlayerParts(Monster playerMonster) {
		for (int i = 0; i < allParts.Length; ++i) {
			allParts[i].isOwnedByPlayer[0] = true;
			allParts[i].parts[0].isEquipedByPlayer = true;
			//for (int j = 1; j < allParts[i].parts.Length; ++j) {
			//	allParts[i].isOwnedByPlayer[j] = false;
			//}
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
}

[Serializable]
public class TypedBodyPartsList {
	public BodyPartType type;
	public BodyPart[] parts;
	public bool[] isOwnedByPlayer;
}