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
		StopRecording (false);
	}
	public bool IsRecording()
	{
		return recording;
	}
	public void StartRecording()
	{
		if (recording)
			return;
		recording = true;
		recordedData = new GhostReplayData (recordingInterval, GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed(), GlobalGameData.currentInstance.m_playerData_eventActive.GetGamemode());
		StartCoroutine ("RecordData");
		print ("[REPLAY] Recording ghost data.");
	}
	public void StopRecording(bool valid)
	{
		if (!recording)
			return;
		recording = false;
		if (!valid)
			return;

		print ("[REPLAY] Ended ghost data recording.");
		if (GlobalGameData.currentInstance.m_playerData_eventActive.IsObjectiveTypeScore ()) {
			recordedData.SetGhostScore (StageData.currentData.GetEventScore (), !GlobalGameData.currentInstance.m_playerData_eventActive.IsObjectiveTypeScore ());
		} else {
			recordedData.SetGhostScore (StageData.currentData.GetTimePassedValue(), !GlobalGameData.currentInstance.m_playerData_eventActive.IsObjectiveTypeScore ());
		}
	
		GlobalGameData.currentInstance.SetPlayerGhostPB(recordedData);
	}
}
