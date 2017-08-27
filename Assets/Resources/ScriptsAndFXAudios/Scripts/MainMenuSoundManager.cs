using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundManager : MonoBehaviour {

	private static AudioSource currentFXSound;
	private static AudioSource currentSong;

	[Header("0 = Accept, 1 = Cancel, 2 = Notification")]
	public AudioClip[]FXSounds;

	private AudioClip acceptSound;
	private AudioClip cancelSound;
	private AudioClip notificationSound;

	public static MainMenuSoundManager instance;

	void Awake()
	{
		getSoundsFromResources ();
		instance = this;
	}

	// Use this for initialization
	void Start () {

		currentFXSound = this.GetComponents<AudioSource>() [1];
		currentSong = this.GetComponents<AudioSource>() [0];

		//Test de si funciona la referencia estatica
		currentSong.Play ();
		currentSong.volume = 0.5f;

		currentFXSound.loop = false;
		currentFXSound.volume = 0.15f;

	}

	void getSoundsFromResources()
	{
		//acceptSound = Resources.Load ("ScriptsAndFXAudios/FX Sounds/Accept", typeof(AudioClip)) as AudioClip;
		//cancelSound = Resources.Load ("ScriptsAndFXAudios/FX Sounds/Cancel", typeof(AudioClip)) as AudioClip;
		//notificationSound = Resources.Load ("ScriptsAndFXAudios/FX Sounds/Notification", typeof(AudioClip)) as AudioClip;

		acceptSound = FXSounds [0];
		cancelSound = FXSounds [1];
		notificationSound = FXSounds [2];
	}

	public void playAcceptSound()
	{
		//if (currentFXSound.isPlaying) {	return;	}
		currentFXSound.clip = acceptSound;
		currentFXSound.Play ();
	}

	 public void playCancelSound()
	{
		//if (currentFXSound.isPlaying) {	return;	}
		currentFXSound.clip = cancelSound;
		currentFXSound.Play ();
	}
		
	 public void playNotificationSound()
	{
		//if (currentFXSound.isPlaying) {	return;	}
		currentFXSound.clip = notificationSound;
		currentFXSound.Play ();
	}


}
