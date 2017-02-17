using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class PuzzlePiece : MonoBehaviour {

	public virtual void AddListeners(){
		EventManager.StartListening ("ProgressPuzzle", ProgressPuzzle);
		GetComponentInParent<Interactable> ().onAttachedToHand += GetComponent<GlowShadow> ().OnHold;
		GetComponentInParent<Interactable> ().onDetachedFromHand += GetComponent<GlowShadow> ().OnDrop;
	}

	public virtual void RemoveListeners(){
		EventManager.StopListening ("ProgressPuzzle", ProgressPuzzle);
		GetComponentInParent<Interactable> ().onAttachedToHand -= GetComponent<GlowShadow> ().OnHold;
		GetComponentInParent<Interactable> ().onDetachedFromHand -= GetComponent<GlowShadow> ().OnDrop;
	}

	public virtual void ProgressPuzzle(){
		EventManager.StopListening ("ProgressPuzzle", ProgressPuzzle);
	}

	void OnDestroy(){
		RemoveListeners ();
	}
}
