using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddToMinimap : MonoBehaviour {

	private float scaleConversionFactor = 0.2f;

	public Image iconToInstantiate;
	private Image reference;
	private bool isActiveCP = false;
	// Use this for initialization
	void Start () {
		reference = Instantiate (iconToInstantiate, MinimapManager.currentInstance.gameObject.transform) as Image;
		reference.transform.localPosition = new Vector3(transform.position.x, transform.position.z, 0) * scaleConversionFactor;
		reference.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.y);
		if (isActiveCP)
			reference.color = Color.yellow;
	}
	
	// Update is called once per frame
	public void SetAsActiveOnMinimap()
	{
		isActiveCP = true;
	}
	void OnDestroy()
	{
		Destroy (reference);
	}
}
