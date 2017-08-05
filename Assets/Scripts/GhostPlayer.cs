using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour {

	public static GhostPlayer currentInstance;

	public GameObject target;

	private GhostReplayData replayData;
	private bool playing = false;

	// Use this for initialization
	void Awake () {
		currentInstance = this;
		replayData = GlobalGameData.currentInstance.test;
	}
	public void StartPlaying()
	{
		if (replayData == null)
			return;
		playing = true;
		StartCoroutine ("PlayGhost");
	}
	IEnumerator PlayGhost()
	{
		int index = 0;
		int maxIndex = replayData.GetReplayLenght () - 2;
		float t = 0;
		float interval = replayData.GetRecordingInterval();

		Vector3 lerpPos0;
		Vector3 lerpPos1;
		Quaternion lerpRot0;
		Quaternion lerpRot1;

		target.gameObject.SetActive (true);

		target.transform.localPosition = replayData.GetPositionAt (0);
		target.transform.localRotation = replayData.GetRotationAt (0);
		while (index < maxIndex  && playing) {
			
			lerpPos0 = replayData.GetPositionAt (index);
			lerpPos1 = replayData.GetPositionAt (index+1);
			lerpRot0 = replayData.GetRotationAt (index);
			lerpRot1 = replayData.GetRotationAt (index+1);

			while (t < 1) {
				yield return new WaitForFixedUpdate();
				t += (Time.fixedDeltaTime / interval);
				if (t > 1) {
					target.transform.localPosition = Vector3.Lerp (replayData.GetPositionAt(index+1), replayData.GetPositionAt(index+2), t-1);
					target.transform.localRotation = Quaternion.Lerp (replayData.GetRotationAt(index+1), replayData.GetRotationAt(index+2), t-1);
				} else {
					target.transform.localPosition = Vector3.Lerp (lerpPos0, lerpPos1, t);
					target.transform.localRotation = Quaternion.Lerp (lerpRot0, lerpRot1, t);
				}
//				if (t > 1) {
//					target.transform.localPosition = Vector3.Lerp (replayData.GetPositionAt(index+1), replayData.GetPositionAt(index+2), t-1);
//					target.transform.localRotation = Quaternion.Lerp (replayData.GetRotationAt(index+1), replayData.GetRotationAt(index+2), t-1);
//				} else {
//					target.transform.localPosition = Vector3.Lerp (lerpPos0, lerpPos1, t);
//					target.transform.localRotation = Quaternion.Lerp (lerpRot0, lerpRot1, t);
//				}
			}
			index++;
			t -= 1;
		}
		playing = false;
	}
	public void StopPlaying()
	{
		playing = false;
	}
}
