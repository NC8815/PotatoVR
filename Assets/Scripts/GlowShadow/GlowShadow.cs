using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using System.Linq;

//[RequireComponent(typeof(PuzzlePiece))]
[System.Serializable]
public class GlowShadow : MonoBehaviour {

	public Color GlowColor = Color.black;
	float LerpFactor = 10;
	private float GlowIntensity = 0;

	private List<Material> _materials = new List<Material> ();
	//[SerializeField]
	private Color _currentColor;
	private Color _targetColor;

//	public Vector3 TargetForward;
//	public Vector3 TargetUp;
//	public Vector3 TargetLocation;

	public Texture2D TargetShadow;
	Color32[] _targetShadow;

//	private float forAcc;
//	private float upAcc;
//	private float locAcc;

//	float degThreshold = 20;
//	float minProximity = 1f;

	[SerializeField]
	float accuracyThreshold = 0.95f;

	[Range(1,3999)]
	[SerializeField]
	int pulseStrength = 3000;
//	int weakPulse = 100;

//	float lastGlow = 0;

	//bool isHeld = false;
	public Hand holdingHand { get; private set; }

	public GameObject LightSource;

	//Utility variables
	public ShadowSample Sampler;

	private bool animating = false;

	PuzzlePiece puzzle { get { return GetComponent<PuzzlePiece> (); } }

	public float accuracy {
		get {
//			forAcc = Mathf.Clamp01((Vector3.Dot(transform.forward.normalized,TargetForward.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
//			upAcc = Mathf.Clamp01((Vector3.Dot(transform.up.normalized,TargetUp.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
//			locAcc = Mathf.Clamp01 ((minProximity - Vector3.Distance (transform.position, TargetLocation)) / minProximity);
//			return Mathf.Sqrt(locAcc) * forAcc * forAcc * upAcc * upAcc;
			float angularAccuracy = LightSource == null ? 1 : Vector3.Dot(LightSource.transform.forward,(transform.position - LightSource.transform.position).normalized);
			return _targetShadow == null ? 0 : ShadowSample.CurrentImage.Compare(_targetShadow) * angularAccuracy * angularAccuracy;

		}
	}

	void Awake(){
		foreach (var renderer in GetComponentsInChildren<Renderer>()) {
			_materials.AddRange (renderer.materials);
		}
		if(puzzle != null)
			puzzle.AddListeners();
	}

	void HapticFeedback(float acc){
		if (holdingHand != null && holdingHand.controller != null)
			holdingHand.controller.TriggerHapticPulse ((ushort)Mathf.Clamp (acc * pulseStrength, 0, 3999));
	}

	void UpdateGlow ()
	{
		_targetColor = Color.Lerp (Color.black, GlowColor, GlowIntensity);
		if (_currentColor != _targetColor) {
			_currentColor = Color.Lerp (_currentColor, _targetColor, Time.deltaTime * LerpFactor);
			for (int i = 0; i < _materials.Count; i++) {
				_materials [i].SetColor ("_GlowColor", _currentColor);
			}
		}
	}

	void Update(){
		GlowIntensity = accuracy;
		if (!animating) {
			if (accuracy > accuracyThreshold)
				StartCoroutine (Despawn());
			else
				UpdateGlow ();
		};
	}

	public void UpdateShadow(){
		_targetShadow = TargetShadow.GetPixels32 ();
	}

	private void HandAttachedUpdate(){
		GlowIntensity = accuracy;
		HapticFeedback (GlowIntensity);

//		if (!animating) {
//			if (accuracy > accuracyThreshold)
//				StartCoroutine (Despawn());
//			else
//				UpdateGlow ();
//		}
	}

	IEnumerator Despawn(){
		animating = true;
		holdingHand.DetachObject (gameObject);
		enabled = false;
		GlowIntensity = 1;
		while (_currentColor != GlowColor) {
			UpdateGlow ();
			yield return null;
		}
		puzzle.ProgressPuzzle ();
	}

	void OnDestroy(){
		puzzle.RemoveListeners ();
	}

	public void OnHold(Hand hand){
		//isHeld = true;
		enabled = true;
		holdingHand = hand;
	}

	public void OnDrop(Hand hand){
		//isHeld = false;
		holdingHand = null;
	}
}
