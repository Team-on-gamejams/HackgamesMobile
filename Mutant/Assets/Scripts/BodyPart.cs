using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour {
	public BodyPartType type;
	[System.NonSerialized] public bool isEquipedByPlayer;
	[System.NonSerialized] public bool isForce = false;

	[Header("Card info")][Space]
	public string gameName;
	public string gameDescription;
	
	[Header("Stats")][Space]
	public StatValue[] stats;

	[Header("Connectors")]
	[Space]
	public BodyPartType[] connectionParts;
	public Transform[] connectionPoints;

	[Header("Refs")][Space]
	[SerializeField] Rigidbody2D rigidbody;
	[SerializeField] Rigidbody2D rigidbody2;
	public SpriteRenderer sr;
	[SerializeField] SpriteRenderer sr2;

	private void Start() {
		if(rigidbody != null)
			rigidbody.simulated = false;
		if(rigidbody2 != null)
			rigidbody2.simulated = false;

		if (!isForce) {
			Color c = sr.color;
			c.a = 0.0f;
			sr.color = c;
			if (sr2 != null)
				sr2.color = c;
			LeanTween.value(gameObject, 0.0f, 1.0f, 0.33f)
				.setOnUpdate((float t) => {
					c = sr.color;
					c.a = t;
					sr.color = c;
					if (sr2 != null)
						sr2.color = c;
				});
		}
		else {
			Color c = sr.color;
			c.a = 0.5f;
			sr.color = c;
			if (sr2 != null)
				sr2.color = c;
			LeanTween.value(gameObject, 0.5f, 1.0f, 0.22f)
				.setOnUpdate((float t) => {
					c = sr.color;
					c.a = t;
					sr.color = c;
					if (sr2 != null)
						sr2.color = c;
				});
		}
	}

	public void OnDie() {
		if(rigidbody != null) {
			rigidbody.simulated = true;
			rigidbody.AddForce(new Vector2(Random.Range(-500, 500), Random.Range(0, 750)), ForceMode2D.Impulse);
			rigidbody.AddTorque(Random.Range(-5, 5), ForceMode2D.Impulse);
		}

		if (rigidbody2 != null) {
			rigidbody2.transform.SetParent(transform.parent);
			rigidbody2.simulated = true;
			rigidbody2.AddForce(new Vector2(Random.Range(-500, 500), Random.Range(0, 750)), ForceMode2D.Impulse);
			rigidbody2.AddTorque(Random.Range(-5, 5), ForceMode2D.Impulse);
		}

		LeanTween.value(gameObject, 1.0f, 0.0f, 1.5f)
			.setOnUpdate((float t) => {
				Color c = sr.color;
				c.a = t;
				sr.color = c;
				if (sr2 != null)
					sr2.color = c;
			});
	}
}

[System.Serializable]
public class StatValue {
	public StatType type;
	public float value;
}