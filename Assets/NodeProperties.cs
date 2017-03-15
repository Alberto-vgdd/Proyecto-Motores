using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeProperties : MonoBehaviour {

	[Header("References")]
	public List<Transform> envorimentPositions;
	public List<GameObject> posibleEnvDeco;
	public Transform envoirmentParent;
	public Transform checkPointVisualParent;
	public GameObject checkPointTrigger;

	[Header("Node Parameters")]
	public bool isActiveCheckPoint;
	public float nodeWeight;
	public float relativeDispFwd;
	public float relativeDispSdw;
	public float relativeDispUp;
	public int relativeRotation;

	private GameObject lastInstancedDecoration;

	public void SetAsActiveCheckPoint()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.tag = "CP_Active";
		isActiveCheckPoint = true;
	}
	public void DisableCheckPoint()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.GetComponent<Collider> ().enabled = false;
	}
	public void SetAsWrongWay ()
	{
		if (checkPointTrigger == null)
			return;
		checkPointTrigger.tag = "CP_WrongWay";
	}
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
