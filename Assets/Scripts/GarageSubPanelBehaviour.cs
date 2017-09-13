using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GarageSubPanelBehaviour : MonoBehaviour {

	public Text text_carName;
	public Text text_details;
	public Image carSelected;
	public Image image_background;
	public GameObject infoParent;
	public SelfBlinkAnimation SBA;

	public List<Sprite> carIcons;
	private bool valid = false;

	private int m_index;


	// Use this for initialization
	public void OnClick()
	{
		if (!valid)
			return;
		MainMenuManager.currentInstance.SetCarSelected(m_index);
		SBA.ResetBlinkingAnimation ();
		CarModel.currentInstance.ChangeMaterial(CarMaterials.currentInstance.GetMaterial(m_index));

	}
	public void SetPanelForCar(CarData data, int index)
	{
		if (data == null) {
			infoParent.SetActive (false);
			image_background.gameObject.SetActive (false);
			valid = false;
			return;
		}
		valid = true;
		infoParent.SetActive (true);
		image_background.gameObject.SetActive (true);
		carSelected.gameObject.SetActive (false);
		m_index = index;
		text_carName.text = data.GetCarName();
		text_details.text = "CLASS A";
		if (data.GetSkinId () < carIcons.Count) {
			image_background.sprite = carIcons [data.GetSkinId ()];
		}
	}
	public void SetSelected(bool arg)
	{
		carSelected.gameObject.SetActive (arg);
	}
}
