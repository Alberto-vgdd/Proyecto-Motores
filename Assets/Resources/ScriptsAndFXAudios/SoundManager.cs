﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	private Object[] playList;
	//private AudioSource audioSource;

	private AudioClip actualSong;
	private AudioClip nextSong;

	private float currentSongTime;
	private float songSpaceTime;

	private float fadeOutValue = 0.1f / 5.0f;
	private bool isFadeOutActive;

	public static string[] playListName = 
	{
		"ACE - Adrenaline",
		"ACE - Futureland", 
		"ACE & PAMSY - Inside My Soul",
		"AN-G - Different Girl", 
		"BLISS - Higher Higher More and More",
		"BRONY ft. ODYSSEY - Discord", 
		"FASTWAY - Hyper Super Power",
		"GO2 - Hot Hot Racing Car", 
		"HOTBLADE - Power Two", 
		"IRENE - Coolover", 
		"JAGER - I Won't Fall Apart"
	};

	void Awake()
	{
		//playList = Resources.LoadAll ("EurobeatInstrumentalLowQuality", typeof(AudioClip));
		//song = playList [Random.Range (0, playList.Length)] as AudioClip;
		//if (playList == null) {	print ("Ruta equivocada");		}
		//print ("Tamaño de playlist cargada: " + playList.Length);

		actualSong = GetSongFromPlayList ();
		currentSongTime = 0.0f;
		songSpaceTime = 0.0f;

		nextSong = GetSongFromPlayList ();

		while (actualSong == nextSong) 
		{nextSong = GetSongFromPlayList ();		}

	}
		
	// Use this for initialization
	void Start () 
	{
		//print ("Nombre Cancion a mostrar: " + actualSong.name);
		this.GetComponent<AudioSource> ().clip = actualSong;
		this.GetComponent<AudioSource> ().volume = 0.2f;
		this.GetComponent<AudioSource> ().Play ();
	}

	private AudioClip GetSongFromPlayList()
	{
		return Resources.Load ("EurobeatInstrumentalLowQuality/" + 
			playListName [Random.Range(0, playListName.Length)], typeof(AudioClip)) as AudioClip;
	}



	private void SwitchSongs()
	{
		actualSong = nextSong;
		nextSong = GetSongFromPlayList ();

		if (nextSong == actualSong) 
		{ nextSong = GetSongFromPlayList ();	}

		//print ("Nombre Cancion a mostrar: " + actualSong.name);
		this.GetComponent<AudioSource> ().clip = actualSong;
		this.GetComponent<AudioSource> ().volume = 0.2f;
		this.GetComponent<AudioSource> ().Play ();
	}

	public void SongFadeOutIn5Seconds()
	{	isFadeOutActive = true;	}


	void FixedUpdate()
	{
		//Controlamos el ciclo de canciones.
		currentSongTime += Time.deltaTime;
		if (currentSongTime >= actualSong.length) 
		{
			songSpaceTime += Time.deltaTime;

			if (songSpaceTime >= 1.5f) 
			{
				SwitchSongs ();
				this.GetComponent<AudioSource> ().Play ();
				currentSongTime = 0.0f;
				songSpaceTime = 0.0f;
			}
		}

		//Controlamos si queremos un fadeout de la cancion
		if (isFadeOutActive) 
		{   
			if (this.GetComponent<AudioSource> ().volume <= 0.0f) {
				isFadeOutActive = false;
			} 
			else
			{ this.GetComponent<AudioSource> ().volume -= Time.deltaTime / fadeOutValue; }
				
		}
	}
}