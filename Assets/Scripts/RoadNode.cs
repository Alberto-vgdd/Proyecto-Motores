using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour {

	// Version mejorada de las propiedades del nodo, demomento no hace nada con el desplazamiento vertical, pero es facilmente implementable.
	// Importante: Mantener coherencia con los indices de las decoraciones, todos deben usar el mismo indice para cada tipo de decoracion, 
	// por ejemplo: 0 para la valla publicitaria, 1 para el muro de cemento, 2 para el quitamiedos, 3 para el muro de plantas.

	[Header("Unique ID")]
	public int roadPieceID;
	[Header("Displacement Parameters")]
	public float dispLateral;									// Desplazamiento lateral relativo
	public float dispFrontal;									// Desplazamiento frontal relativo
	public float dispVertical;									// Desplazamiento vertical relativo
	public float dispAngularHorizontal;							// Desplazamiento angular horizontal relativo
	public float dispAngularVertical;							// Desplazamiento angular vertical relativo

	[Header("References")]
	public List<GameObject> envLeftWall;						// Posibles decoraciones de muro izquierdo
	public List<GameObject> envRightWall;						// Posibles decoraciones de muro derecho
	public List<GameObject> envLeftGround;						// Posibles decoraciones de suelo izquierdo
	public List<GameObject> envRightGround;						// Posibles decoraciones de suelo derecho
	public List<GameObject> envTunnel;							// Posibles variantes de tunel. 
	public List<Light> envLights;								// Luces ambientales de esta pieza
	public GameObject checkPointTrigger;						// Trigger del checkpoint
	public GameObject CP_VisualTunnel;							// Parte visual del checkpoint si es tunel
	public GameObject CP_VisualNormal;							// Parte visual del checkpoint

	[Header("Other Params")]
	public float lightRange;
	public float nodeWeight;									// Peso o dificultad del nodo, determina la contribucion a tiempo extendido del nodo.
	private float timeAwarded = 0;								// Tiempo extra que dara este nodo si es un punto de control activo.
	private int nodeID;											// ID del nodo (orden en el que se ha creado)
	private bool isTunnel = false;

	// Enciende o apaga las luces de este nodo, funcion llamada por defecto desde StageData o RoadGenerator al crear la pieza.

	public void SetLightState(bool state)
	{
		if (isTunnel)
			state = false;
		for (int i = 0; i < envLights.Count; i++) {
			envLights [i].gameObject.SetActive (state);
			envLights [i].enabled = state;
		}
	}

	// Modifica la escala de las luces, utilizado al crear esta pieza desde RoadGenerator.

	public void SetLighScale(float scale)
	{
		for (int i = 0; i < envLights.Count; i++) {
			envLights [i].range = lightRange * scale;
		}
	}

	// Prepara la decoracion ambiental

	public void SetEnvironment(int LWall, int RWall, int LGround, int RGround, bool Tunnel)
	{
		for (int i = 0; i < envLeftWall.Count; i++) {
			if (i == LWall) {
				envLeftWall [i].SetActive (true);
			} else {
				envLeftWall [i].SetActive (false);
			}
		}
		for (int i = 0; i < envRightWall.Count; i++) {
			if (i == RWall) {
				envRightWall [i].SetActive (true);
			} else {
				envRightWall [i].SetActive (false);
			}
		}
		for (int i = 0; i < envLeftGround.Count; i++) {
			if (i == LGround) {
				envLeftGround [i].SetActive (true);
			} else {
				envLeftGround [i].SetActive (false);
			}
		}
		for (int i = 0; i < envRightGround.Count; i++) {
			if (i == RGround) {
				envRightGround [i].SetActive (true);
			} else {
				envRightGround [i].SetActive (false);
			}
		}
		envTunnel [0].SetActive (Tunnel);
		isTunnel = Tunnel;

	}

	// Prepara este nodo para que sea un punto de control activo.

	public void SetAsActiveCheckpoint(float _timeAwarded)
	{
		timeAwarded = _timeAwarded;
		CP_VisualNormal.SetActive (!isTunnel);
		CP_VisualTunnel.SetActive (isTunnel);
		checkPointTrigger.tag = "CP_Active";
		checkPointTrigger.SetActive (true);

		GetComponent<AddToMinimap> ().SetAsActiveOnMinimap (true);
	}
	public void SetAsPassiveCheckpoint()
	{
		timeAwarded = 0;
		CP_VisualNormal.SetActive (false);
		CP_VisualTunnel.SetActive (false);
		checkPointTrigger.tag = "CP_Passive";
		checkPointTrigger.SetActive (true);
		GetComponent<AddToMinimap> ().SetAsActiveOnMinimap (false);
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
	public float GetTimeAwarded()
	{
		return timeAwarded;
	}
}
