using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEvent : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip collisionSfx;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			//PlaySound ();
		}
	}

	void OnCollisionEnter(Collision other){
		PlaySound ();
	}

	public void PlaySound()
	{
		if (!audioSource.isPlaying)
			AudioController.instance.PlaySingle (collisionSfx,audioSource);
	}
}
