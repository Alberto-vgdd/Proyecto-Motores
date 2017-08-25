using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatSliderBehaviour : MonoBehaviour {

	public string statNameString;
	[Header("Text/Sliders")]
	public Slider statSliderBase;
	public Slider statSliderBonus;
	public Text statTextBase;
	public Text statTextBonus;
	public Text statName;

//	[Header("Fill/Handles")]
//	public Image statSliderBaseHandle;
//	public Image statSliderBaseFill;
//	public Image statSliderBonusHandle;
//	public Image statSliderBonusFill;

	private float statBaseTarget = 0;
	private float statBaseCurrent = 0;

	private float statBonusTarget = 0;
	private float statBonusCurrent = 0;

	private float transitionSpeed = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		statBaseCurrent = Mathf.MoveTowards (statBaseCurrent, statBaseTarget, Time.deltaTime * transitionSpeed);
		statBonusCurrent = Mathf.MoveTowards (statBonusCurrent, statBonusTarget, Time.deltaTime * transitionSpeed);
		statSliderBase.value = Mathf.Clamp01 (0.1f + statBaseCurrent * 0.9f);
		statSliderBonus.value = Mathf.Clamp01 (0.1f + statBonusCurrent * 0.9f);
	}
	public void SetValues(float statBase, float statBonus)
	{
		statBase = Mathf.Clamp (statBase, 0, 10);
		statBonus = Mathf.Clamp (statBonus, -10, 10);
		statBaseTarget = Mathf.Clamp01(statBase / 10f);
		statBonusTarget = Mathf.Clamp01((statBase + statBonus) / 10f);
		statTextBase.text = Mathf.Clamp(statBase, 0, 10).ToString ("F1");
		statTextBonus.text = Mathf.Clamp(statBase + statBonus, 0, 10).ToString ("F1");
		if (statBonus < 0) {
			statName.text = statNameString + " (" + statBonus.ToString ("F1") + ")";
		} else {
			statName.text = statNameString + " (+" + statBonus.ToString ("F1") + ")";
		}

	}
}
