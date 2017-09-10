using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenTileSpawner : MonoBehaviour {

	public GameObject roadTilePrefab;
	public List<Transform> wheelsToAnimate;
	private List<GameObject> roadTiles;
	private int totalTiles = 20;

	private bool animated = true;

	// Use this for initialization
	void Start () {
		roadTiles = new List<GameObject> ();
		GameObject lastCreatedTile;
		for (int i = 0; i < totalTiles; i++) {
			lastCreatedTile = Instantiate (roadTilePrefab, transform.position + Vector3.left * i * 5, Quaternion.identity) as GameObject;
			roadTiles.Add (lastCreatedTile);
		}
		StartCoroutine ("MoveTerrain");
		StartCoroutine ("AnimateWheels");
	}

	IEnumerator AnimateWheels()
	{
		float animspeed = 1000f;
		while (animated) {
			for (int i = 0; i < wheelsToAnimate.Count; i++) {
				wheelsToAnimate [i].Rotate (animspeed * Time.deltaTime, 0, 0);
			}
			yield return null;
		}
	}
	IEnumerator MoveTerrain()
	{
		float animSpeed = 12.5f;
		float maxOffset = 15f;
		while (animated) {
			for (int i = 0; i < totalTiles; i++) {
				roadTiles [i].transform.Translate (Vector3.left * -animSpeed * Time.deltaTime);
				if (roadTiles [i].transform.position.x > maxOffset) {
					roadTiles [i].transform.Translate (Vector3.left * totalTiles * 5);
				}
			}
			yield return null;
		}
	}
}
