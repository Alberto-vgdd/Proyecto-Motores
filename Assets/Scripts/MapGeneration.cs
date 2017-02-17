using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

	public static MapGeneration currentData;

	[Header("Render parameters")]
	public int maxLoadedNodes;
	public int initialLodadedNodes;
	public int indexOfWrongWayNode;
	[Header("Stage parameters")]
	public int stageLenght;
	public int minStraight;
	public int maxStraight;
	public float curveChance;
	public int nodesBetweenActiveCheckpoints;
	[Header("Node Parameters")]
	public float baseNodeSize;
	[Header("Generation Debug Info")]
	public string turnState;
	public int nodeCount;
	public int straightNodesChained;
	public int curveNodesChained;
	public int totalNodesSpawned;

	GameObject insNode;
	[Header("References")]
	public List<GameObject> nodesInStage;
	public GameObject[] LeftNodes;
	public GameObject[] RightNodes;
	public GameObject[] forwNodes;

	private int nodesWithPassiveCheckpoints = 0;

	// Use this for initialization
	void Awake()
	{
		currentData = this;
	}
	void Start () {
		turnState = "middle";
		
	}
	
	// Update is called once per frame
	void Update () {
		while (nodeCount < initialLodadedNodes) {
			nodeCount++;
			SpawnNode ();

		}
		
	}
	public void SpawnNode()
	{
		totalNodesSpawned++;
		int turn = 2;
		switch (turnState)
		{
		case "left":
			{
				if (!(straightNodesChained < minStraight) && Random.Range (1, 101) < curveChance || straightNodesChained >= maxStraight) {
					turn = 3;
					turnState = "middle";
				} else {
					turn = 2;
					turnState = "left";
				}
				break;
			}
		case "right":
			{
				if (!(straightNodesChained < minStraight) && Random.Range (1, 101) < curveChance || straightNodesChained >= maxStraight) {
					turn = 1;
					turnState = "middle";
				} else {
					turn = 2;
					turnState = "right";
				}
				break;
			}
		case "middle":
			{
				if (!(straightNodesChained < minStraight) && Random.Range (1, 101) < curveChance || straightNodesChained >= maxStraight) {
					if (Random.Range (1, 3) == 1) {
						turn = 1;
						turnState = "left";
					} else {
						turn = 3;
						turnState = "right";
					}
				} else {
					turn = 2;
					turnState = "middle";
				}
				break;
			}
		}
		// Mover y colocar el spawner
		switch (turn) {
		case 1: // left
			{
				SpawnLeftNode ();
				break;
			}
		case 2: // middle
			{
				SpawnStraightNode ();
				break;
			}
		case 3: // right
			{
				SpawnRightNode ();
				break;
			}
		}
		nodesInStage.Add (insNode);
		print ("node Added");
		nodesWithPassiveCheckpoints++;
		curveChance += 1;
		if (nodesWithPassiveCheckpoints > nodesBetweenActiveCheckpoints) {
			insNode.transform.FindChild ("CheckPointTrigger").tag = "checkPointActive";
			nodesWithPassiveCheckpoints = 0;
		}
		if (nodesInStage.Count > maxLoadedNodes) {
			Destroy (nodesInStage [0].gameObject);
			nodesInStage.RemoveAt (0);
			nodesInStage [indexOfWrongWayNode].transform.FindChild ("CheckPointTrigger").tag = "Respawn";
			nodesInStage [indexOfWrongWayNode].transform.FindChild ("CheckPointTrigger").GetComponent<Collider> ().enabled = true;
		}
	}

	void SpawnStraightNode()
	{
		int variation = Random.Range (0, forwNodes.Length);
		switch (variation) {
		case 0:
			{
				insNode = Instantiate (forwNodes[variation], transform.position, transform.rotation) as GameObject;
				straightNodesChained++;
				curveNodesChained = 0;
				// Traslacion propia del nodo
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		case 1:
			{
				// Pre-Posicionamiento
				transform.Translate (Vector3.forward * baseNodeSize * 1);
				insNode = Instantiate (forwNodes[variation], transform.position, transform.rotation) as GameObject;
				straightNodesChained++;
				curveNodesChained = 0;
				// Traslacion propia del nodo
				transform.Translate (Vector3.forward * baseNodeSize * 2);
				break;
			}
		case 2:
			{
				break;
			}
		}
	}

	void SpawnLeftNode()
	{
		int variation = Random.Range (0, LeftNodes.Length);
		switch (variation) {
		case 0:
			{
				insNode = Instantiate (LeftNodes[0], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, -90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		case 1:
			{
				insNode = Instantiate (LeftNodes[1], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, -90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		case 2:
			{
				insNode = Instantiate (LeftNodes[2], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, -90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		}

	}

	void SpawnRightNode()
	{
		int variation = Random.Range (0, RightNodes.Length);
		switch (variation) {
		case 0:
			{
				insNode = Instantiate (RightNodes[0], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, 90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		case 1:
			{
				insNode = Instantiate (RightNodes[1], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, 90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		case 2:
			{
				insNode = Instantiate (RightNodes[2], transform.position, transform.rotation) as GameObject;
				curveNodesChained++;
				straightNodesChained = 0;
				// Traslacion propia del nodo
				transform.Rotate (0, 90, 0);
				transform.Translate (Vector3.forward * baseNodeSize);
				break;
			}
		}
	}
}
