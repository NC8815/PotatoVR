using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

[CustomEditor(typeof(GlowShadow))]
public class GlowShadowEditor : Editor {

	Texture2D temp;
	public GlowShadow current { get { return (GlowShadow)target; } }

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		EditorGUILayout.FloatField ("Accuracy:", current.accuracy);
		current.TargetShadow = (Texture2D)EditorGUILayout.ObjectField ("Target Shadow", current.TargetShadow, typeof(Texture2D), false);
		if (temp != current.TargetShadow) {
			temp = current.TargetShadow;
			current.UpdateShadow ();
			EditorUtility.SetDirty (current);
		}
	}
}
