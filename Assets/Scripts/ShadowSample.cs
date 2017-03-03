using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowSample : MonoBehaviour {

	public static Texture2D CurrentImage;

	public static ShadowSample Instance { get; private set; }

	Camera _camera { get { return GetComponent<Camera> (); } }
	public GameObject Target;

	Rect imageRect;

	void Awake(){
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy (this);
	}

	void OnEnable () {
		//Set up the replacement shader and current image
		Shader glowShader = Shader.Find ("Hidden/SolidWhite");
		_camera.SetReplacementShader (glowShader, "Glowable");
		imageRect = new Rect (0, 0, _camera.targetTexture.width, _camera.targetTexture.height);
		CurrentImage = new Texture2D (_camera.targetTexture.width, _camera.targetTexture.height);
	}

	void UpdateFOV(){
		if (Target != null && Target.GetComponentInChildren<MeshFilter> () != null) {
			Mesh mesh = Target.GetComponentInChildren<MeshFilter> ().sharedMesh;
			GameObject child = Target.GetComponentInChildren<MeshFilter> ().gameObject;
			Vector3[] verts = mesh.vertices;
			float minX = Mathf.Infinity;
			float maxX = Mathf.NegativeInfinity;
			float minY = Mathf.Infinity;
			float maxY = Mathf.NegativeInfinity;
			for (int i = 0; i < verts.Length; i++) {
				verts[i] = child.transform.TransformPoint(verts[i]);
//				verts [i] = transform.InverseTransformPoint (verts [i]);
				verts [i] = _camera.worldToCameraMatrix.MultiplyPoint (verts [i]);
				minX = Mathf.Min (minX, verts [i].x);
				maxX = Mathf.Max (maxX, verts [i].x);
				minY = Mathf.Min (minY, verts [i].y);
				maxY = Mathf.Max (maxY, verts [i].y);
			}
			float width = maxX - minX;
			float height = maxY - minY;
			float cross = Mathf.Max (width, height);
			float dist = Mathf.Clamp (Vector3.Distance (transform.position, Target.transform.position), _camera.nearClipPlane, _camera.farClipPlane);
			float tan = cross / 2 / dist;
			_camera.fieldOfView = Mathf.Atan (tan) * Mathf.Rad2Deg * 2;
		}
	}

	void Update(){
		//If we have a target, look at it and orient the camera.
		if (Target != null) {
			transform.up = Target.transform.up;
			transform.LookAt (Target.transform, transform.up);
			UpdateFOV ();
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst){
		//Read source to the current image so other scripts can access them.
		RenderTexture.active = _camera.targetTexture;
		CurrentImage.ReadPixels (imageRect, 0, 0);
		RenderTexture.active = null;

		//Write to the target texture.
		Graphics.Blit(src,dst);
	}
}
