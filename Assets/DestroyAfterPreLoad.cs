using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterPreLoad : MonoBehaviour {

	void Start () {
		Destroy(this.gameObject, (0.1f));
	}

}
