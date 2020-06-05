using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType : byte {
	Hp,
	HpRegen,
	Armor,
	Attack,
	AttackSpeed,
	CriticalChance,
	GearLevel,
	LAST_STAT,
}

public enum BodyPartType {
	Body,
	Head,
	Arms,
	Legs,
	Eyes,
	Teeth,
	Wings,
	Horns,
	Tail,
	Fur,
}

public enum SkillType {
	None,
	Evade,
}