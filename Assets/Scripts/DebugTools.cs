using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugTools : MonoBehaviour {

	public PlayerMovement pm;
	public Text UiDisplayInfo;
	// Use this for initialization
	void Start () {
		pm = GetComponent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (pm != null) {
			UiDisplayInfo.text = "Speed: " + (int)(pm.accumulatedAcceleration*15) + 
				"\nDrift : " + (int)pm.driftDegree + " º" + "\nHealth: " + (int)StageData.currentData.playerHealth;
		}
	}
}
