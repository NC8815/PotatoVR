using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class InteractionTarget : MonoBehaviour {

	void Awake(){
		//GetComponent<Interactable>().
	}

	private void OnHandHoverBegin(Hand hand){
		Debug.Log ("Began Hovering!");
		GameObject held = hand.currentAttachedObject;
		if (held.name == "WhirlyGig") {
			hand.DetachObject (held);
			Destroy (held);
		}
	}
}
