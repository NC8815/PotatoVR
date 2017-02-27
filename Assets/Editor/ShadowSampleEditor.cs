using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ShadowSample))]
public class ShadowSampleEditor : Editor {

	ShadowSample current { get { return (ShadowSample)target; } }

	public override void OnInspectorGUI ()
	{
		RenderTexture cameraImage = current.GetComponent<Camera> ().targetTexture;

		base.OnInspectorGUI ();
		if (GUILayout.Button (cameraImage)) {
			string path = EditorUtility.SaveFilePanelInProject ("Save texture as PNG", "snapshot.png", "png", "Please enter a file name.");
			if (path.Length != 0) {
				//Save previous settings and get a temp texture.
				Texture2D temp = new Texture2D (cameraImage.width, cameraImage.height, TextureFormat.ARGB32, false);
				RenderTexture rt = RenderTexture.active;

				//Read the camera and get the data;
				RenderTexture.active = cameraImage;
				temp.ReadPixels (new Rect (0, 0, cameraImage.width, cameraImage.height), 0, 0);
				var pngData = temp.EncodeToPNG ();

				//Reset previous settings and clean up temps.
				RenderTexture.active = rt;
				DestroyImmediate (temp);
				DestroyImmediate (rt);

				//Save the data at the assigned path.
				if (pngData != null) {
					File.WriteAllBytes (path, pngData);
					AssetDatabase.Refresh ();
				}
			}
		}
	}
}
