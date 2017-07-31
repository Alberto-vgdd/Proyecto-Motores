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
		if (referencedImage == null) {
			referencedImage = Instantiate (iconToInstantiate, MinimapManager.currentInstance.displaceParent) as Image;
		}
		referencedImage.gameObject.SetActive (true);
		referencedImage.transform.SetAsLastSibling ();
		isActiveCP = arg;
		if (isActiveCP) {
			referencedImage.color = Color.yellow;
		} else {
			referencedImage.color = Color.white;
		}
		UpdateMinimapPosition ();
	}
	public void UpdateMinimapPosition()
	{
		referencedImage.transform.localPosition = new Vector3(transform.position.x, transform.position.z, 0) * scaleConversionFactor;
		referencedImage.transform.localRotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.y);
	}
	void OnDisable()
	{
		if (referencedImage != null)
			referencedImage.gameObject.SetActive (false);
	}
}
