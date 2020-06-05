using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
	public float[] Stats;

	[SerializeField] bool IsPlayer;
	[SerializeField] List<BodyPart> usedBodyParts;

	List<BodyPart> placedParts = new List<BodyPart>();

	private void Start() {
		RecreateBodyParts();
	}

	void RecreateBodyParts() {
		Stats = new float[(int)StatType.LAST_STAT];

		foreach (var part in placedParts)
			Destroy(part.gameObject);
		placedParts.Clear();

		placedParts.Add(GetInstantiatedPart(BodyPartType.Body));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Arms));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Legs));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Tail));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Wings));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Fur));

		placedParts.Add(GetInstantiatedPart(BodyPartType.Head));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Horns));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Teeth));
		placedParts.Add(GetInstantiatedPart(BodyPartType.Eyes));
	}

	BodyPart GetInstantiatedPart(BodyPartType type) {
		for (int i = 0; i < usedBodyParts.Count; ++i) {
			BodyPart partPrefab = usedBodyParts[i];
			if (partPrefab.type == type) {
				Vector3 pos = Vector3.zero;
				Transform parent = transform;

				for (int j = 0; j < placedParts.Count; ++j) {
					for (int k = 0; k < placedParts[j].connectionParts.Length; ++k) {
						if (partPrefab.type == placedParts[j].connectionParts[k]) {
							pos = placedParts[j].connectionPoints[k].position;
							parent = placedParts[j].connectionPoints[k];
							break;
						}
					}
				}

				for(int j = 0; j < partPrefab.stats.Length; ++j) {
					Stats[j] += partPrefab.stats[j].value;
				}

				return Instantiate(partPrefab, pos, Quaternion.identity, parent).GetComponent<BodyPart>();
			}
		}
		return null;
	}
}
