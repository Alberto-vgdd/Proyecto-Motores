using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuNotificationManager : MonoBehaviour {

	public static MainMenuNotificationManager currentInstance;

	public CanvasGroup globalCG;
	public CanvasGroup headerCG;
	public CanvasGroup bodyCG;
	public Text headerInfo;
	public Text bodyInfo;

	private List<MainMenuNotificationData> notifications;

	private bool animationInProcess;
	private Vector3 headerInitialPos;
	private Vector3 bodyInitialPos;

	void Awake ()
	{
		currentInstance = this;
		notifications = new List<MainMenuNotificationData> ();
	}

	IEnumerator ShowNotification()
	{
		headerInitialPos = headerCG.transform.localPosition;
		bodyInitialPos = bodyCG.transform.localPosition;

		globalCG.gameObject.SetActive (true);
		animationInProcess = true;
		globalCG.alpha = headerCG.alpha = bodyCG.alpha = 0f;
		headerInfo.text = notifications [0].GetMessage ();
		bodyInfo.text = notifications [0].GetSubMessage ();
		notifications.RemoveAt (0);

		float t = 0f;
		float animSpeed = 8f;


		while (t < 2f) {
			t += Time.deltaTime * animSpeed;

			globalCG.alpha = Mathf.Clamp (t, 0, 1);
			headerCG.alpha = Mathf.Clamp (t, 0, 1);
			bodyCG.alpha = Mathf.Clamp (t - 1, 0, 1);
			headerCG.transform.localPosition = headerInitialPos + Vector3.left * (1- Mathf.Clamp (t, 0, 1)) * 50;
			bodyCG.transform.localPosition = bodyInitialPos + Vector3.left * (1-Mathf.Clamp (t-1, 0, 1)) * 50;
			yield return null;
		}
		animationInProcess = false;
	}
	IEnumerator CloseNotification()
	{
		float t = 1f;
		float animSpeed = 10f;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			globalCG.alpha = headerCG.alpha = bodyCG.alpha = t;
			yield return null;
		}
		globalCG.gameObject.SetActive (false);
		CheckNotifications ();
	
	}
	private void CheckNotifications()
	{
		if (notifications.Count > 0 && !animationInProcess && !IsOpen()) {
			animationInProcess = true;
			StartCoroutine ("ShowNotification");
		}
	}
	public void AddNotification(MainMenuNotificationData data)
	{
		notifications.Add (data);
		CheckNotifications ();
	}
	public void OnOkButtonClicked()
	{
		if (animationInProcess)
			return;
		StartCoroutine ("CloseNotification");
	}
	public bool IsOpen()
	{
		return globalCG.gameObject.activeInHierarchy;
	}
}
