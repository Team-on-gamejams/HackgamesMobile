using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteColor : MonoBehaviour {
	void Start() {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
	}
}
