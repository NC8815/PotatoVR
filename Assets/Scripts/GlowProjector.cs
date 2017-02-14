using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GlowPrePass))]
[RequireComponent(typeof(Projector))]
public class GlowProjector : MonoBehaviour {

	[SerializeField]
	private Material _material;
	private Projector projector { get { return GetComponent<Projector> (); } }

	// Use this for initialization
	void Awake () {
		//_material = new Material (Shader.Find("Projector/Light"));
		//_material.shader = Shader.Find("Projector/Light");
		//_material.SetTexture ("Cookie", GlowPrePass.PrePass);
		projector.material.shader = Shader.Find ("Projector/Light");
		//Shader.SetGlobalTexture ("GlowTex", GlowPrePass.PrePass);
	}

	void Update () {
	}
}
