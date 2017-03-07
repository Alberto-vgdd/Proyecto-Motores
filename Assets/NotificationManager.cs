using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour {

	public static NotificationManager currentInstance;

	public Text notificationUI;

	private List<GameNotification> notifications;
	private float notificationTimer;
	public float notificationLifetime;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		notifications = new List<GameNotification> ();
		notificationTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (notifications.Count > 0) {
			if (notificationTimer == 0)
				SetNotification ();
			notificationTimer += Time.deltaTime;
			if (notificationTimer > notificationLifetime) {
				notificationTimer = 0;
				notifications.RemoveAt (0);
			}
		} else {
			ClearNotification ();
		}
		
	}
	void SetNotification()
	{
		notificationUI.text = notifications [0].infoText;
		notificationUI.color = notifications [0].infoColor;
		notificationUI.fontSize = notifications [0].infoSize;
		notificationUI.GetComponent<Animator> ().SetTrigger ("TriggerAnim");
	}
	void ClearNotification()
	{
		notificationUI.text = "";
	}
	public void AddNotification(GameNotification notif)
	{
		notifications.Add (notif);
	}
}
