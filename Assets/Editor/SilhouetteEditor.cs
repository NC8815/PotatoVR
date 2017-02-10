using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Silhouette))]
public class SilhouetteEditor : Editor {

	public Silhouette current { get { return (Silhouette)target; } }

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button ("Set")) {
			current.TargetUp = current.transform.up;
			current.TargetForward = current.transform.forward;
			current.TargetLocation = current.transform.position;
		}
	}
}
