using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

	// Administra la generacion de la carretera y el terreno a su alrededor. Para que funcione correctamente necesita:
	// - Referencias a los prefabs que puede instanciar.
	// - Ajustar parametros de creacion de nivel.

	public static MapGeneration currentData;								// Referencia estatica.

	public int baseNodeSize;												// Tamaño del mundo (Ajustable)

	[Header("Map Generation Parameters")]
	public int minStraight;													// Minima recta posible
	public int maxStraight;													// Maxima recta posible
	public float curveChance;												// Probabilidad de recta
	public int currentDegree;												// Giro despues de la ultima curva
	public float EnvoirmentalDecorationDensity;								// Densidad de decoraciones
	public int nodesBetweenActiveCheckpoints;								// Nodos entre puntos de control activos.

	[Header("Render Parameters")]
	public int loadedNodesInitial;											// Nodos cargados inicialmente.
	public int loadedNodesMax;												// Nodos maximos cargados.

	private NodeProperties lastReadedNode;									// (TEMP) Ultimo NodeProperties leido.
	private GameObject lastInstancedNode;									// (TEMP) Ultimo nodo instanciado.

	private bool nextNodeIsStraight;										// (TEMP) El proximo nodo es una recta?
	private bool nextNodeL;													// (TEMP) El proximo nodo gira a la izquierda si no es recto?
	private int nodesSinceLastCheckpoint;									// (TEMP) Nodos desde el ultimo punto de control.
	private int forcedStraight;												// (TEMP) Rectas forzadas restantes.

	[Header("References")]
	public List<GameObject> InstancedNodes;									// Todos los nodos que se han instanciado
	public List<GameObject> StraightNodes;									// Piezas rectas que puede instanciar.
	public List<GameObject> LeftNodes_90;									// Curvas izquierda (90º) que puede instanciar.
	public List<GameObject> RightNodes_90;									// Curvas derecha (90º) que puede instanciar.

	// TO DO: Es necesario realmente diferenciar entre nodos izquierda y derecha? Unity da problemas al cambiar la escala a negativo.

	void Awake() { currentData = this; }
	void Start()
	{
		forcedStraight = 0;
		currentDegree = 0;
		for (int i = 0; i < loadedNodesInitial; i++) {
			SpawnNode (GetNodeToSpawn ());
		}
	}

	// Decide cual sera el proximo nodo que creara.

	private GameObject GetNodeToSpawn()
	{
		nextNodeIsStraight = forcedStraight > 0 || !(Random.Range (1, 100) < curveChance);
		nextNodeL = Random.Range (1, 3) == 1;

		switch (currentDegree) {
		case 0: // [STRAIGHT]
			{
				if (nextNodeIsStraight) {
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL) {
					return LeftNodes_90 [Random.Range (0, LeftNodes_90.Count)];
				} else {
					return RightNodes_90 [Random.Range (0, RightNodes_90.Count)];
				}
			}
		case 90: // [RIGHT]
			{
				if (nextNodeIsStraight) {
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL) {
					return LeftNodes_90 [Random.Range (0, LeftNodes_90.Count)];
				} else {
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				}
			}
		case -90: // [LEFT]
			{
				if (nextNodeIsStraight) {
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL) {
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else {
					return RightNodes_90 [Random.Range (0, RightNodes_90.Count)];
				}
			}
		}
		return null;
	}

	// Llamado por el jugador al cruzar un punto de control (activo o pasivo), crea un nodo mas al final del circuito

	public void CrossCheckPoint()
	{
		SpawnNode (GetNodeToSpawn ());
		if (InstancedNodes.Count > loadedNodesMax) {
			InstancedNodes.RemoveAt (0);
		}
	}

	// Dado un nodo, lo crea, y prepara.

	void SpawnNode(GameObject nodeToSpawn)
	{
		lastInstancedNode = Instantiate (nodeToSpawn, transform.position, transform.rotation * Quaternion.Euler(0,90,0)) as GameObject;
		lastReadedNode = lastInstancedNode.GetComponent<NodeProperties> ();
		lastReadedNode.SetEnvoirmentDecoration (EnvoirmentalDecorationDensity);
		forcedStraight += lastReadedNode.forcedStraightAfter;
		lastInstancedNode.transform.localScale *= baseNodeSize;
		transform.Translate (Vector3.forward * lastReadedNode.relativeDispFwd * baseNodeSize);
		transform.Translate (Vector3.right * lastReadedNode.relativeDispSdw * baseNodeSize);
		transform.Translate (Vector3.up * lastReadedNode.relativeDispUp * baseNodeSize);
		transform.Rotate (0, lastReadedNode.relativeRotation, 0);
		currentDegree += lastReadedNode.relativeRotation;

		InstancedNodes.Add (lastInstancedNode);
		if (nodesSinceLastCheckpoint >= nodesBetweenActiveCheckpoints) {
			nodesSinceLastCheckpoint = 0;
			lastReadedNode.SetAsActiveCheckPoint ();
		}

		if (forcedStraight > 0)
			forcedStraight--;
		nodesSinceLastCheckpoint++;
		curveChance++;
		EnvoirmentalDecorationDensity++;
	}
}
