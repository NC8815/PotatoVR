using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class PuzzlePiece : MonoBehaviour {

	public GameObject hint;
	public GameObject[] spawns;

	void Start(){
		if (hint != null)
			hint = Instantiate (hint);
	}

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
			if (i == 0) {
				go.GetComponent<GlowShadow> ().Sampler = GetComponent<GlowShadow> ().Sampler;
				GetComponent<GlowShadow> ().Sampler.Target = go;
			}
		}
		//GetComponent<GlowShadow> ().holdingHand.DetachObject (gameObject);
		if(hint!= null)
			Destroy (hint);
		Destroy (gameObject);
	}

	void OnDestroy(){
		RemoveListeners ();
	}
}
