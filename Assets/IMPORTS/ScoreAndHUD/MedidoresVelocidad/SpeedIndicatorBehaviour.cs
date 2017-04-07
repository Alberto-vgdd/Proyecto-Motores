using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	void ObtainSpeedCounter()	{	speedCounter = GameObject.Find ("SpeedCounter");	}
	void UpdateSpeedCounter()   {	speedCounter.GetComponent<TextMesh> ().text = velosidah.ToString();	}



	private GameObject dummy;
	private GameObject[] posiciones = new GameObject[6];

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



	[Range(0.0f,10.0f)]
	public float velosidah;

	private float divisionBars;

	void ShowSpeedBars()
	{
		divisionBars = 10.0f / 6.0f; //Si va a vel máxima, mostrará la última barra.
									  //Recorrerá siempre 6 veces, pero si supera lo ocupado,
									  // devolverá el estado de vacio al sprite.
		for (int i = 0; i < 6; i++) 
		{
			if (velosidah - divisionBars > 0.0f) { //Va a esa velocidad 
				if ((i == 0 || i == 1) && (divisionBars * (i + 1) < velosidah)) //tiene que ser verde
				{
					posiciones [i].GetComponent<SpriteRenderer> ().sprite = verde;
				}
				if ((i == 2 || i == 3) && (divisionBars * (i + 1) < velosidah)) //tiene que ser amarillo
				{
					posiciones [i].GetComponent<SpriteRenderer> ().sprite = amarillo;
				}
				if ((i == 4 || i == 5) && (divisionBars * (i + 1) < velosidah)) //tiene que ser rojo
				{
					posiciones [i].GetComponent<SpriteRenderer> ().sprite = rojo;
				}
				if (divisionBars * (i + 1) > velosidah) //No va a esa velocidad
				{
					posiciones [i].GetComponent<SpriteRenderer> ().sprite = vacio;
				}
			}
		}
	}


	void Awake() //Establecemos posiciones para el placeholder (vacio)
	{
		ObtainSpeedBars ();
		ObtainSpeedCounter ();
	}

	void Update () 
	{
		ShowSpeedBars ();
		UpdateSpeedCounter ();
	}
}
