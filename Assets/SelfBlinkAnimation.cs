using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfBlinkAnimation : MonoBehaviour {

	public CanvasGroup CG;

	private float targetAlpha = 0;
	private float blinkSpeed = 3f;

	// Update is called once per frame
	void Update () {
		CG.alpha = Mathf.MoveTowards (CG.alpha, targetAlpha, Time.deltaTime * blinkSpeed);
		if (CG.alpha == targetAlpha) {
			if (targetAlpha == 0)
				targetAlpha = 1;
			else
				targetAlpha = 0;
		}
		
	}
	public void ResetBlinkingAnimation()
	{
		targetAlpha = 0;
		CG.alpha = 1;
		this.gameObject.SetActive (true);
	}
}
