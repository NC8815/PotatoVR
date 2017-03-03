using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TextureExtensions {
	public static float Compare(this Texture2D tex, Color32[] other){
		Color32[] texMap = tex.GetPixels32 ();
		float matches = 0;
//		float baseline = 0;
		if (texMap.Length == other.Length) {
			for (int i = 0; i < other.Length; i++) {
//				baseline += texMap [i].r * 0.5f + 0.5f;
				if (texMap [i].r == other [i].r) {
					matches++;
//					matches+= 0.5f + texMap[i].r * 0.5f; //half credit for matching black space, full marks for matching white space.
				}
			}
		}
		return matches / texMap.Length;
	}
}
