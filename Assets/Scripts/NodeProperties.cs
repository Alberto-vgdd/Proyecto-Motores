using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeProperties : MonoBehaviour {

	// Administra la informacion almacenada en cada nodo y sus propiedades, para que funcione correctamente necesita:
	// - Referencias a las partes del nodo: Posiciones de decoracion y su parent, Trigger de checkpoint y su parent.
	// - Debe estar colocado en el prefab del nodo.

	[Header("References")]
	public List<Transform> envorimentPositions;									// Posiciones de decoracion ambiental posibles.
	public List<GameObject> posibleEnvDeco;										// (TO DO: Mover a un prefab manager) Instancias de decoracion.
	public Transform envoirmentParent;											// (UNUSED) Parent de las decoraciones
	public Transform checkPointVisualParent;									// (UNUSED) Parent de la parte visual del checkpoint
	public GameObject checkPointTrigger;										// Trigger del checkpoint.

	[Header("Node Parameters")]
	public bool isActiveCheckPoint;												// Es un nodo activo? (Restaura tiempo)
	public float nodeWeight;													// Peso (Longitud/Dificultad) del nodo.
	public int forcedStraightAfter;												// Linea recta que fuerza despues de este nodo.
	public float relativeDispFwd;												// Desplazamiento relativo del nodo [ FRONTAL ]
	public float relativeDispSdw;												// Desplazamiento relativo del nodo [ LATERAL ]
	public float relativeDispUp;												// Desplazamiento relativo del nodo [ VERTICAL ]
	public int relativeRotation;												// Rotacion relativa del nodo [ EJE Y ]

	private GameObject lastInstancedDecoration;									// (TEMP) Ultima decoracion colocada.




	// Transforma el nodo en activo (Funcion llamada desde MapGeneration)

	public void SetAsActiveCheckPoint()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.tag = "CP_Active";
		isActiveCheckPoint = true;
	}

	// Desactiva el checkpoint (Funcion llamada por PlayerMovement al cruzar el punto de control)

	public void DisableCheckPoint()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.GetComponent<Collider> ().enabled = false;
	}

	// Reactiva el punto de control como WrongWay (Funcion llamada al cruzar dos puntos de control por delante de este)

	public void SetAsWrongWay ()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.GetComponent<Collider> ().enabled = true;
		checkPointTrigger.tag = "CP_WrongWay";
	}

	// Crea las decoraciones ambientales (Funcion llamada por MapGeneration al instanciar el nodo)

	public void SetEnvoirmentDecoration(float density)
	{
		for (int i = 0; i < envorimentPositions.Count; i++) {
			if (Random.Range (1, 100) < density) {
				lastInstancedDecoration = Instantiate(posibleEnvDeco[Random.Range(0, posibleEnvDeco.Count)], envorimentPositions[i].transform.position, 
					envorimentPositions[i].transform.rotation, transform) as GameObject;
			}
		}
	}
}
