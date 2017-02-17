using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(GlowPrePass))]
public class GlowPrePassEditor : Editor {

	GlowPrePass current { get { return (GlowPrePass)target; } }

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

//		current.SnapshotPath = EditorGUILayout.TextField ("Snapshot Name:", current.SnapshotPath);
//
//		if (GUILayout.Button ("Take Snapshot")) {
//			if (current.SnapshotPath != string.Empty) {
//
//				byte[] png = current.ScreenShot;
//
//				File.WriteAllBytes (Application.dataPath + "/Textures/Object Silhouettes/" + current.SnapshotPath + ".png", png);
//
//				AssetDatabase.Refresh ();
//			}
//		}
	}

}