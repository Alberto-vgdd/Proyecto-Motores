using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskScript : MonoBehaviour {

	private GameObject maskTest;
	private GameObject speedScript;
	private GameObject[] speedBars = new GameObject[6];
	private GameObject dummy;

	void Awake()
	{
		maskTest = GameObject.Find ("SpeedCounter");
		speedScript = GameObject.Find ("SpeedBarsNumbers");
		ObtainSpeedBars ();
	}


	// Use this for initialization
	void Start ()
	{
		DoSomething ();
	}

	void ObtainSpeedBars()
	{
		for (int i = 0; i < speedBars.Length; i++) 
		{
			dummy = GameObject.Find ("Pos " + (i + 1));
			//print (dummy.GetComponent<MaskableGraphic> ().maskable);
			speedBars [i] = dummy;
			//print ("Añadido: " + dummy.name + " , a lista");
			//posiciones [i].GetComponent<SpriteRenderer> ().sprite = verde;
		}
	}


	void DoSomething()
	{
		//print (maskTest.GetComponent<MaskableGraphic>().maskable);	
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
