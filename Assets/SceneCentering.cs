using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCentering : MonoBehaviour {

	public Transform playerTransform;
	public Transform worldTransform;

	private float distanceLimit = 1000;
	private float checkInterval = 1f;

	// Use this for initialization
	void Start () {
		StartCoroutine("CheckOffset");
	}

	IEnumerator CheckOffset () {
		while (true) {
			if (playerTransform.position.z > distanceLimit) {
				worldTransform.Translate (-distanceLimit * Vector3.forward);
				RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			}
			if (playerTransform.position.x > distanceLimit) {
				worldTransform.transform.Translate (-distanceLimit * Vector3.right);
				RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			}
			if (playerTransform.position.x < -distanceLimit) {
				worldTransform.transform.Translate (distanceLimit * Vector3.right);
				RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			}
			yield return new WaitForSeconds (checkInterval);
		}
	}
}
