using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SpawnEvent : MonoBehaviour {

	public GameObject shadowObject;
	public AudioSource audioSource;
	public AudioClip collisionSfx;


	private UnityAction someListener;

	void Awake ()
	{
		someListener = new UnityAction (SpawnShadow);
		audioSource = GetComponent<AudioSource> ();
	}

	void OnEnable ()
	{
		EventManager.StartListening ("Spawn", someListener);
	}

	void OnDisable ()
	{
		EventManager.StopListening ("Spawn", someListener);
	}

	//Can put on click here to trigger event
	void Update(){
		if (Input.GetKeyDown ("o")) {
			EventManager.TriggerEvent ("Spawn");
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			PlaySound ();
		}
	}
		
	void SpawnShadow ()
	{
		Instantiate (shadowObject, transform.position, Quaternion.identity);
	}

	void OnCollisionEnter(Collision other){
		PlaySound ();
	}

	public void PlaySound()
	{
		AudioController.instance.efxSource = audioSource;
		if (!AudioController.instance.efxSource.isPlaying) {
			AudioController.instance.PlaySingle (audioSource, collisionSfx);
		}
	}
}