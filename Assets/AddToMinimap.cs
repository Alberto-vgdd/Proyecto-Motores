using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddToMinimap : MonoBehaviour {

	private float scaleConversionFactor = 0.2f;

	public Image iconToInstantiate;
	private Image referencedImage;
	private bool isActiveCP = false;

	// Update is called once per frame
	public void SetAsActiveOnMinimap(bool arg)
	{
		isActiveCP = arg;
		referencedImage = Instantiate (iconToInstantiate, MinimapManager.currentInstance.gameObject.transform) as Image;
		referencedImage.transform.localPosition = new Vector3(transform.position.x, transform.position.z, 0) * scaleConversionFactor;
		referencedImage.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.y);
		if (isActiveCP)
			referencedImage.color = Color.yellow;
	}
	void OnDisable()
	{
		Destroy (referencedImage);
	}
}
