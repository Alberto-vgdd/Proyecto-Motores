﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

	// Version mejorada del generador de mapas, ahora mismo no genera diferentes alturas, pero admite carreteras y curvas de cualquier angulo,
	// incluyendo formas irregulares o angulos no multiplos de 90.
	// TODO: Añadir alturas en el caso de que se creen rampas o desniveles.

	public static RoadGenerator currentInstance;

	private int levelSeed;
	private int nodesBehindLoaded = 6;						// Nodos cargados DETRAS del jugador
	private int curveChance;								// Probabilidad de curva
	private int minStraight;								// Minima recta
	private int maxStraight;								// Maxima recta
	private float maxHeight;
	private float rampChance;

	private int nodesSinceLastActiveCP;						// (TEMP) Nodos desde el ultimo punto de control.
	private int nodesSinceLastCurve;						// (TEMP) Nodos desde la ultima curva.
	private int nextNodeType;								// Tipo del proximo nodo: 1 = recta, 2 = curva, 3 = inclinacion.
	private int currentAngle = 0;							// Angulo global de la carretera. (No puede ser ni mayor de 180 ni menor de -180)
	private float currentHeight = 0;						// Altura actual
	private float currentInclination = 0;
	private float maxAbsoluteInclinationDegree = 15f;		// Desnivel maximo (absoluto)
	private int forcedSraight = 5;

	private int nodeDecoTunnelEnabledFor = 0;
	private int nodeDecoWallL = 0;							// Indice de la decoracion del [MURO] [IZQUIERDO]
	private int nodeDecoWallR = 0;							// Indice de la decoracion del [MURO] [DERECHO]
	private int nodeDecoGroundL = 0;						// Indice de la decoracion del [SUELO] [IZQUIERDO]
	private int nodeDecoGroundR = 0;						// Indice de la decoracion del [SUELO] [DERECHO]
	private int nodesUnttilDecoChange;						// Nodos hasta el proximo cambio en los indices de la decoracion.

	public GameObject roadStart;
	private bool roadStartRemoved = false;

	public List<GameObject> spawnedNodes;					// Nodos creados
	private List<GameObject> tempValidNodes;				// (TEMP) Lista temporal para determinar cuales seran los posibles proximos nodos.
	private RoadNode lastReadedNode;						// (AUX) Ultimo nodo leido
	private GameObject lastCreatedNode;						// (AUX) Ultimo nodo creado
	private int totalNodesCreated;							// Total de nodos creados
	private float stackedNodeWeight;						// "Peso" acumulado de los nodos, determina el tiempo extra que dara el proximo P.Control.

	private const int FORCED_STRAIGHT_AFTER_RAMP = 2;
	private const int NODES_BETWEEN_CHECKPOINTS = 25;	
	private const float GLOBAL_ROAD_SCALE = 10;

	private float NodeWeight2Time = 0.4f;
	private int maxLoadedNodes = 24;

	void Awake () {
		currentInstance = this;
	}

	void Start ()
	{
		maxLoadedNodes = GlobalGameData.currentInstance.m_gameSettings_nodesLoaded;
		levelSeed = GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed();
		Random.InitState(levelSeed);
		curveChance = Random.Range (10, 71);
		minStraight = Random.Range (0, 3);
		maxStraight = Random.Range (minStraight, minStraight +5);
		Random.Range (1, 24); // LLAMADA DUMMY, NO HACE NADA, solo sincroniza la seed.
		maxHeight = 5;
		roadStart.transform.localScale = Vector3.one * GLOBAL_ROAD_SCALE;

		if (Random.Range (1, 4) == 1) {
			rampChance = 0;
		} else {
			rampChance = Random.Range (3, 10);
		}

		totalNodesCreated = 0;
		nodesUnttilDecoChange = -1;
		print ("[MAP] Generating seed " + levelSeed + " | DayTime set to " + GlobalGameData.currentInstance.m_playerData_eventActive.GetStartingHour ());
		print ("[MAP] Rampchance: " + rampChance);
		print ("[MAP] MaxStraight: " + maxStraight);
		print ("[MAP] Curvechance:" + curveChance);
	
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
		currentHeight += lastReadedNode.dispVertical;
		currentInclination += lastReadedNode.dispAngularVertical;
		if (lastReadedNode.dispAngularVertical != 0 && currentInclination == 0) {
			forcedSraight = FORCED_STRAIGHT_AFTER_RAMP;
		}

		// Node setup

		lastReadedNode.SetID (totalNodesCreated);
		lastCreatedNode.transform.localScale = Vector3.one * GLOBAL_ROAD_SCALE;
		SetupDecorationsForNextNode ();
		lastReadedNode.SetLightState (DayNightCycle.currentInstance.getLightsOn());
		lastReadedNode.SetLighScale (GLOBAL_ROAD_SCALE);
		//Aqui creamos CheckPoint
		if (nodesSinceLastActiveCP >= NODES_BETWEEN_CHECKPOINTS) {
			lastReadedNode.SetAsActiveCheckpoint ((stackedNodeWeight * NodeWeight2Time));
			nodesSinceLastActiveCP = 0;
			stackedNodeWeight = 0;
		} else {
			lastReadedNode.SetAsPassiveCheckpoint ();
		}

		// Self setup for next node

		transform.Translate (Vector3.forward * lastReadedNode.dispFrontal * GLOBAL_ROAD_SCALE);
		transform.Translate (Vector3.right * lastReadedNode.dispLateral * GLOBAL_ROAD_SCALE);
		transform.Translate (Vector3.up * lastReadedNode.dispVertical * GLOBAL_ROAD_SCALE);
		transform.Rotate (new Vector3 (0, lastReadedNode.dispAngularHorizontal, 0));
		transform.Rotate (new Vector3 (lastReadedNode.dispAngularVertical, 0, 0));
		if (lastReadedNode.dispAngularHorizontal == 0) {
			nodesSinceLastCurve++;
		} else {
			nodesSinceLastCurve = 0;
			currentAngle += (int) lastReadedNode.dispAngularHorizontal;
		}
		totalNodesCreated++;
		spawnedNodes.Add (lastCreatedNode);

		// Remove Exceding nodes

		if (spawnedNodes.Count > maxLoadedNodes) {
			spawnedNodes [nodesBehindLoaded - 2].GetComponent<RoadNode> ().SetAsWrongWay ();
			GameObject nodeToRemove = spawnedNodes [0];
			spawnedNodes.Remove (nodeToRemove);
			nodeToRemove.SetActive(false);
			if (!roadStartRemoved) {
				roadStartRemoved = true;
				Destroy (roadStart);
			}
		}

	}

	public GameObject GetNextNodeToSpawn()
	{
		
		if (forcedSraight > 0) {
			forcedSraight--;
			nextNodeType = 1;
		} else {
			if (currentInclination == 0) {
				if (Random.Range (1, 101) < rampChance) {
					nextNodeType = 3;
				} else if ((nodesSinceLastCurve >= minStraight) && ((nodesSinceLastCurve > maxStraight) || (Random.Range (1, 101) < curveChance)) && currentInclination == 0) {
					nextNodeType = 2;
				} else {
					nextNodeType = 1;
				}
			} else {
				nextNodeType = 3;
			}
		}
			
		tempValidNodes.Clear ();
		for (int i = 0; i < RoadPool.currentInstance.instantiableRoads.Count; i++) {
			lastReadedNode = RoadPool.currentInstance.instantiableRoads [i].GetComponent<RoadNode>();

			switch (nextNodeType) {
			case 1: // Recta
				{
					if (lastReadedNode.dispVertical != 0 || lastReadedNode.dispAngularHorizontal != 0)
						continue;
					break;
				}
			case 2: // Curva
				{
					if (lastReadedNode.dispVertical != 0 || lastReadedNode.dispAngularHorizontal == 0)
						continue;
					if (Mathf.Abs (lastReadedNode.dispAngularHorizontal + currentAngle) > 90)
						continue;
					break;
				}
			case 3: // Inclinacion
				{
					if (lastReadedNode.dispVertical == 0)
						continue;
					if (Mathf.Abs (lastReadedNode.dispAngularVertical + currentInclination) > maxAbsoluteInclinationDegree)
						continue;
					if ( (lastReadedNode.dispVertical <= 0 && transform.position.y < 0.3f*GLOBAL_ROAD_SCALE) || (lastReadedNode.dispVertical >= 0 && transform.position.y > maxHeight*GLOBAL_ROAD_SCALE))
						continue;

					break;
				}
			default:
				{
					// Ninguna comprobacion.
					break;
				}
			}
			tempValidNodes.Add (RoadPool.currentInstance.instantiableRoads[i]);

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
		if (nodeDecoTunnelEnabledFor <= 0) {
			if (Random.Range (1, 101) < 3)
				nodeDecoTunnelEnabledFor = Random.Range (2, 20);
		} else {
			nodeDecoTunnelEnabledFor--;
		}
		lastReadedNode.SetEnvironment (nodeDecoWallL, nodeDecoWallR, nodeDecoGroundL, nodeDecoGroundR, nodeDecoTunnelEnabledFor > 0);
	}

	//TODO: Esto no deberia estar aqui...
	public void UpdateMinimapForAllActivePieces()
	{
		for (int i = 0; i < spawnedNodes.Count; i++) {
			spawnedNodes [i].GetComponent<AddToMinimap> ().UpdateMinimapPosition ();
		}
	}
}
