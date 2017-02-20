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
		EventManager.StopListening ("ProgressPuzzle", ProgressPuzzle);
		for (int i = 0; i < spawns.Length; i++) {
			Instantiate (spawns [i]);
		}
//		Destroy (transform.parent.gameObject);
		Destroy (gameObject);
		if(hint!= null)
			Destroy (hint);
	}

	void OnDestroy(){
		RemoveListeners ();
	}
}
