using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour {

	// Version mejorada de las propiedades del nodo, demomento no hace nada con el desplazamiento vertical, pero es facilmente implementable.
	// Importante: Mantener coherencia con los indices de las decoraciones, todos deben usar el mismo indice para cada tipo de decoracion, 
	// por ejemplo: 0 para la valla publicitaria, 1 para el muro de cemento, 2 para el quitamiedos, 3 para el muro de plantas.

	[Header("Displacement Parameters")]
	public float dispLateral;									// Desplazamiento lateral relativo
	public float dispFrontal;									// Desplazamiento frontal relativo
	public float dispVertical;									// Desplazamiento vertical relativo
	public float dispAngular;									// Desplazamiento angular relativo

	[Header("References")]
	public List<GameObject> envLeftWall;						// Posibles decoraciones de muro izquierdo
	public List<GameObject> envRightWall;						// Posibles decoraciones de muro derecho
	public List<GameObject> envLeftGround;						// Posibles decoraciones de suelo izquierdo
	public List<GameObject> envRightGround;						// Posibles decoraciones de suelo derecho
	public List<Light> envLights;								// Luces ambientales de esta pieza
	public GameObject checkPointTrigger;						// Trigger del checkpoint
	public GameObject checkPointVisualParent;					// Parte visual del checkpoint

	[Header("Other Params")]
	public float nodeWeight;									// Peso o dificultad del nodo, determina la contribucion a tiempo extendido del nodo.
	private int timeAwarded = 0;								// Tiempo extra que dara este nodo si es un punto de control activo.
	private int nodeID;											// ID del nodo (orden en el que se ha creado)

	// Enciende o apaga las luces de este nodo, funcion llamada por defecto desde StageData o RoadGenerator al crear la pieza.

	public void SetLightState(bool state)
	{
		for (int i = 0; i < envLights.Count; i++) {
			envLights [i].gameObject.SetActive (state);
			envLights [i].enabled = state;
		}
	}

	// Modifica la escala de las luces, utilizado al crear esta pieza desde RoadGenerator.

	public void SetLighScale(float scale)
	{
		for (int i = 0; i < envLights.Count; i++) {
			envLights [i].range *= scale;
		}
	}

	// Prepara la decoracion ambiental, IMPORTANTE, debe llamarse SOLO una vez para que funcione correctamente.

	public void SetEnvoirment(int LWall, int RWall, int LGround, int RGround)
	{
		if (LWall < envLeftWall.Count) {
			envLeftWall [LWall].SetActive (true);
		}
		if (RWall < envRightWall.Count) {
			envRightWall [RWall].SetActive (true);
		}
		if (LGround < envLeftGround.Count) {
			envLeftGround [LGround].SetActive (true);
		}
		if (RGround < envRightGround.Count) {
			envRightGround [RGround].SetActive (true);
		}
	}

	// Prepara este nodo para que sea un punto de control activo.

	public void SetAsActiveCheckpoint(int _timeAwarded)
	{
		timeAwarded = _timeAwarded;
		checkPointVisualParent.SetActive (true);
		checkPointTrigger.tag = "CP_Active";
		checkPointTrigger.SetActive (true);
	}

	// Prepara este punto de control para que cuente como direccion contraria.

	public void SetAsWrongWay()
	{
		checkPointTrigger.tag = "CP_WrongWay";
		checkPointTrigger.SetActive (true);
	}

	// Desactiva este punto de control una vez cruzado.

	public void CrossCheckPoint()
	{
		checkPointTrigger.SetActive (false);
	}

	// Getters/Setters

	public int GetID()
	{
		return nodeID;
	}
	public void SetID(int arg)
	{
		nodeID = arg;
	}
	public int GetTimeAwarded()
	{
		return timeAwarded;
	}
}
