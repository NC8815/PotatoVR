using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlowShadow : MonoBehaviour {

	public Color GlowColor = Color.black;
	public float LerpFactor = 10;
	[Range (0f,1f)]
	[SerializeField]
	private float GlowIntensity = 0;

	private List<Material> _materials = new List<Material> ();
	[SerializeField]
	private Color _currentColor;
	private Color _targetColor;

	public Vector3 TargetForward;
	public Vector3 TargetUp;
	public Vector3 TargetLocation;

	private float forAcc;
	private float upAcc;
	private float locAcc;

	float degThreshold = 10;
	float minProximity = 0.1f;

	public float accuracy {
		get {
			forAcc = Mathf.Clamp01((Vector3.Dot(transform.forward.normalized,TargetForward.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			upAcc = Mathf.Clamp01((Vector3.Dot(transform.up.normalized,TargetUp.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			locAcc = Mathf.Clamp01 ((minProximity - Vector3.Distance (transform.position, TargetLocation)) / minProximity);
			return forAcc * upAcc * locAcc;
		}
	}

	public void SetTarget(Vector3 forward, Vector3 up, Vector3 position){
		TargetForward = forward;
		TargetUp = up;
		TargetLocation = position;
	}

	void Awake(){
		foreach (var renderer in GetComponentsInChildren<Renderer>()) {
			_materials.AddRange (renderer.materials);
		}
		EventManager.StartListening("SpawnNextPiece",ProgressPuzzle);
	}

	void Update(){
		GlowIntensity = accuracy;
		if (accuracy < 0.95f) {
			_targetColor = Color.Lerp (Color.black, GlowColor, GlowIntensity);
			_currentColor = Color.Lerp (_currentColor, _targetColor, Time.deltaTime * LerpFactor);

			for (int i = 0; i < _materials.Count; i++) {
				_materials [i].SetColor ("_GlowColor", _currentColor);
			}
		} else {
			EventManager.TriggerEvent ("SpawnNextPiece");
		}
	}

	public void ProgressPuzzle(){
		Instantiate (Resources.Load<GameObject> ("Prefabs/DemoCube"));
		Destroy (transform.parent.gameObject);
		EventManager.StopListening ("SpawnNextPiece", ProgressPuzzle);
	}

}
