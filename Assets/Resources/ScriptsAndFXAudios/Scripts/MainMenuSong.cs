using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSong : MonoBehaviour {

	private static AudioSource currentSong;



	// Use this for initialization
	void Awake () {

		currentSong = this.GetComponent<AudioSource>();
		currentSong.loop = true;
		currentSong.playOnAwake = true;

		currentSong.Play ();
		currentSong.volume = 0.3f;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
