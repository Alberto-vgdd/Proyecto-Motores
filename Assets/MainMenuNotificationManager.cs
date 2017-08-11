using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNotificationManager : MonoBehaviour {

	private List<MainMenuNotificationData> notifications;

	// Use this for initialization
	void Start () {
		notifications = new List<MainMenuNotificationData> ();
	}

	void Update () {
		
	}

	public void AddNotification(MainMenuNotificationData data)
	{
		notifications.Add (data);
	}
}
