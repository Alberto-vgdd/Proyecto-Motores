using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

	// Version mejorada del generador de mapas, ahora mismo no genera diferentes alturas, pero admite carreteras y curvas de cualquier angulo,
	// incluyendo formas irregulares o angulos no multiplos de 90.
	// TODO: Añadir alturas en el caso de que se creen rampas o desniveles.

	public static RoadGenerator currentInstance;

	public int levelSeed;
	public int maxLoadedNodes;								// Maximo de nodos cargados
	private int nodesBehindLoaded = 6;						// Nodos cargados DETRAS del jugador
	public float globalRoadScale;							// Escala global del mundo
	public int nodesBetweenActiveCP;						// Nodos entre puntos de control
	public int curveChance;									// Probabilidad de curva
	public int minStraight;									// Minima recta
	public int maxStraight;									// Maxima recta

	private int nodesSinceLastActiveCP;						// (TEMP) Nodos desde el ultimo punto de control.
	private int nodesSinceLastCurve;						// (TEMP) Nodos desde la ultima curva.
	private bool nextNodeIsCurve;							// Decide si el proximo nodo sera curva.
	private int currentAngle = 0;							// Angulo global de la carretera. (No puede ser ni mayor de 180 ni menor de -180)
	private float currentHeight;							// Altura actual (Sin usar, hacen falta piezas con desnivel)

	private int nodeDecoWallL = 0;							// Indice de la decoracion del [MURO] [IZQUIERDO]
	private int nodeDecoWallR = 0;							// Indice de la decoracion del [MURO] [DERECHO]
	private int nodeDecoGroundL = 0;						// Indice de la decoracion del [SUELO] [IZQUIERDO]
	private int nodeDecoGroundR = 0;						// Indice de la decoracion del [SUELO] [DERECHO]
	private int nodesUnttilDecoChange;						// Nodos hasta el proximo cambio en los indices de la decoracion.

	public List<GameObject> spawnedNodes;					// Nodos creados
	public List<GameObject> availableNodes;					// Nodos disponibles para crear (Ya no es necesario clasificarlos por angulo)
	private List<GameObject> tempValidNodes;				// (TEMP) Lista temporal para determinar cuales seran los posibles proximos nodos.
	private RoadNode lastReadedNode;						// (AUX) Ultimo nodo leido
	private GameObject lastCreatedNode;						// (AUX) Ultimo nodo creado
	private int totalNodesCreated;							// Total de nodos creados
	private float stackedNodeWeight;						// "Peso" acumulado de los nodos, determina el tiempo extra que dara el proximo P.Control.

	private float dayTime;
	private float dayTimescale;

	private float NodeWeight2Time = 0.4f;

	void Awake () {
		currentInstance = this;
	}

	void Start ()
	{
		if (levelSeed == 0)
			levelSeed = Random.Range (1, 9999999);
		Random.InitState(levelSeed);
		totalNodesCreated = 0;
		nodesUnttilDecoChange = -1;
		// Testing
		dayTime = Random.Range(0f, 24f);
		print ("[MAP] Generating seed " + levelSeed + " | DayTime set to " + dayTime);
		dayTimescale = 0.025f;
		DayNightCycle.currentInstance.SetTimeAndTimescale (dayTime, dayTimescale);
		// ==========
		tempValidNodes = new List<GameObject>();
		for (int i = 0; i < maxLoadedNodes - nodesBehindLoaded; i++) {
			SpawnNextNode ();
		}
	}

	// Crea el proximo nodo, mueve el generador de carreteras, y elimina los nodos que sobren.

	public void SpawnNextNode()
	{
		lastCreatedNode = RoadPool.currentInstance.GetRoadPiece(GetNextNodeToSpawn ().GetComponent<RoadNode>().roadPieceID);
		lastCreatedNode.transform.position = transform.position;
		lastCreatedNode.transform.rotation = transform.rotation * Quaternion.Euler(0,90,0);
		lastCreatedNode.SetActive(true);
		lastReadedNode = lastCreatedNode.GetComponent<RoadNode> ();
		nodesSinceLastActiveCP++;
		stackedNodeWeight += lastReadedNode.nodeWeight;

		// Node setup

		lastReadedNode.SetID (totalNodesCreated);
		lastCreatedNode.transform.localScale = Vector3.one * globalRoadScale;
		SetupDecorationsForNextNode ();
		lastReadedNode.SetLightState (DayNightCycle.currentInstance.getLightsOn());
		lastReadedNode.SetLighScale (globalRoadScale);
		//Aqui creamos CheckPoint
		if (nodesSinceLastActiveCP >= nodesBetweenActiveCP) {
			lastReadedNode.SetAsActiveCheckpoint ((stackedNodeWeight * NodeWeight2Time));
			nodesSinceLastActiveCP = 0;
			stackedNodeWeight = 0;
			NodeWeight2Time *= 0.98f;
			curveChance = Mathf.Clamp (curveChance + 3, 0, 100);
		} else {
			lastReadedNode.SetAsPassiveCheckpoint ();
		}

		// Self setup for next node

		transform.Translate (Vector3.forward * lastReadedNode.dispFrontal * globalRoadScale);
		transform.Translate (Vector3.right * lastReadedNode.dispLateral * globalRoadScale);
		transform.Rotate (new Vector3 (0, lastReadedNode.dispAngular, 0));
		if (lastReadedNode.dispAngular == 0) {
			nodesSinceLastCurve++;
		} else {
			nodesSinceLastCurve = 0;
			currentAngle += (int) lastReadedNode.dispAngular;
		}
		totalNodesCreated++;
		spawnedNodes.Add (lastCreatedNode);

		// Remove Exceding nodes

		if (spawnedNodes.Count > maxLoadedNodes) {
			spawnedNodes [nodesBehindLoaded - 2].GetComponent<RoadNode> ().SetAsWrongWay ();
			GameObject nodeToRemove = spawnedNodes [0];
			spawnedNodes.Remove (nodeToRemove);
			nodeToRemove.SetActive(false);
		}

	}

	public GameObject GetNextNodeToSpawn()
	{
		nextNodeIsCurve = (nodesSinceLastCurve >= minStraight) && ( (nodesSinceLastCurve > maxStraight) || (Random.Range(1,101) < curveChance) ); 
		if (totalNodesCreated < 4)
			nextNodeIsCurve = false;
		tempValidNodes.Clear ();
		for (int i = 0; i < availableNodes.Count; i++) {
			lastReadedNode = availableNodes [i].GetComponent<RoadNode>();
			if ((lastReadedNode.dispAngular + currentAngle) > 90 || (lastReadedNode.dispAngular + currentAngle) < -90) {
				continue;
			}
			if (nextNodeIsCurve && lastReadedNode.dispAngular == 0) {
				continue;
			} else if (!nextNodeIsCurve && lastReadedNode.dispAngular != 0) {
				continue;
			}
			tempValidNodes.Add (availableNodes [i]);

		}
		return tempValidNodes [Random.Range (0, tempValidNodes.Count)];
	}

	// Prepara la decoracion para el ultimo nodo que se ha creado (utiliza lastReadedNode)

	void SetupDecorationsForNextNode()
	{
		
		nodesUnttilDecoChange--;
		if (nodesUnttilDecoChange <= 0) {
			nodesUnttilDecoChange = Random.Range (3, 10);
			nodeDecoWallL = Random.Range (0, 5);
			nodeDecoWallR = Random.Range (0, 5);

			if (nodeDecoWallL == 1) { // Concrete wall FORCES water canal
				nodeDecoGroundL = 3; // Water canal
			} else {
				nodeDecoGroundL = Random.Range (0, 3);
			}
			if (nodeDecoWallR == 1) { // Concrete wall FORCES water canal
				nodeDecoGroundR = 3; // Water canal
			} else {
				nodeDecoGroundR = Random.Range (0, 3);
			}
		}
        lastReadedNode.SetEnvoirment (nodeDecoWallL, nodeDecoWallR, nodeDecoGroundL, nodeDecoGroundR);
	}

	//TODO: Esto no deberia estar aqui...
	public void UpdateMinimapForAllActivePieces()
	{
		for (int i = 0; i < spawnedNodes.Count; i++) {
			spawnedNodes [i].GetComponent<AddToMinimap> ().UpdateMinimapPosition ();
		}
	}
}
