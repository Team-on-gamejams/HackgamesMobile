using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour {
	public BodyPartType type;
	[Space]
	public BodyPartType[] connectionParts;
	public Transform[] connectionPoints;
	[Space]
	public StatValue[] stats;
}

[Serializable]
public class StatValue {
	public StatType type;
	public float value;
}