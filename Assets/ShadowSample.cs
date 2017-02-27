using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowSample : MonoBehaviour {

	public static Texture2D CurrentImage;

	Camera _camera { get { return GetComponent<Camera> (); } }
	public GameObject Target;

	Rect imageRect;

	void OnEnable () {
		//Set up the replacement shader and current image
		Shader glowShader = Shader.Find ("Hidden/SolidWhite");
		_camera.SetReplacementShader (glowShader, "Glowable");
		imageRect = new Rect (0, 0, _camera.targetTexture.width, _camera.targetTexture.height);
		CurrentImage = new Texture2D (_camera.targetTexture.width, _camera.targetTexture.height);
	}

	void Update(){
		//If we have a target, look at it and orient the camera.
		if (Target != null) {
			transform.up = Target.transform.up;
			transform.LookAt (Target.transform, transform.up);
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
