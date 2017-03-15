using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

	public static MapGeneration currentData;

	public int baseNodeSize;

	[Header("Map Generation Parameters")]
	public int minStraight;
	public int maxStraight;
	public float curveChance;
	public int currentDegree;
	public float EnvoirmentalDecorationDensity;
	public int nodesBetweenActiveCheckpoints;

	[Header("Render Parameters")]
	public int loadedNodesInitial;
	public int loadedNodesMax;

	private NodeProperties lastReadedNode;
	private GameObject lastInstancedNode;

	private bool nextNodeIsStraight;
	private bool nextNodeL;
	private int nodesSinceLastCheckpoint;

	[Header("References")]
	public List<GameObject> InstancedNodes;
	public List<GameObject> StraightNodes;
	public List<GameObject> LeftNodes_90;
	public List<GameObject> RightNodes_90;

	void Awake()
	{
		currentData = this;
	}
	void Start()
	{
		currentDegree = 0;
		for (int i = 0; i < loadedNodesInitial; i++) {
			SpawnNode (GetNodeToSpawn ());
		}
	}
	private GameObject GetNodeToSpawn()
	{
		nextNodeIsStraight = !(Random.Range (1, 100) < curveChance);
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
	public void CrossCheckPoint()
	{
		SpawnNode (GetNodeToSpawn ());
		if (InstancedNodes.Count > loadedNodesMax) {
			InstancedNodes.RemoveAt (0);
		}
	}
	void SpawnNode(GameObject nodeToSpawn)
	{
		lastInstancedNode = Instantiate (nodeToSpawn, transform.position, transform.rotation * Quaternion.Euler(0,90,0)) as GameObject;
		lastReadedNode = lastInstancedNode.GetComponent<NodeProperties> ();
		lastReadedNode.SetEnvoirmentDecoration (EnvoirmentalDecorationDensity);
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

		nodesSinceLastCheckpoint++;
		curveChance++;
		EnvoirmentalDecorationDensity++;
	}
}
