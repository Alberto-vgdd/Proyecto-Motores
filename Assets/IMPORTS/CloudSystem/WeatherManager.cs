using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(Camera))]
public class WeatherManager : MonoBehaviour {

	//=== ATENCION =====
	// ESTE SCRIPT ES NECESARIO QUE ESTÉ EN LA CÁMARA, DE LO CONTRARIO
	// LOS SHADERS DE LAS NUBES NO FUNCIONARÁN.

	//== Nubes: creación y distribución

	//public GameObject cloudObject;
	private Texture2D outputTex;

	private void OnPreRender()
	{
		Shader.SetGlobalVector ("_CamPos", this.transform.position);
		Shader.SetGlobalVector ("_CamRight", this.transform.right);
		Shader.SetGlobalVector ("_CamUp", this.transform.up);
		Shader.SetGlobalVector ("_CamForward", this.transform.forward);
		Shader.SetGlobalFloat ("_AspectRatio", (float)Screen.width / (float)Screen.height);
		Shader.SetGlobalFloat ("_FieldOfView", Mathf.Tan (Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2f);

		//Shader.SetGlobalVector ("_CloudPos", cloudObject.transform.position);

	}

	[SerializeField]
	private Material material;

	[SerializeField, Range (10, 200)]
	private int width = 100;

	[SerializeField, Range (10, 200)]
	private int height = 100;

	[SerializeField]
	private Texture2D noiseOffsetTexture;


	private void Awake()
	{
		//== Anadimos comandos al buffer de comandos
		int lowResRenderTarget = Shader.PropertyToID ("_LowResRenderTarget");

		CommandBuffer cb = new CommandBuffer ();

		cb.GetTemporaryRT (lowResRenderTarget, this.width, this.height, 0, 
			FilterMode.Trilinear, RenderTextureFormat.ARGB32);

		cb.Blit (lowResRenderTarget, lowResRenderTarget, this.material);
		cb.Blit (lowResRenderTarget, BuiltinRenderTextureType.CameraTarget);

		cb.ReleaseTemporaryRT (lowResRenderTarget);

		this.GetComponent<Camera> ().AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		 // == Acabamos de asignar el comando


		//== ATENCION: ESTO ES NECESARIO PARA LAS NUBES.
		Shader.SetGlobalTexture ("_NoiseOffsets", this.noiseOffsetTexture);


		//Intento captura de textura

//		outputTex = new Texture2D (256, 256, TextureFormat.ARGB32, false);
//		RenderTexture buffer = new RenderTexture (512, 512, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
//
//		cloudObject = GameObject.Find ("SimpleClouds");
//
//		Graphics.Blit (cloudObject.GetComponent<MeshRenderer> ().materials[0].mainTexture, buffer, 
//					   cloudObject.GetComponent<MeshRenderer> ().materials[0], -1);
//
//		RenderTexture.active = buffer;
//
//		getShaderTexture ();

	}

	private void getShaderTexture()
	{
		outputTex.ReadPixels (new Rect (0, 0, 512, 512), 0, 0, false);
		outputTex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.dataPath + "/" + "intentoCaptura.png", outputTex.EncodeToPNG());

		print ("Se ha creado la imagen??");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



















}








