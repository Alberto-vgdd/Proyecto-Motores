using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {
	public Text timeRemainingText;
	public Text timeAddedText;

	private float lastRegistredTime;
	private StageData sd;
	private Animator animAT;


	// Use this for initialization
	void Start () {
		animAT = timeAddedText.GetComponent<Animator> ();
		sd = StageData.currentData;
		lastRegistredTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (sd == null) {
			sd = StageData.currentData;
		} else {
			if (lastRegistredTime < sd.remainingSec) {
				timeAddedText.text = "+ " + (int)(sd.remainingSec - lastRegistredTime +0.1);
				animAT.SetTrigger ("TriggerIncrease");
			}
			lastRegistredTime = sd.remainingSec;
		}
	}
}
