using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour {

	public static StageData currentData;

	public GameObject playerObj;
	public float playerHealth;
	public float remainingSec;

	public Text timeRemainingInfo;

	void Awake () { currentData = this; }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeRemainingInfo.text = ((int)remainingSec / 60).ToString () + ":" + ((int)remainingSec % 60).ToString ();;
		remainingSec = Mathf.MoveTowards(remainingSec, 0, Time.deltaTime);

		
	}
	public void CrossCheckPoint()
	{
		remainingSec = 15;
	}
}
