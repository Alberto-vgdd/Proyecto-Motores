using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarModel : MonoBehaviour
 {
	 public static CarModel currentInstance;
	 public MeshRenderer chasis;
	 public MeshRenderer faros;
	 
	void Awake () {
		if (currentInstance == null)
		{
			DontDestroyOnLoad(this.gameObject);
			currentInstance = this;
		}
		else
		{
			currentInstance.transform.SetParent(this.transform.parent);
			currentInstance.transform.position = this.transform.position;
			currentInstance.transform.rotation = this.transform.rotation;
			currentInstance.transform.localScale = this.transform.localScale;
			Destroy(this.gameObject);
		}
	}

	public void ChangeMaterial(Material material)
	{
		chasis.material = material;
		Material[] materials = faros.materials;
		materials[1] = material;
		faros.materials = materials;
	}
}
