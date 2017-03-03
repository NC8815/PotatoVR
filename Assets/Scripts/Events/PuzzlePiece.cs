using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class PuzzlePiece : MonoBehaviour {

//	public GameObject hint;
	public GameObject[] spawns;

	public virtual void AddListeners(){
		EventManager.StartListening ("ProgressPuzzle", ProgressPuzzle);
		if (GetComponentInParent<Interactable> () != null) {
			GetComponentInParent<Interactable> ().onAttachedToHand += GetComponent<GlowShadow> ().OnHold;
			GetComponentInParent<Interactable> ().onDetachedFromHand += GetComponent<GlowShadow> ().OnDrop;
		}
	}

	public virtual void RemoveListeners(){
		EventManager.StopListening ("ProgressPuzzle", ProgressPuzzle);
		if (GetComponentInParent<Interactable> () != null) {
			GetComponentInParent<Interactable> ().onAttachedToHand -= GetComponent<GlowShadow> ().OnHold;
			GetComponentInParent<Interactable> ().onDetachedFromHand -= GetComponent<GlowShadow> ().OnDrop;
		}
	}

	public virtual void ProgressPuzzle(){
		//EventManager.StopListening ("ProgressPuzzle", ProgressPuzzle);
		for (int i = 0; i < spawns.Length; i++) {
			GameObject go = Instantiate (spawns [i]);
			if (go.GetComponent<GlowShadow>() != null) {
				go.GetComponent<GlowShadow> ().Sampler = GetComponent<GlowShadow> ().Sampler;
				go.GetComponent<GlowShadow> ().LightSource = GetComponent<GlowShadow> ().LightSource;
				GetComponent<GlowShadow> ().Sampler.Target = go;
			}
		}

		Destroy (gameObject);
	}

	void OnDestroy(){
		Debug.Log ("DESTROYING");
		RemoveListeners ();
	}
}
