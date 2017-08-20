using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour {

	// Manager de notificaciones, debe ser colocado en la escena jugable, y se puede llamar atraves de la variable estatica currentInstance,
	// las notificaciones deben de ser añadidas atraves de la funcion AddNotification(), dando como parametro un GameNotification

	public static NotificationManager currentInstance;					// Referencia estatica

	public Text notificationUI;											// Texto del interfaz

	private List<GameNotification> notifications;						// Lista de notificaciones en cola
	private float notificationTimer;									// Tiempo que la notificacion lleva activa (temp)
	private float notificationLifetime = 1.55f;							// [AJUSTABLE] Tiempo de vida de cada notificacion

	void Awake ()
	{
		currentInstance = this;
		notifications = new List<GameNotification> ();
		notificationTimer = 0;
	}

	void Start () {
	}

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
