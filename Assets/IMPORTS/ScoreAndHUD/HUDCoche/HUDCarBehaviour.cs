using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCarBehaviour : MonoBehaviour {


	//Este script irá en el gameobject padre de todo el HUD.

	private GameObject line1; //Linea superior de texto
	private GameObject line2; //Linea inferior de texto

	public GameObject score; //El scoreManager de la escena

	public string line1Text; //Texto que escribiremos en line1
	public string line2Text; //Texto que escribiremos en line2

	public GameObject cocheObjetivo;


	// Use this for initialization
	void Start () 
	{
		line1 = GameObject.Find ("Line1");
		//line1.GetComponent<TextMesh>().text = "Sampel tecst"; //Texto de la izq del HUD
		//Debug.Log ("Oie, hemos sustituido en linea 1");

		line2 = GameObject.Find ("Line2");
		//line2.GetComponent<TextMesh>().text = "Sampel tecst"; //Texto de la izq del HUD
		//Debug.Log("Oie, hemos sustituido en linea 2");

		score = GameObject.Find ("ScoreDummy");
		cocheObjetivo = GameObject.FindGameObjectWithTag ("Player");

	}
		

	void showActualScore()
	{
////		if (cocheObjetivo.GetComponent<PlayerMovement> ().drifting) //Está drifteando
////		{
//		line1Text =
//			("Drifting: " + cocheObjetivo.GetComponent<PlayerMovement>().driftDegree + "degrees" + 
//		score.GetComponent<ScoreManager>().getLocalDriftDistance() + "m"  	);
////
////		}



	}


	void TextManager()
	{



	}



	// Update is called once per frame
	void Update () {		}
}
