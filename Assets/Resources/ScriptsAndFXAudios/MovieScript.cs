using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MovieScript : MonoBehaviour {

	//private GameObject movieQuad;
	//private AudioSource soundMovieQuad;
	//int vsyncprevious;

	public VideoSource videoTrailer;
	public float timeWaiting = 0.0f;

	private bool videoFadeOutActive = false;
	private bool videoFadeInActive = false;
	private bool WaitingForVideo = true;
	private bool FromVideoToMenu = false;

	void Awake()
	{
		this.GetComponent<VideoPlayer>().targetCameraAlpha = 1f;
		this.GetComponent<VideoPlayer> ().Stop ();

	}

	// Use this for initialization
	void Start () 
	{
		//Esto es para el VideoPlayer
		//Acuerdate de meter en el GameObject el trailer.
		SetMovieProperties();

	}

	void SetMovieProperties()
	{
		this.GetComponent<VideoPlayer>().targetCameraAlpha = 0f;
		this.GetComponent<VideoPlayer> ().isLooping = false;
		this.GetComponent<VideoPlayer> ().playOnAwake = false;

		this.GetComponent<AudioSource> ().volume = 0.5f;
		this.GetComponent<AudioSource> ().loop = false;

		this.GetComponent<RawImage> ().raycastTarget = false;
		this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		videoFadeOutActive = true;
		GameObject.Find ("PressAnyKey").GetComponent<RawImage> ().enabled = false;
	}

	void MovieEventSystem()
	{
		if (Input.anyKey) 
		{
			//videoFadeInActive = true; 
			this.GetComponent<RawImage> ().raycastTarget = false;
			this.GetComponent<VideoPlayer> ().Stop ();
			timeWaiting = 0.0f;
			WaitingForVideo = true;
			 
			GameObject.Find ("Menu").GetComponent<Canvas> ().enabled = true;
			GameObject.Find ("PressAnyKey").GetComponent<RawImage> ().enabled = false;
			GameObject.Find ("PressAnyKey").GetComponent<Animation> ().Stop ();
		}	
			
		if (timeWaiting >= 7.0f && WaitingForVideo) //Iniciamos video
		{			
			//videoFadeInActive = true; 
			this.GetComponent<RawImage> ().raycastTarget = true;

			if (!this.GetComponent<VideoPlayer> ().isPlaying) {
				this.GetComponent<VideoPlayer> ().targetCameraAlpha = 1f;
				this.GetComponent<VideoPlayer> ().Play ();
				//videoFadeOutActive = true;
				WaitingForVideo = false;

				GameObject.Find ("Menu").GetComponent<Canvas> ().enabled = false;
				GameObject.Find ("PressAnyKey").GetComponent<RawImage> ().enabled = true;
				GameObject.Find ("PressAnyKey").GetComponent<Animation> ().Play ();

			} 
		}

		if (timeWaiting - 6.0f >= this.GetComponent<VideoPlayer> ().clip.length ) 
		{
			//videoFadeInActive = true;
			this.GetComponent<VideoPlayer> ().Stop();
			//this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
			//videoFadeOutActive = true;
			this.GetComponent<RawImage> ().raycastTarget = false;
			timeWaiting = -3.0f;
			WaitingForVideo = true;
			//print ("Llegamos final video");

			GameObject.Find ("Menu").GetComponent<Canvas> ().enabled = true;
			GameObject.Find ("PressAnyKey").GetComponent<RawImage> ().enabled = false;
			GameObject.Find ("PressAnyKey").GetComponent<Animation> ().Stop ();

		}
	}
		
	void FadeOut()
	{
		if (this.GetComponent<RawImage> ().color.a > 0f) 
		{
			this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f,
				this.GetComponent<RawImage>().color.a - (Time.deltaTime));			
		} 
		else
		{
			videoFadeOutActive = false;
			this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
		}
	}

	void FadeIn()
	{
		if (this.GetComponent<RawImage> ().color.a < 1f) 
		{
			this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f,
				this.GetComponent<RawImage>().color.a + (Time.deltaTime));			
		} 
		else
		{
			videoFadeInActive = false;
			this.GetComponent<RawImage> ().color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		}
	}


	// Update is called once per frame
	void Update () 
	{
		timeWaiting += Time.deltaTime;

		MovieEventSystem ();

		if (videoFadeOutActive) 
		{
			FadeOut ();
		}

		if (videoFadeInActive) 
		{
			FadeIn ();
		}


	}
}
