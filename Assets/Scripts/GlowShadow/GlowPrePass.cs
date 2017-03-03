using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Projector))]
public class GlowPrePass : MonoBehaviour
{
	public float Intensity = 2;

	private RenderTexture ProjectionTexture;
	private Material ProjectionMaterial;

	private RenderTexture Blurred;
	private Material _blurMat;

	private RenderTexture PrePass;

	private Material _compositeMat;

	private RenderTexture Border;
	private Material _bordMat;

	private Camera _camera {get { return GetComponent<Camera>();}}
	private Projector _projector {get { return GetComponent<Projector>(); }}

	void OnEnable()
	{
		//Instantiate the PrePass RenderTexture
		PrePass = new RenderTexture (Screen.width << 1, Screen.height << 1, 24);
		PrePass.antiAliasing = QualitySettings.antiAliasing;

		//Instantiate the Blurred Material and RenderTexture
		Blurred = new RenderTexture (Screen.width << 1, Screen.width << 1, 0);
		_blurMat = new Material (Shader.Find ("Hidden/Blur"));
		_blurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * 1.5f, Blurred.texelSize.y * 1.5f));

		//Instantiate the Border Material and RenderTexture
		Border = new RenderTexture(Screen.width << 1, Screen.height << 1, 0);
		_bordMat = new Material (Shader.Find ("Hidden/Border"));
		_bordMat.SetVector ("_BorderSize", new Vector2 (Border.texelSize.x, Border.texelSize.y));

		//Set the camera replacement shader, and target the PrePass texture.
		Shader glowShader = Shader.Find("Hidden/GlowReplace");
		_camera.SetReplacementShader(glowShader, "Glowable");
		_camera.targetTexture = PrePass;
		_camera.targetTexture.wrapMode = TextureWrapMode.Clamp;

		//Instantiate the composite material, then set the prepass and blurred textures.
		_compositeMat = new Material(Shader.Find("Hidden/GlowComposite"));
		Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
		Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
		Shader.SetGlobalTexture ("_GlowBorderTex", Border);

		_camera.aspect = 1;
		_projector.aspectRatio = _camera.aspect;
		_projector.material.SetTexture ("_ShadowTex", _camera.targetTexture);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		_compositeMat.SetFloat ("_Intensity", Intensity);

		Graphics.Blit(src, dst);

		Graphics.SetRenderTarget (Border);
		GL.Clear (false, true, Color.white);

		Graphics.Blit (src, Border);

		var temp = RenderTexture.GetTemporary(Border.width, Border.height);
		Graphics.Blit(Border, temp, _bordMat, 0);
		Graphics.Blit(temp, Border, _bordMat, 1);
		RenderTexture.ReleaseTemporary(temp);

		Graphics.Blit (src, dst, _compositeMat, 0);

		Graphics.SetRenderTarget(Blurred);
		GL.Clear(false, true, Color.clear);

		Graphics.Blit(dst, Blurred);
		
		for (int i = 0; i < 2; i++)
		{
			temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			Graphics.Blit(Blurred, temp, _blurMat, 0);
			Graphics.Blit(temp, Blurred, _blurMat, 1);
			RenderTexture.ReleaseTemporary(temp);
		}

		Graphics.Blit (src, dst, _compositeMat,1);
	}
}
