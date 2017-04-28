using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedIndicatorBehaviour : MonoBehaviour {

	//Primero, obtenemos las imágenes.
	//Después, podemos hacer lo siguiente:
	// 1 - Cargamos las imágenes en una lista, y cuando vaya a mostrarse, 
	//  activamos el elemento, de forma que se muestra.
	//2 - Se establecen las coordenadas según su posición relativa,
	// cambiando solo el alfa en el momento preciso

	public Sprite vacio;
	public Sprite verde;
	public Sprite amarillo;
	public Sprite rojo;


	//Esto no cambia la velocidad, solo muestra en el texto la actual.
	private GameObject speedCounter;
	private GameObject speedCounterOutline;

	private GameObject player;
	private float maxSpeed;

	public int velosidah;

	private float divisionBars;

	private GameObject dummy;
	private GameObject[] posiciones = new GameObject[6];

	void ObtainSpeedCounters()	
	{	
        speedCounter = this.gameObject;
		speedCounterOutline = GameObject.Find ("SpeedCounterOutline");
	}
	void UpdateSpeedCounter()   
	{	
        velosidah = (int) (player.GetComponent<PlayerMovement> ().accumulatedAcceleration * 5f);
		speedCounterOutline.GetComponent<Text> ().text = velosidah.ToString();
		speedCounter.GetComponent<Text> ().text = velosidah.ToString();
	}
		
	void ObtainSpeedBars()
	{
		for (int i = 0; i < posiciones.Length; i++) 
		{
			dummy = GameObject.Find ("Pos " + (i + 1));
			posiciones [i] = dummy;
			//print ("Añadido: " + dummy.name + " , a lista");
			//posiciones [i].GetComponent<SpriteRenderer> ().sprite = verde;
		}
	}

	void ShowSpeedBars()
	{
		divisionBars = maxSpeed *0.9f/ 6.0f; //Si va a vel máxima, mostrará la última barra.
									  //Recorrerá siempre 6 veces, pero si supera lo ocupado,
									  // devolverá el estado de vacio al sprite.
		for (int i = 0; i < 6; i++) 
		{
			if (Mathf.Abs(velosidah - divisionBars) >= 0.0f) { //Va a esa velocidad 
				if ((i == 0 || i == 1) && (divisionBars * (i + 1) < velosidah)) //tiene que ser verde
				{
					posiciones [i].GetComponent<Image> ().sprite = verde;
				}
				if ((i == 2 || i == 3) && (divisionBars * (i + 1) < velosidah)) //tiene que ser amarillo
				{
					posiciones [i].GetComponent<Image> ().sprite = amarillo;
				}
				if ((i == 4 || i == 5) && (divisionBars * (i + 1) < velosidah)) //tiene que ser rojo
				{
					posiciones [i].GetComponent<Image> ().sprite = rojo;
				}
				if (divisionBars * (i + 1) > velosidah || velosidah == 0) //No va a esa velocidad
				{
					posiciones [i].GetComponent<Image> ().sprite = vacio;
				}
			}
		}
	}

	void Awake() //Establecemos posiciones para el placeholder (vacio)
	{
		ObtainSpeedBars ();
		ObtainSpeedCounters ();
		player = GameObject.FindGameObjectWithTag ("Player");

		//Por si la velocidad hacia atrás es mayor que hacia delante.
		float dummy = player.GetComponent<PlayerMovement> ().maxBwdSpeed * 5f; 
		if (player.GetComponent<PlayerMovement> ().maxFwdSpeed > dummy) 
		{
			maxSpeed = player.GetComponent<PlayerMovement> ().maxFwdSpeed * 5f;
		} 
		else
		{
			maxSpeed = dummy;
		}
	}

	void Update () 
	{
		ShowSpeedBars ();
		UpdateSpeedCounter ();
	}
		
}
