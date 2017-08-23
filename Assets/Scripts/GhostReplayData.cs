using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostReplayData {

	private List<Vector3> recordedPositions;
	private List<Quaternion> recordedRotations;

	private int recordedAt_seed;
	private int recordedAt_gamemode;
	private float scoreRecorded;
	private bool scoreRecordedIsTime;
	private string ghostName;

	private float recordingInterval;
	private const int MAX_ALLOWED_RECORDINGS = 5000;

	public GhostReplayData (float interval, int seed, int gamemode)
	{
		recordingInterval = interval;
		recordedAt_seed = seed;
		recordedAt_gamemode = gamemode;
		ghostName = "[TEST] Ghost";
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
	public int GetRecordedAtSeed()
	{
		return recordedAt_seed;
	}
	public int GetRecordedAtGamemode()
	{
		return recordedAt_gamemode;
	}
	public string GetGhostName()
	{
		return ghostName;
	}
	public void SetGhostScore(float score, bool isTime)
	{
		scoreRecorded = score;
		scoreRecordedIsTime = isTime;
	}
	public float GetScoreRecorded()
	{
		return scoreRecorded;
	}
	public bool GetScoreRecordedIsTime()
	{
		return scoreRecordedIsTime;
	}
	public string GetScoreRecordedString()
	{
		if (scoreRecordedIsTime) {
			int seconds = (int)(scoreRecorded % 60);
			int minutes = (int)(scoreRecorded / 60);
			int milisec = (int)Mathf.Floor(scoreRecorded * 100 % 100);
			return minutes + ":" + seconds.ToString ("D2") + ":" + milisec.ToString("D2");
		} else {
			return ((int)scoreRecorded).ToString ();
		}
	}

}
