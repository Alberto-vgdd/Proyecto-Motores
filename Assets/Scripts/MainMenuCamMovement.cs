using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuCamMovement : MonoBehaviour {

	public static MainMenuCamMovement currentInstance;

	public GameObject cam;
	public Transform camFocusNormalView;
	public Transform camFocusGarageView;
	public CanvasGroup fadeCG;

	public EventSystem es;

	public List<GameObject> camPositions;

	private bool camInCarViewMode = false;

	private float timeWithNoDragInput = 0f;
	private float camAutoTurningTime = 2.5f;
	private float camAutoTurningSpeed = 4.5f;
	private float camDragStrenght = 100f;

	void Awake ()
	{
		currentInstance = this;
	}

	void Start () {
		StartCoroutine ("camMove1");
	}
	
	public void SwitchToCarView(bool arg)
	{
		camInCarViewMode = arg;
		if (arg) {
			timeWithNoDragInput = 0f;
		}
	}
	IEnumerator camMove1()
	{
		cam.transform.position = camPositions [0].transform.position;
		cam.transform.rotation = camPositions [0].transform.rotation;

		float t = 0;
		float tmax = 10f;
		float camSpeed = 0.5f;

		while (t < tmax) {
			if (t < 1f) {
				fadeCG.alpha = 1 - t;
			} else if (t > tmax-1) {
				fadeCG.alpha = 1-(tmax-t);
			} else {
				fadeCG.alpha = 0;
			}

			t += Time.deltaTime;
			cam.transform.Translate (Time.deltaTime*camSpeed, 0, 0);

			if (camInCarViewMode) {
				StartCoroutine ("CarViewMode");
				yield break;
			}

			yield return null;
		}
		StartCoroutine ("camMove2");
	}
	IEnumerator camMove2()
	{
		cam.transform.position = camPositions [1].transform.position;
		cam.transform.rotation = camPositions [1].transform.rotation;

		float t = 0;
		float tmax = 10f;
		float camSpeed = 0.75f;

		while (t < tmax) {
			if (t < 1f) {
				fadeCG.alpha = 1 - t;
			} else if (t > tmax-1) {
				fadeCG.alpha = 1-(tmax-t);
			} else {
				fadeCG.alpha = 0;
			}

			t += Time.deltaTime;
			cam.transform.Translate (0, camSpeed * Time.deltaTime, 0);

			if (camInCarViewMode) {
				StartCoroutine ("CarViewMode");
				yield break;
			}

			yield return null;
		}
		StartCoroutine ("camMove3");
	}
	IEnumerator camMove3()
	{
		cam.transform.position = camPositions [2].transform.position;
		cam.transform.rotation = camPositions [2].transform.rotation;

		float t = 0;
		float tmax = 10f;
		float camSpeed = 0.5f;

		while (t < tmax) {
			if (t < 1f) {
				fadeCG.alpha = 1 - t;
			} else if (t > tmax-1) {
				fadeCG.alpha = 1-(tmax-t);
			} else {
				fadeCG.alpha = 0;
			}

			t += Time.deltaTime;
			cam.transform.Translate (Time.deltaTime*camSpeed, 0, 0);

			if (camInCarViewMode) {
				StartCoroutine ("CarViewMode");
				yield break;
			}

			yield return null;
		}
		StartCoroutine ("camMove4");
	}
	IEnumerator camMove4()
	{
		cam.transform.position = camPositions [3].transform.position;
		cam.transform.rotation = camPositions [3].transform.rotation;

		float t = 0;
		float tmax = 10f;
		float camSpeed = 0.5f;

		while (t < tmax) {
			if (t < 1f) {
				fadeCG.alpha = 1 - t;
			} else if (t > tmax-1) {
				fadeCG.alpha = 1-(tmax-t);
			} else {
				fadeCG.alpha = 0;
			}

			t += Time.deltaTime;
			cam.transform.Translate (Time.deltaTime*camSpeed, 0, 0);

			if (camInCarViewMode) {
				StartCoroutine ("CarViewMode");
				yield break;
			}

			yield return null;
		}
		StartCoroutine ("camMove5");
	}
	IEnumerator camMove5()
	{
		cam.transform.position = camPositions [4].transform.position;
		cam.transform.rotation = camPositions [4].transform.rotation;

		float t = 0;
		float tmax = 10f;
		float camSpeed = 0.5f;

		while (t < tmax) {
			if (t < 1f) {
				fadeCG.alpha = 1 - t;
			} else if (t > tmax-1) {
				fadeCG.alpha = 1-(tmax-t);
			} else {
				fadeCG.alpha = 0;
			}

			t += Time.deltaTime;
			cam.transform.Translate (0, 0, -camSpeed * Time.deltaTime);

			if (camInCarViewMode) {
				StartCoroutine ("CarViewMode");
				yield break;
			}

			yield return null;
		}
		StartCoroutine ("camMove1");
	}
	IEnumerator CarViewMode()
	{
		float fadespeed = 3.5f;

		while (fadeCG.alpha < 1) {
			fadeCG.alpha = Mathf.MoveTowards (fadeCG.alpha, 1, Time.deltaTime * fadespeed);
			yield return null;
		}
			
		cam.transform.position = camPositions [5].transform.position;
		cam.transform.LookAt (camFocusGarageView);

		while (fadeCG.alpha > 0) {
			fadeCG.alpha = Mathf.MoveTowards (fadeCG.alpha, 0, Time.deltaTime * fadespeed);
			yield return null;
		}
		while (camInCarViewMode) {
			if (Input.GetMouseButton (0) && !es.IsPointerOverGameObject ()) {
				cam.transform.RotateAround (camFocusNormalView.position, new Vector3 (0, 1, 0), Input.GetAxis ("Mouse X") * camDragStrenght * Time.deltaTime); 
				cam.transform.LookAt (camFocusGarageView);
				timeWithNoDragInput = 0;
			} else {
				if (timeWithNoDragInput > camAutoTurningTime) {
					cam.transform.RotateAround (camFocusNormalView.position, new Vector3 (0, 1, 0), camAutoTurningSpeed * Time.deltaTime); 
				} else {
					timeWithNoDragInput += Time.deltaTime;
				}


			}

			yield return null;
		}
		while (fadeCG.alpha < 1) {
			fadeCG.alpha = Mathf.MoveTowards (fadeCG.alpha, 1, Time.deltaTime * fadespeed);
			yield return null;
		}
		StartCoroutine ("camMove1");
	}
}
