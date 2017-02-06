using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System;

//  Copyright(c) 2016, Michal Skalsky
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its contributors
//     may be used to endorse or promote products derived from this software without
//     specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT
//  SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
//  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//  This code is slightly rewritten from the original, but I wouldn't have known how without the source code. -Noah

[RequireComponent(typeof(Light))]
public class MyVolumetricLight : MonoBehaviour {

	public event Action<MyVolumetricLightRenderer,MyVolumetricLight,CommandBuffer,Matrix4x4> VolumetricRenderEvent;

	private Light _light;
	private Material _material;
	private CommandBuffer _commandBuffer;
	private CommandBuffer _cascadeShadowCommandBuffer;

	[Range(1, 64)]
	public int SampleCount = 8;
	[Range(0.0f, 1.0f)]
	public float ScatteringCoef = 0.5f;
	[Range(0.0f, 0.1f)]
	public float ExtinctionCoef = 0.01f;
	[Range(0.0f, 1.0f)]
	public float SkyboxExtinctionCoef = 0.9f;
	[Range(0.0f, 0.999f)]
	public float MieG = 0.1f;
	public bool HeightFog = false;
	[Range(0, 0.5f)]
	public float HeightScale = 0.10f;
	public float GroundLevel = 0;
	public bool Noise = false;
	public float NoiseScale = 0.015f;
	public float NoiseIntensity = 1.0f;
	public float NoiseIntensityOffset = 0.3f;
	public Vector2 NoiseVelocity = new Vector2(3.0f, 3.0f);

	public Light Light { get { return _light; } }
	public Material VolumetricMaterial { get { return _material; } }


	//Setting up initial private variables.
	void Start () {

		_commandBuffer = new CommandBuffer ();
		_commandBuffer.name = "Light Command Buffer";

		_cascadeShadowCommandBuffer = new CommandBuffer ();
		_cascadeShadowCommandBuffer.name = "Directional Light Command Buffer";
		_cascadeShadowCommandBuffer.SetGlobalTexture ("_CascadeShadowMapTexture", new RenderTargetIdentifier (BuiltinRenderTextureType.CurrentActive));

		_light = GetComponent<Light> ();
		if (_light.type == LightType.Directional) {
			_light.AddCommandBuffer (LightEvent.BeforeScreenspaceMask,_commandBuffer);
			_light.AddCommandBuffer (LightEvent.AfterShadowMap,_cascadeShadowCommandBuffer);
		} else {
			_light.AddCommandBuffer (LightEvent.AfterShadowMap,_commandBuffer);
		}

		Shader shader = Shader.Find ("Lights/VolumetricLight");
		if (shader == null)
			throw new Exception ("ERROR: \"Lights/VolumetricLight\" shader is missing. Check \"Always Included Shaders\" in ProjectSettings/Graphics");
		_material = new Material (shader);

	}

	//Setting up Listeners
	void OnEnable(){
		MyVolumetricLightRenderer.PreRenderEvent += VolumetricLightRenderer_PreRenderEvent;
	}

	void OnDisable()
	{
		MyVolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
	}

	public void OnDestroy()
	{   
		//Prevent memory leaks by deleting materials that are dynamically created.
		Destroy(_material);
	}

	void Update(){
		_commandBuffer.Clear ();
	}
		
	//Utility variable which determines if the distance between the currently rendering camera and the point light is less than the range of the light
	private bool IsCameraInPointLightBounds {
		get {
			float distanceSqr = (_light.transform.position - Camera.current.transform.position).sqrMagnitude;
			float extendedRange = _light.range + 1;
			return (distanceSqr < (extendedRange * extendedRange));
		}
	}

	private bool IsCameraInSpotLightBounds {
		get {
			// Check if the camera is outside the end range of the light
			float distance = Vector3.Dot (_light.transform.forward, (Camera.current.transform.position - _light.transform.position));
			float extendedRange = _light.range + 1;
			if (distance > (extendedRange))
				return false;

			// Check if the camera is outside the subtended arc of the light.
			//The dot product of two unit vectors is the cosine of the angle between them.
			float cosAngle = Vector3.Dot (transform.forward, (Camera.current.transform.position - _light.transform.position).normalized);
			if ((Mathf.Acos (cosAngle) * Mathf.Rad2Deg) > (_light.spotAngle + 3) * 0.5f)
				return false;

			return true;
		}
	}

	private void VolumetricLightRenderer_PreRenderEvent(MyVolumetricLightRenderer renderer, Matrix4x4 viewProjection){
		//If code somehow removed the light or the gameobject without deregistering the event, do that.
		if (_light == null || _light.gameObject == null)
			MyVolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;

		//If the object isn't active or the light is disabled, don't do anything.
		if (!_light.gameObject.activeInHierarchy || !_light.enabled)
			return;

		//Set shader data
		_material.SetVector ("_CameraForward", Camera.main.transform.forward);

		_material.SetInt("_SampleCount", SampleCount);
		_material.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
		_material.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
		_material.SetVector("_MieG", new Vector4(1 - (MieG * MieG), 1 + (MieG * MieG), 2 * MieG, 1.0f / (4.0f * Mathf.PI)));
		_material.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, _light.range, 1.0f - SkyboxExtinctionCoef));

		//_material.SetTexture ("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer ());

		_material.SetFloat ("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

		int pass;

		switch (_light.type) {
		case LightType.Directional:
			pass = 4;
			//SetupDirectionLight (renderer, viewProjection, pass);
			break;
		case LightType.Point:
			pass = IsCameraInPointLightBounds ? 0 : 2;
			SetupPointLight (renderer, viewProjection, pass);
			break;
		case LightType.Spot:
			pass = IsCameraInSpotLightBounds ? 1 : 3;
			SetupSpotLight (renderer, viewProjection, pass);
			break;
		default:
			break;
		}
	}


	private void SetupPointLight(MyVolumetricLightRenderer renderer, Matrix4x4 viewProjection, int pass){
		_material.SetPass (pass);

		Mesh mesh = MyVolumetricLightRenderer.PointLightMesh;

		float scale = _light.range * 2;
		Matrix4x4 world = Matrix4x4.TRS (transform.position, transform.rotation, Vector3.one * scale);

		_material.SetMatrix ("_WorldViewProjection", viewProjection * world);
		_material.SetMatrix ("_WorldView", Camera.current.worldToCameraMatrix * world);

		if (Noise)
			_material.EnableKeyword ("NOISE");
		else
			_material.DisableKeyword ("NOISE");

		_material.SetVector ("_LightPos", new Vector4 (_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1.0f / (_light.range * _light.range)));
		_material.SetVector ("_LightColor", _light.color * _light.intensity);

		_material.EnableKeyword("SHADOWS_CUBE");

		_commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
		_commandBuffer.SetRenderTarget(renderer.VolumeLightBuffer);

		_commandBuffer.DrawMesh(mesh, world, _material, 0, pass);

		if (VolumetricRenderEvent != null)
			VolumetricRenderEvent(renderer, this, _commandBuffer, viewProjection); 
	}

	private void SetupSpotLight(MyVolumetricLightRenderer renderer, Matrix4x4 viewProjection, int pass){
		_material.SetPass (pass);

		Mesh mesh = MyVolumetricLightRenderer.SpotLightMesh;

		float scale = _light.range;
		float angleScale = Mathf.Tan ((_light.spotAngle + 1) * 0.5f * Mathf.Deg2Rad) * scale;

		Matrix4x4 world = Matrix4x4.TRS (transform.position, transform.rotation, Vector3.one * scale);
		Matrix4x4 view = Matrix4x4.TRS (transform.position, transform.rotation, Vector3.one).inverse;

		Matrix4x4 clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1.0f));
		Matrix4x4 proj = Matrix4x4.Perspective(_light.spotAngle, 1, 0, 1);

		_material.SetMatrix ("_MyLightMatrix0", clip * proj * view);
		_material.SetMatrix ("_WorldViewProjection", viewProjection * world);

		_material.SetVector ("_LightPos", new Vector4 (_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1.0f / (_light.range * _light.range)));
		_material.SetVector ("_LightColor", _light.color * _light.intensity);

		Vector3 apex = transform.position;
		Vector3 axis = transform.forward;
		// plane equation ax + by + cz + d = 0; precompute d here to lighten the shader
		Vector3 center = apex + axis * _light.range;
		float d = -Vector3.Dot(center, axis);

		// update material
		_material.SetFloat("_PlaneD", d);        
		_material.SetFloat("_CosAngle", Mathf.Cos((_light.spotAngle + 1) * 0.5f * Mathf.Deg2Rad));

		_material.SetVector("_ConeApex", new Vector4(apex.x, apex.y, apex.z));
		_material.SetVector("_ConeAxis", new Vector4(axis.x, axis.y, axis.z));

		_material.EnableKeyword("SPOT");

		if (Noise)
			_material.EnableKeyword("NOISE");
		else
			_material.DisableKeyword("NOISE");

		//_material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());

		clip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));

		proj = Matrix4x4.Perspective(_light.spotAngle, 1, _light.shadowNearPlane, _light.range);

		Matrix4x4 m = clip * proj;
		m[0, 2] *= -1;
		m[1, 2] *= -1;
		m[2, 2] *= -1;
		m[3, 2] *= -1;

		//view = _light.transform.worldToLocalMatrix;
		_material.SetMatrix("_MyWorld2Shadow", m * view);
		_material.SetMatrix("_WorldView", m * view);

		_material.EnableKeyword("SHADOWS_DEPTH");
		_commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
		_commandBuffer.SetRenderTarget(renderer.VolumeLightBuffer);

		_commandBuffer.DrawMesh(mesh, world, _material, 0, pass);

		if (VolumetricRenderEvent != null)
			VolumetricRenderEvent(renderer, this, _commandBuffer, viewProjection);    
	}
}
