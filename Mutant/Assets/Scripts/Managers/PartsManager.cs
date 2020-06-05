using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsManager : MonoBehaviour {
	[SerializeField] TypedBodyPartsList[] allParts;

	public BodyPart GetRandomPart(BodyPartType type) {
		return allParts[(int)type].parts.Random();
	}
}

[Serializable]
public class TypedBodyPartsList {
	public BodyPartType type;
	public BodyPart[] parts;
}