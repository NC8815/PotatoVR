using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SpawnEvent : MonoBehaviour {

	public GameObject shadowObject;


	private UnityAction someListener;

	void Awake ()
	{
		someListener = new UnityAction (SpawnShadow);
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

	}
		
	void SpawnShadow ()
	{
		Instantiate (shadowObject, transform.position, Quaternion.identity);
	}


}