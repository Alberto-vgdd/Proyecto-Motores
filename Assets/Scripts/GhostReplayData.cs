using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostReplayData {

	private List<Vector3> recordedPositions;
	private List<Quaternion> recordedRotations;

	private float recordingInterval;
	private const int MAX_ALLOWED_RECORDINGS = 5000;

	public GhostReplayData (float interval)
	{
		recordingInterval = interval;
		recordedPositions = new List<Vector3> ();
		recordedRotations = new List<Quaternion> ();
	}
	public Vector3 GetPositionAt(int index)
	{
		return recordedPositions [index];
	}
	public Quaternion GetRotationAt(int index)
	{
		return recordedRotations [index];
	}
	public void AddRecording(GameObject obj)
	{
		if (recordedPositions.Count >= MAX_ALLOWED_RECORDINGS)
			return;
		recordedPositions.Add (obj.transform.localPosition);
		recordedRotations.Add (obj.transform.localRotation);
	}
	public float GetRecordingInterval()
	{
		return recordingInterval;
	}
	public bool IsFull()
	{
		return recordedPositions.Count >= MAX_ALLOWED_RECORDINGS;
	}
	public int GetReplayLenght()
	{
		return recordedPositions.Count;
	}

}
