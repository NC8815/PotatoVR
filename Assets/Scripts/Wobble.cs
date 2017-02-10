using UnityEngine;
using System.Collections;

public class Wobble : MonoBehaviour {

	Vector3 EPS = Vector3.zero;
	Vector3 lastEPS;
	Vector3 nextEPS;
	[Range (0.01f,10)]
	public float Stability = 1;
	float timeToChange = 0;

	public Vector3 EulersPerSecond;

	void Start(){
		lastEPS = EPS;
		nextEPS = Random.onUnitSphere;
	}

	// Update is called once per frame
	void Update () {
		timeToChange += Time.deltaTime;
		transform.Rotate (EPS * Time.deltaTime / Stability);
		transform.Rotate (EulersPerSecond * Time.deltaTime);
		EPS = Vector3.Slerp (lastEPS, nextEPS, timeToChange / Stability);
		if (timeToChange >= Stability) {
			lastEPS = nextEPS;
			nextEPS = Random.onUnitSphere;
			timeToChange = 0;
		}
	}
}
