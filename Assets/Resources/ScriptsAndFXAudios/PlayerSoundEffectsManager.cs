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

	public float accumulatedEnginePitch = 0.0f;

	private AudioSource engineSound;
	private AudioSource driftSound;
	private PlayerMovement pm;

	private const float DRIFT_SOUND_VOLUME_BASE = 0.2f;
	private const float DRIFT_SOUND_VOLUME_SPEEDSCALING = 0.035f;
	private const float DRIFT_SOUND_PITCH_BASE = 0.8f;
	private const float DRIFT_SOUND_PITCH_DEGREESCALING = 0.025f;
	private const float DRIFT_SOUND_PITCH_MAX = 2f;
	private const float ENGINE_SOUND_VOLUME_BASE = 0.35f;
	private const float ENGINE_SOUND_VOLUME_SPEEDSCALING = 0.075f;
	private const float ENGINE_SOUND_PITCH_BASE = 0.1f;
	private const float ENGINE_SOUND_PITCH_SPEEDSCALING = 0.025f;
	private const float ENGINE_SOUND_PITCH_MAX = 1.5f;

	// Use this for initialization
	void Start ()
	{
		//playerEffects = this.GetComponents<AudioSource> ();
		//print ("numero de audioSources obtenidos: " + playerEffects.Length);
		playerReference = GameObject.FindGameObjectWithTag ("Player");
		pm = playerReference.GetComponent<PlayerMovement> ();

		GetFXFromResources ();

		//this.GetComponents<AudioSource> () [0].clip = m_Engine;
		engineSound = this.GetComponents<AudioSource> () [0];
		engineSound.volume = 0;
		engineSound.pitch = 0;
		engineSound.loop = true;
		engineSound.Play ();


		//this.GetComponents<AudioSource> () [1].clip = m_DriftingSound;
		driftSound = this.GetComponents<AudioSource> () [1];
		driftSound.volume = 0;
		driftSound.pitch = 0;
		driftSound.loop = true;
		driftSound.Play ();

	}

	private void EngineAudio()
	{
		//Primero, aumentaremos el pitch del audio del engine en funcion de la velocidad del jugador.
		engineSound.pitch = Mathf.Clamp(ENGINE_SOUND_PITCH_BASE + Mathf.Abs(playerReference.GetComponent<PlayerMovement>().GetCurrentSpeed()) * ENGINE_SOUND_PITCH_SPEEDSCALING, 0, ENGINE_SOUND_PITCH_MAX);
		engineSound.volume = Mathf.Clamp01(ENGINE_SOUND_VOLUME_BASE + Mathf.Abs(playerReference.GetComponent<PlayerMovement> ().GetCurrentSpeed()) * ENGINE_SOUND_VOLUME_SPEEDSCALING);

		if (pm.IsDrifting ()) {
			driftSound.volume = Mathf.Clamp01 (pm.GetCurrentSpeed () * DRIFT_SOUND_VOLUME_SPEEDSCALING + DRIFT_SOUND_VOLUME_BASE) ;
			driftSound.pitch = Mathf.Clamp (Mathf.Abs (pm.GetDriftDegree () * DRIFT_SOUND_PITCH_DEGREESCALING) + DRIFT_SOUND_PITCH_BASE, 0, DRIFT_SOUND_PITCH_MAX);
		} else {
			driftSound.volume = driftSound.pitch = 0;
		}



	}

	private void GetFXFromResources()
	{
		//m_Engine = Resources.Load ("ScriptsAndFXSounds/car_idle", typeof(AudioClip)) as AudioClip;
		m_Engine = Resources.Load ("ScriptsAndFXSounds/CarEngineMotoCycle", typeof(AudioClip)) as AudioClip;
		m_DriftingSound = Resources.Load ("ScriptsAndFXSounds/DRIFTSound", typeof(AudioClip)) as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
		EngineAudio ();
	}
}
