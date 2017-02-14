using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GlowShadow))]
public class GlowShadowEditor : Editor {

	public GlowShadow current { get { return (GlowShadow)target; } }

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		EditorGUILayout.FloatField ("Accuracy:", current.accuracy);
		if (GUILayout.Button ("Set")) {
			current.SetTarget (current.transform.forward,current.transform.up,current.transform.position);
		}
		if (GUI.changed)
			EditorUtility.SetDirty (current);
	}
}
