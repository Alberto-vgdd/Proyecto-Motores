using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectionAnimation : MonoBehaviour {

	public GameObject[] cars;
	public int selected;
	public float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			selected--;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			selected++;
		}
		selected = Mathf.Clamp (selected, 0, cars.Length-1);
		transform.position = Vector3.MoveTowards(transform.position,Vector3.up * 0.5f + Vector3.left * (selected-1) * 8, Time.deltaTime * 50);
		cars [selected].transform.Rotate (Vector3.up * Time.deltaTime * rotationSpeed);

		
	}
}
