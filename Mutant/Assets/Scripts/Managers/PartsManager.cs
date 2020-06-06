using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour {
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
}

[Serializable]
public class TypedBodyPartsList {
	public BodyPartType type;
	public BodyPart[] parts;
	public bool[] isOwnedByPlayer;
	public bool[] playerLevel;	//TODO: level up 
}