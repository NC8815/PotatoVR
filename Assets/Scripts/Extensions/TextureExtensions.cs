using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TextureExtensions {
	public static float Compare(this Texture2D tex, Color32[] other){
		Color32[] texMap = tex.GetPixels32 ();
		float matches = 0;
		if (texMap.Length == other.Length) {
			for (int i = 0; i < other.Length; i++) {
				if (texMap [i].r == other [i].r) {
					matches++;
				}
			}
		}
		return matches / texMap.Length;
	}
}
