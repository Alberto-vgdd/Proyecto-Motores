using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

	// Administra la generacion de la carretera y el terreno a su alrededor. Para que funcione correctamente necesita:
	// - Referencias a los prefabs que puede instanciar.
	// - Ajustar parametros de creacion de nivel.
	// TODO: Considerar usar un object pool para las piezas en lugar de instanciar y destruir.

	public static MapGeneration currentData;								// Referencia estatica.

	public int baseNodeSize;												// Tamaño del mundo (Ajustable)

	[Header("Map Generation Parameters")]
	public float maxHeight;													// Altura maxima
	public float minHeight;													// Altura minima (recomendado 0)
	public int minStraight;													// Minima recta posible
	public int maxStraight;													// Maxima recta posible
	public float curveChance;												// Probabilidad de recta
	public float rampChance;												// Probabilidad de desnivel
	public int currentDegree;												// Giro despues de la ultima curva
	public float EnvoirmentalDecorationDensity;								// Densidad de decoraciones
	public int nodesBetweenActiveCheckpoints;								// Nodos entre puntos de control activos.
	public int nodesSpawned;												// Contador de nodos, se usa para asignar IDs.

	[Header("Render Parameters")]

	// - LoadedNodesInitial debe ser menor que LoadedNodesMax.
	// - La diferencia entre LoadedNodesInitial y LoadedNodesMax son los nodos que el jugador tendra detras.
	// - IndexOfWrongWay debe ser un numero menor que esta diferencia - 2;
	// - TODO: automatizar todo esto(?)

	public int loadedNodesInitial;											// Nodos cargados inicialmente.
	public int loadedNodesMax;												// Nodos maximos cargados.
	public int indexOfWrongWay;												// Indice del nodo wrong way.

	private NodeProperties lastReadedNode;									// (TEMP) Ultimo NodeProperties leido.
	private GameObject lastInstancedNode;									// (TEMP) Ultimo nodo instanciado.

	private bool nextNodeIsStraight;										// (TEMP) El proximo nodo es una recta?
	private bool nextNodeL;													// (TEMP) El proximo nodo gira a la izquierda si no es recto?
	private bool nextNodeIsRamp;											// (TEMP) El proximo nodo sera un cambio de altura?
	private bool nextNodeIsRampUp;											// (TEMP) El proximo nodo de cambio de altura sera una subida?
	private int nodesSinceLastCheckpoint;									// (TEMP) Nodos desde el ultimo punto de control.
	private int forcedStraight;												// (TEMP) Rectas forzadas restantes.
	private float weightAccumulated;										// (TEMP) "Peso" acumulado de los nodos.
	private int straightChain;												// (TEMP) Cadena de rectas actual.
	private float currentHeight;											// (TEMP) Altura actual.

	[Header("References")]
	public List<GameObject> InstancedNodes;									// Todos los nodos que se han instanciado
	public List<GameObject> StraightNodes;									// Piezas rectas que puede instanciar.
	public List<GameObject> LeftNodes_90;									// Curvas izquierda (90º) que puede instanciar.
	public List<GameObject> RightNodes_90;									// Curvas derecha (90º) que puede instanciar.
	public List<GameObject> StrUpNodes;										// Rampas hacia arriba.
	public List<GameObject> StrDownNodes;									// Rampas hacia abajo.

	// TODO: Es necesario realmente diferenciar entre nodos izquierda y derecha? Unity da problemas al cambiar la escala a negativo.

	void Awake() { currentData = this; }
	void Start()
	{
		forcedStraight = 0;
		currentDegree = 0;
		SpawnMultipleNodes (loadedNodesInitial);
	}

	// Decide cual sera el proximo nodo que creara.

	public void SpawnMultipleNodes(int amount)
	{
		for (int i = 0; i < amount; i++) {
			SpawnNode (GetNodeToSpawn ());
			if (InstancedNodes.Count > loadedNodesMax) {
				lastInstancedNode = InstancedNodes [0];
				InstancedNodes.RemoveAt (0);
				InstancedNodes [indexOfWrongWay].GetComponent<NodeProperties> ().SetAsWrongWay ();
				Destroy (lastInstancedNode);
			}
		}
	}

	private GameObject GetNodeToSpawn()
	{
		nextNodeIsStraight = (straightChain < minStraight) && (straightChain < maxStraight) || !(Random.Range (1, 100) < curveChance);
		if (forcedStraight > 0)
			nextNodeIsStraight = true;
		nextNodeIsRamp = Random.Range(1,100) < rampChance;
		if (currentHeight > 0) {
			if (Random.Range (1, 100) < 25) {
				return StrDownNodes [Random.Range (0, StrDownNodes.Count)];
			}
		}
		if (nextNodeIsRamp) {
			if (currentHeight > minHeight) {
				if (currentHeight < maxHeight) {
					nextNodeIsRampUp = Random.Range (1, 3) == 1;
				} else {
					nextNodeIsRampUp = false;
				}
			} else {
				nextNodeIsRampUp = true;
			}

			if (nextNodeIsRamp && nextNodeIsRampUp) {
				return StrUpNodes [Random.Range (0, StrUpNodes.Count)];
			} else if (nextNodeIsRamp) {
				return StrDownNodes [Random.Range (0, StrDownNodes.Count)];
			}

		}
		nextNodeL = Random.Range (1, 3) == 1;

		switch (currentDegree) {
		case 0: // [STRAIGHT]
			{
				if (nextNodeIsStraight) {
					straightChain++;
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL) {
					straightChain = 0;
					return LeftNodes_90 [Random.Range (0, LeftNodes_90.Count)];
				} else {
					straightChain = 0;
					return RightNodes_90 [Random.Range (0, RightNodes_90.Count)];
				}
			}
		case 90: // [RIGHT]
			{
				if (nextNodeIsStraight) {
					straightChain++;
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL || straightChain > maxStraight) {
					straightChain = 0;
					return LeftNodes_90 [Random.Range (0, LeftNodes_90.Count)];
				} else {
					straightChain++;
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				}
			}
		case -90: // [LEFT]
			{
				if (nextNodeIsStraight) {
					straightChain++;
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else if (nextNodeL && straightChain < maxStraight) {
					straightChain++;
					return StraightNodes [Random.Range (0, StraightNodes.Count)];
				} else {
					straightChain = 0;
					return RightNodes_90 [Random.Range (0, RightNodes_90.Count)];
				}
			}
		}
		return null;
	}

	// Llamado por el jugador al cruzar un punto de control (activo o pasivo), crea un nodo mas al final del circuito

	public void CrossCheckPoint (int lastPlayerCrossedNode, int currentPlayerCrossedNode)
	{
		SpawnMultipleNodes (currentPlayerCrossedNode-lastPlayerCrossedNode);
	}

	// Dado un nodo, lo crea, y prepara.

	void SpawnNode(GameObject nodeToSpawn)
	{
		lastInstancedNode = Instantiate (nodeToSpawn, transform.position, transform.rotation * Quaternion.Euler(0,90,0)) as GameObject;
		lastReadedNode = lastInstancedNode.GetComponent<NodeProperties> ();
		lastReadedNode.nodeId = nodesSpawned;
        lastReadedNode.absoluteScale = baseNodeSize;
        lastReadedNode.SetEnvoirmentDecoration (EnvoirmentalDecorationDensity, currentHeight);
		currentHeight += lastReadedNode.relativeDispUp;
		forcedStraight += lastReadedNode.forcedStraightAfter;
		weightAccumulated += lastReadedNode.nodeWeight;
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
		// TEST ==========================
		curveChance++;
		EnvoirmentalDecorationDensity++;
		// ===============================
		nodesSpawned++;

	}
}
