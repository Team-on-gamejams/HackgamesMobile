using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClickSound : MonoBehaviour {
	[SerializeField] AudioClip[] sounds;

	private void Awake() {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(OnClick);
	}

	void OnClick() {
		AudioManager.Instance.Play(sounds.Random(), channel: AudioManager.AudioChannel.Sound);
	}
}
