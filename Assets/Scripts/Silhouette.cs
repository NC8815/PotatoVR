using UnityEngine;
using System.Collections;
using System.Linq;

public class Silhouette : MonoBehaviour {
	public GameObject ShadowWall;
	private Plane Wall;

	public GameObject LightSource;

	public Vector3 TargetForward;
	public Vector3 TargetUp;
	public Vector3 TargetLocation;
	private float minProximity = 0.5f;
	public float ForwardAccuracy;
	public float UpAccuracy;
	public float LocationAccuracy;

	float degThreshold = 10;
	float accuracy {
		get {
			float forAcc = Mathf.Clamp01((Vector3.Dot(transform.forward,TargetForward.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			float upAcc = Mathf.Clamp01((Vector3.Dot(transform.up,TargetUp.normalized) - Mathf.Cos(degThreshold * Mathf.Deg2Rad))/(1 - Mathf.Cos(degThreshold * Mathf.Deg2Rad)));
			//float forAcc = Mathf.Lerp(0,1 Mathf.Clamp01 (Vector3.Dot (transform.forward, TargetForward.normalized));
			//float upAcc = Mathf.Clamp01 (Vector3.Dot (transform.up, TargetUp.normalized));
			float locAcc = Mathf.Clamp01 ((minProximity - Vector3.Distance (transform.position, TargetLocation)) / minProximity);
			ForwardAccuracy = forAcc;
			UpAccuracy = upAcc;
			LocationAccuracy = locAcc;
			return forAcc * upAcc * locAcc;
		}
	}

	private GameObject _shadow;
	private Mesh _shadowMesh {
		get {
			if (_shadow.GetComponent<MeshFilter> () == null)
				_shadow.AddComponent<MeshFilter> ();
			return _shadow.GetComponent<MeshFilter> ().mesh;
		}
		set {
			if (_shadow.GetComponent<MeshFilter> () == null)
				_shadow.AddComponent<MeshFilter> ();
			_shadow.GetComponent<MeshFilter> ().mesh = value;
		}
	}
		
	void ProjectShadow(){
		float dist;
		Ray ray = new Ray (LightSource.transform.position, transform.position - LightSource.transform.position);
		if (Wall.Raycast (ray, out dist))
			_shadow.transform.position = ray.GetPoint (dist);
		//_shadow.transform.position = transform.position;
		_shadowMesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = GetComponent<MeshFilter> ().mesh.vertices.Select (p => transform.TransformPoint (p)).ToArray();
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = _shadow.transform.InverseTransformPoint(ProjectPoint (vertices [i]));
		}
		_shadowMesh.vertices = vertices;
	}

	//Project a ray from the lightsource to the original point, given in world space. Return the intersection of the wall and the ray.
	Vector3 ProjectPoint(Vector3 originalPoint){
		Ray ray = new Ray (LightSource.transform.position, originalPoint - LightSource.transform.position);
		//Debug.DrawRay (LightSource.transform.position, originalPoint - LightSource.transform.position, Color.white, 1);
		float dist;
		return Wall.Raycast (ray, out dist) ? ray.GetPoint (dist) : _shadow.transform.position;
	}

	//Set the wall plane using the first three points of the ShadowWall mesh. Use a Quad Mesh.
	void RecalculateWall(){
		Mesh mesh = ShadowWall.GetComponent<MeshFilter> ().mesh;
		Vector3 p1 = ShadowWall.transform.TransformPoint (mesh.vertices [0]);
		Vector3 p2 = ShadowWall.transform.TransformPoint (mesh.vertices [1]);
		Vector3 p3 = ShadowWall.transform.TransformPoint (mesh.vertices [2]);

		Wall = new Plane (p1,p2,p3);
	}

	// Use this for initialization
	void Start () {
		_shadow = Instantiate (Resources.Load<GameObject> ("Prefabs/Shadow")) as GameObject;
		_shadow.name = gameObject.name + " Shadow";
		if (ShadowWall != null) {
			RecalculateWall ();
		} else {
			Debug.Log ("No wall specified for " + gameObject.name);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Precheck ())
			ProjectShadow ();
		_shadow.GetComponent<GlowObject> ().ChangeTargetColor (accuracy);
	}

	bool Precheck(){
		if (ShadowWall == null)
			return false;
		if (LightSource == null)
			return false;
		if (LightSource.GetComponent<Light> () == null || !LightSource.GetComponent<Light> ().enabled)
			return false;
		return true;
	}
}
