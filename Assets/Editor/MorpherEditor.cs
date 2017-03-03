using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Morpher))]
public class MorpherEditor : Editor {

	Morpher current {get {return (Morpher)target;}}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button ("Morph")) {
			Mesh mesh = current.GetComponent<MeshFilter> ().sharedMesh;
			Matrix4x4 proj = current.camera.projectionMatrix;
			Vector3[] newVerts = mesh.vertices.Select (v => proj.MultiplyPoint (v)).ToArray();
			mesh.vertices = newVerts;
			current.GetComponent<MeshFilter> ().mesh = mesh;
		}
	}
}
