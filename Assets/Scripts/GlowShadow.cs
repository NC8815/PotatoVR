using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using System.Linq;

//[RequireComponent(typeof(PuzzlePiece))]
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

	float degThreshold = 20;
	float minProximity = 1f;

	[SerializeField]
	float accuracyThreshold = 0.6f;

	public int strongPulse = 3000;
	public int weakPulse = 100;

	float lastGlow = 0;

	bool isHeld = false;
	Hand holdingHand;

	PuzzlePiece puzzle { get { return GetComponent<PuzzlePiece> (); } }

	public float accuracy {
		get {
			forAcc = Mathf.Clamp01((Vector3.Dot(transform.forward.normalized,TargetForward.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			upAcc = Mathf.Clamp01((Vector3.Dot(transform.up.normalized,TargetUp.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			locAcc = Mathf.Clamp01 ((minProximity - Vector3.Distance (transform.position, TargetLocation)) / minProximity);
			return Mathf.Sqrt(locAcc) * forAcc * forAcc * upAcc * upAcc;
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
		//EventManager.StartListening("SpawnNextPiece",ProgressPuzzle);
		if(puzzle != null)
			puzzle.AddListeners();
	}

	void HapticFeedback(float acc){
//		if (lastGlow < acc) {
//			if (holdingHand != null && holdingHand.controller != null)
//				holdingHand.controller.TriggerHapticPulse ((ushort)weakPulse);
//		} else {
//			if (holdingHand != null && holdingHand.controller != null)
//				holdingHand.controller.TriggerHapticPulse ((ushort)strongPulse);
//		}
		if (holdingHand != null && holdingHand.controller != null)
			holdingHand.controller.TriggerHapticPulse ((ushort)Mathf.Clamp (acc * strongPulse, 0, 3999));
		lastGlow = acc;
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

		lastGlow = GlowIntensity;
	}

	void Update(){
		GlowIntensity = accuracy;
		if (isHeld)
			HapticFeedback (GlowIntensity);
		if (accuracy < accuracyThreshold) {
			GlowIntensity = accuracy;
			UpdateGlow ();
		} else {
			GlowIntensity = 1;
			UpdateGlow ();
			if (puzzle != null)
				puzzle.ProgressPuzzle ();
		}
	}
		
	private void HandAttachedUpdate(){
		Update ();
	}

	public void OnHold(Hand hand){
		isHeld = true;
		holdingHand = hand;
	}

	public void OnDrop(Hand hand){
		isHeld = false;
		holdingHand = null;
	}
}
