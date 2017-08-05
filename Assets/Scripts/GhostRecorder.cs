using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour {

	public static GhostRecorder currentInstance;

	public GameObject target;

	private GhostReplayData recordedData;

	private bool recording;
	private float recordingInterval = 0.1f;

	void Awake ()
	{
		currentInstance = this;
	}

	IEnumerator RecordData()
	{
		float t = 0;
		recordedData.AddRecording (target);
		while (!recordedData.IsFull() && recording)
		{
			while (t < 1) {
				yield return new WaitForFixedUpdate();
				t += Time.fixedDeltaTime / recordingInterval;
			}
			recordedData.AddRecording (target);
			t -= 1;
		}
		StopRecording ();
	}
	public bool IsRecording()
	{
		return recording;
	}
	public void StartRecording()
	{
		recording = true;
		recordedData = new GhostReplayData (recordingInterval, GlobalGameData.currentInstance.eventActive.GetSeed(), GlobalGameData.currentInstance.eventActive.GetGamemode());
		StartCoroutine ("RecordData");
		print ("[REPLAY] Recording ghost data.");
	}
	public void StopRecording()
	{
		recording = false;
		GlobalGameData.currentInstance.test = recordedData;
		print ("[REPLAY] Ended ghost data recording.");
	}
}
