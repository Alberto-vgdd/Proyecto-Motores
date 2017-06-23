using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCentering : MonoBehaviour {

	public Transform playerTransform;
	public Transform worldTransform;

	private float distanceLimit = 1000;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (playerTransform.position.z > distanceLimit) {
			worldTransform.Translate (-distanceLimit * Vector3.forward);
			RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			print("Centering on Z axis");
		}
		if (playerTransform.position.x > distanceLimit) {
			worldTransform.transform.Translate (-distanceLimit * Vector3.right);
			RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			print("Centering on X axis");
		}
		if (playerTransform.position.x < -distanceLimit) {
			worldTransform.transform.Translate (distanceLimit * Vector3.right);
			RoadGenerator.currentInstance.UpdateMinimapForAllActivePieces ();
			print("Centering on -X axis");
		}
		
	}
}
