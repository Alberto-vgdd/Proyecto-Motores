using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffectsManager : MonoBehaviour {
 
	public AudioClip m_Engine;         
	public AudioClip m_DriftingSound;

	public GameObject playerSoundEffects;
	public GameObject playerReference;
	//public AudioSource[] playerEffects;
	// el 0 será el engine, y el 1 será el drift.

	public bool driftEnableSound = false;
	public float accumulatedEnginePitch = 0.0f;

	// Use this for initialization
	void Start ()
	{
		//playerEffects = this.GetComponents<AudioSource> ();
		//print ("numero de audioSources obtenidos: " + playerEffects.Length);
		playerReference = GameObject.FindGameObjectWithTag ("Player");

		GetFXFromResources ();

		//this.GetComponents<AudioSource> () [0].clip = m_Engine;
		this.GetComponents<AudioSource> () [0].volume = 0.4f;
		this.GetComponents<AudioSource> () [0].pitch = 0.1f;
		this.GetComponents<AudioSource> () [0].loop = true;

		//this.GetComponents<AudioSource> () [1].clip = m_DriftingSound;
		this.GetComponents<AudioSource> () [1].loop = true;
		this.GetComponents<AudioSource> () [1].volume = 0.2f;


		this.GetComponents<AudioSource> () [0].Play ();


	}

	private void EngineAudio()
	{
		//Primero, aumentaremos el pitch del audio del engine en funcion de la velocidad del jugador.
		this.GetComponents<AudioSource> () [0].pitch = playerReference.GetComponent<PlayerMovement>().accumulatedAcceleration / 45f;


		//Despues, veremos que si está drifteando, sonará el ruido de drifteo.

		if (playerReference.GetComponent<PlayerMovement> ().drifting && !driftEnableSound) 
		{
			driftEnableSound = true;
			this.GetComponents<AudioSource> () [1].Play ();
		}
		if (playerReference.GetComponent<PlayerMovement> ().drifting == false && driftEnableSound) 
		{
			driftEnableSound = false;
			this.GetComponents<AudioSource> () [1].Stop ();
		}
	}

	private void GetFXFromResources()
	{
		m_Engine = Resources.Load ("ScriptsAndFXSounds/car_idle", typeof(AudioClip)) as AudioClip;
		m_DriftingSound = Resources.Load ("ScriptsAndFXSounds/Skid", typeof(AudioClip)) as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
		EngineAudio ();
	}
}
