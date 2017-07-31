using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour {

	public static MinimapManager currentInstance;

	public GameObject parentCG;
	public Transform displaceParent;
	public GameObject playerReference;
	private float scaleConversionFactor = 0.2f;

	private Vector3 initialPosition;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		initialPosition = displaceParent.localPosition;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (playerReference != null) {
			// Z -> Y | X -> X | IGNORED: Y -> Z | rotation y -> z
			displaceParent.localPosition = initialPosition + new Vector3(-playerReference.transform.position.x, -playerReference.transform.position.z, 0) * scaleConversionFactor;
			parentCG.transform.rotation = Quaternion.Euler(0, 0, playerReference.transform.rotation.eulerAngles.y);
		}
	}
}
