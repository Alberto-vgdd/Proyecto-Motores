using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GarageSubPanelBehaviour : MonoBehaviour {

	public Text text_carName;
	public Text text_details;
	public Image carSelected;
	public Image image_background;

	private int m_index;


	// Use this for initialization
	public void OnClick()
	{
		MainMenuManager.currentInstance.SetCarSelected(m_index);
	}
	public void SetPanelForCar(CarData data, int index)
	{
		carSelected.gameObject.SetActive (false);
		m_index = index;
		text_carName.text = data.GetCarName();
		text_details.text = "CLASS 1";
	}
	public void SetSelected(bool arg)
	{
		carSelected.gameObject.SetActive (arg);
	}
}
