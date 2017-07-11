using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPool : MonoBehaviour {

	public static RoadPool currentInstance;

	public List<GameObject> instantiableRoads;
	public List<GameObject> pooledRoadSections;

	private GameObject lastInstantiatedNode;

	void Awake ()
	{
		currentInstance = this;
		pooledRoadSections = new List<GameObject>();
	}
		

	public GameObject GetRoadPiece(int id)
	{
		GameObject nodeFound = null;
		for (int i = 0; i < pooledRoadSections.Count; i++) {
			if (pooledRoadSections [i].activeInHierarchy)
				continue;
			if (pooledRoadSections [i].GetComponent<RoadNode> ().roadPieceID == id) {
				nodeFound = pooledRoadSections [i];
				break;
			}
		}
		if (nodeFound == null) {
			nodeFound = addNodeToPool (id);
		}
		if (nodeFound == null) {
			print ("[POOL] Warning: No nodes avaiable found in pool.");
			return null;
		}
		nodeFound.SetActive(true);
		return nodeFound;
	}
	private GameObject addNodeToPool(int id)
	{
		for (int i = 0; i < instantiableRoads.Count; i++) {
			if (instantiableRoads [i].GetComponent<RoadNode> ().roadPieceID == id) {
				lastInstantiatedNode = Instantiate (instantiableRoads[i], this.transform) as GameObject;
				pooledRoadSections.Add (lastInstantiatedNode);
				return lastInstantiatedNode;
			}
		}
		return null;
	}
}
