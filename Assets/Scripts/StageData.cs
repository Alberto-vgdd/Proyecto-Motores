using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour {

	public static StageData currentData;

	public GameObject playerObj;
	public float playerHealth;
	public float remainingSec;
	public int nodesCrossed;

	public Text timeRemainingInfo;

	void Awake () { currentData = this; }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (remainingSec > 5) {
			timeRemainingInfo.text = ((int)remainingSec).ToString();
		} else {
			timeRemainingInfo.text = remainingSec.ToString("N2");
		}

		remainingSec = Mathf.MoveTowards (remainingSec, 0, Time.deltaTime);

		
	}
	public void CrossCheckPoint()
	{
		remainingSec += 5;
	}
}
