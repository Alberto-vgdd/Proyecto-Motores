using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GhostReplayData {

	private List<V3Serializer> recordedPositions;
	private List<QTSerializer> recordedRotations;

	private int recordedAt_seed;
	private EventData.Gamemode recordedAt_gamemode;
	private float scoreRecorded;
	private bool scoreRecordedIsTime;
	private string ghostName;
	private int GhostSkinID;
	private int GhostModelID;

	private float recordingInterval;
	private const int MAX_ALLOWED_RECORDINGS = 5000;

	public GhostReplayData (float interval, int seed, EventData.Gamemode gmode, string playername, int S_ID, int M_ID)
	{
		recordingInterval = interval;
		recordedAt_seed = seed;
		recordedAt_gamemode = gmode;
		ghostName = playername;
		GhostSkinID = S_ID;
		GhostModelID = M_ID;
		recordedPositions = new List<V3Serializer> ();
		recordedRotations = new List<QTSerializer> ();
	}
	public Vector3 GetPositionAt(int index)
	{
		return recordedPositions [index].Get();
	}
	public Quaternion GetRotationAt(int index)
	{
		return recordedRotations [index].Get();
	}
	public void AddRecording(GameObject obj)
	{
		if (recordedPositions.Count >= MAX_ALLOWED_RECORDINGS)
			return;
		recordedPositions.Add (new V3Serializer(obj.transform.localPosition));
		recordedRotations.Add (new QTSerializer(obj.transform.localRotation));
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
	public EventData.Gamemode GetRecordedAtGamemode()
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
	public int GetGhostSkinID()
	{
		return GhostSkinID;
	}
	public int GetGhostModelID()
	{
		return GhostModelID;
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
