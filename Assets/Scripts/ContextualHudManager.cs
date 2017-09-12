using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextualHudManager : MonoBehaviour {

	// Esta clase administra el interfaz contextual que aparece alrededor del jugador, para que la clase funcione correctamente se necesita:
	// - Referencias a cada parte del interfaz dinamico.
	// - Referencia al jugador.
	// - Instancia de StageData en la escena.

	public static ContextualHudManager currentInstance;

	private float tempDriftChain = 0;
	private float tempDriftMulti = 1;
	private float tempDriftMultiIncrease = 0;

	private float displayChainMultiplier = 1;
	private string extraDisplayString = "";
	private bool enableDriftDisplay = true;
	private bool displayDriftAsInteger = true;
	private bool lastDriftDegreeWasPositive = false;
	private bool currentDriftDegreeIsPositive = false;

	private const float minDriftChain = 100;

	[Header("CG Ref.")]
	public CanvasGroup HealthCG;
	public CanvasGroup DriftCG;
	public CanvasGroup TimeCG;
	[Header("Text Ref.")]
	public Text DriftText;
	public Text DriftMultiplier;
	public Text HpText;
	public Text TimeText;
	public Text IncrTimeText;
	public PlayerMovement pm;

	private float readedTime;

	[Header("Hud Parameters")]
	public Color hpMaxColor;
	public Color hpMinColor;


	void Awake () { if (currentInstance == null) {
			DontDestroyOnLoad (this.gameObject);
			currentInstance = this;
			//InitializeData ();
		}
		else {
			Destroy (this.gameObject);
		} }
	void Start () { 
		if (GlobalGameData.currentInstance.m_playerData_eventActive.GetScoreOnDriftMultiplier () > 0) {
			displayChainMultiplier = GlobalGameData.currentInstance.m_playerData_eventActive.GetScoreOnDriftMultiplier ();
			enableDriftDisplay = true;
			displayDriftAsInteger = true;
			extraDisplayString = " pts.";
		} else if (GlobalGameData.currentInstance.m_playerData_eventActive.GetBonusTimeOnDriftMultiplier () > 0) {
			displayChainMultiplier = GlobalGameData.currentInstance.m_playerData_eventActive.GetBonusTimeOnDriftMultiplier ();
			enableDriftDisplay = true;
			displayDriftAsInteger = false;
			extraDisplayString = " sec.";

		} else {
			enableDriftDisplay = false;
			displayDriftAsInteger = false;
			extraDisplayString = "";
		}
		UpdateDynHealth ();
	}

	void Update () {
		
		if (enableDriftDisplay) {
			UpdateDynDrift ();
		}

		if (pm.IsGrounded()) {
			if (StageData.currentData.playerHealth == 100)
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime / 2);
			else if (StageData.currentData.playerHealth < 50) {
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime*1.5f);
				if (HealthCG.alpha <= 0)
					HealthCG.alpha = 1;
			}
			else
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0.75f, Time.deltaTime/2);
		} else {
			HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime);
		}
	}

	// Administra el interfaz dinamico de drift.

	void UpdateDynDrift()
	{
		if (!pm.IsDrifting()) {
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 0, Time.deltaTime * 2);
			if (tempDriftChain > minDriftChain) 
				StageData.currentData.SendFinishedDrift (tempDriftChain, tempDriftMulti);
			tempDriftChain = 0;
			tempDriftMulti = 1;
			tempDriftMultiIncrease = 0;
			return;
		}
		tempDriftChain += Time.deltaTime * pm.GetCurrentSpeed() * Mathf.Abs(pm.GetDriftDegree());
		tempDriftMultiIncrease += Time.deltaTime * pm.GetCurrentSpeed() * 2f;
		if (tempDriftMultiIncrease > 10) {
			tempDriftMulti += 0.1f;
			tempDriftMultiIncrease = 0;
		}
		currentDriftDegreeIsPositive = pm.GetDriftDegree() > 0;
		if (tempDriftChain > 0 && currentDriftDegreeIsPositive != lastDriftDegreeWasPositive) {
			if (tempDriftChain > minDriftChain)
				StageData.currentData.SendFinishedDrift (tempDriftChain, tempDriftMulti);
			tempDriftChain = 0;
			tempDriftMulti = 1;
			tempDriftMultiIncrease = 0;
		}


		if (tempDriftChain > minDriftChain) {
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 1, Time.deltaTime * 2);
			float colorT = Mathf.Min (1, tempDriftChain / 3000);
			DriftText.color = Color.Lerp (Color.white, Color.red, colorT);
			DriftMultiplier.text = "X " + tempDriftMulti.ToString("F1");

			if (displayDriftAsInteger)
				DriftText.text = ((int)(tempDriftChain*displayChainMultiplier)).ToString() + extraDisplayString;
			else
				DriftText.text = (tempDriftChain*displayChainMultiplier).ToString("F1") + extraDisplayString;
			
		} else {
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 0, Time.deltaTime * 2);
		}
		lastDriftDegreeWasPositive = pm.GetDriftDegree () > 0;
	}

	// Administra el interfaz dinamico de salud.

	public void UpdateDynHealth()
	{
		HpText.color = Color.Lerp (hpMinColor, hpMaxColor, StageData.currentData.playerHealth / 100);
		HealthCG.alpha = 1;
		HpText.text = ((int) StageData.currentData.playerHealth).ToString();
	}

	public void ForceDriftEnd()
	{
		StageData.currentData.SendFinishedDrift (tempDriftChain, tempDriftMulti);
	}
}
