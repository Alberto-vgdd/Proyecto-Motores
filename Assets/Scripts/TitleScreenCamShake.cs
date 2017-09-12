using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCamShake : MonoBehaviour {

	private bool shaking = true;
	private Vector3 basePos;

	void Start () {
		basePos = transform.position;
		StartCoroutine ("ShakeAnimation");
	}
	
	IEnumerator ShakeAnimation()
	{
		 float shakeStrenght = 0.005f;
		 float shakeSpeedX = 0.001f;
		 float shakeSpeedY = 0.005f;

		float currentShakeX = 0;
		float currentShakeY = 0;
		float targetShakeX = 0;
		float targetShakeY = 0;

		int inverseX = 1;
		int inverseY = 1;

		targetShakeX = shakeStrenght * inverseX;
		targetShakeY = shakeStrenght * inverseY;
		while (shaking) {
			currentShakeX = Mathf.MoveTowards (currentShakeX, targetShakeX, Time.deltaTime * shakeSpeedX);
			if (currentShakeX == targetShakeX) {
				inverseX *= -1;
				targetShakeX = shakeStrenght * inverseX;
			}

			currentShakeY = Mathf.MoveTowards (currentShakeY, targetShakeY, Time.deltaTime * shakeSpeedY);
			if (currentShakeY == targetShakeY) {
				inverseY *= -1;
				targetShakeY = shakeStrenght * inverseY;
			}

			transform.position = basePos + Vector3.up * currentShakeX + Vector3.left * currentShakeY;
			yield return null;
		}
	}
}
