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

	public Texture2D TargetShadow;
	Color32[] _targetShadow;

	[SerializeField]
	float accuracyTarget = 0.95f;

	[SerializeField]
	[Range(0,0.99f)]
	float minimumAccuracy = 0.0f;

	[Range(1,3999)]
	[SerializeField]
	int pulseStrength = 3000;

//	[SerializeField]
//	float ACC;

	bool done = false;

	public Hand holdingHand { get; private set; }

	public GameObject LightSource;

	public ShadowSample Sampler;

//	private bool animating = false;

	PuzzlePiece puzzle { get { return GetComponent<PuzzlePiece> (); } }

	public float accuracy {
		get {
			//float angularAccuracy = LightSource == null ? 1 : Vector3.Dot(LightSource.transform.forward,(transform.position - LightSource.transform.position).normalized);
			float angularOffset = Vector3.Angle(LightSource.transform.forward,transform.position - LightSource.transform.position);
			float angularAccuracy = Mathf.Clamp01 ((LightSource.GetComponent<Light> ().spotAngle - 2 * angularOffset) / LightSource.GetComponent<Light> ().spotAngle);
//			ACC = angularOffset;
			return _targetShadow == null ? 0 : ShadowSample.CurrentImage.Compare (_targetShadow);// * angularAccuracy * angularAccuracy;
		}
	}

	void Awake(){
		foreach (var renderer in GetComponentsInChildren<Renderer>()) {
			_materials.AddRange (renderer.materials);
		}
		if(puzzle != null)
			puzzle.AddListeners();
	}

//	void Start(){
//		AudioController.instance.PlaySingle (SpawnClip,GetComponent<AudioSource>());
//	}

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
		GlowIntensity = (accuracy - minimumAccuracy)/(1 - minimumAccuracy);
		UpdateGlow ();
		if (accuracy > accuracyTarget && !done) {
			StartCoroutine (Despawn ());
			done = true;
		}
//		if (!animating) {
//			if (accuracy > accuracyThreshold)
//				StartCoroutine (Despawn());
//			else
//				UpdateGlow ();
//		};
	}

	IEnumerator Despawn(){
		//animating = true;
		GlowIntensity = 1;

		if(holdingHand != null)
			holdingHand.DetachObject (gameObject); //Drop the object
		enabled = false; //Don't update anymore

		while (_currentColor != GlowColor) {
			UpdateGlow ();
			yield return null;
		}

		puzzle.ProgressPuzzle ();
	}

	void OnDestroy(){
		puzzle.RemoveListeners ();
	}

	//SteamVR Functions
	public void OnHold(Hand hand){
		enabled = true;
		holdingHand = hand;
	}

	public void OnDrop(Hand hand){
		holdingHand = null;
	}

	private void HandAttachedUpdate(){
		GlowIntensity = accuracy;
		HapticFeedback (GlowIntensity);
	}
		
	//Editor Functions
	public void UpdateShadow(){
		_targetShadow = TargetShadow.GetPixels32 ();
	}
}
