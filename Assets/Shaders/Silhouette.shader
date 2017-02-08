Shader "Custom/Silhouette"
{
	Properties
	{
		_Color ("Color", Color) = (0.0,0.0,0.0,0.0)
	}
	SubShader
	{
		Tags{"Glowable"="True"}
		Pass
		{
			CGPROGRAM

			//pragmas
			#pragma vertex vert
			#pragma fragment frag

			//user defined variables
			uniform float4 _Color;

			//base input structs
			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			//vertex function
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = fixed4 (0,0,0,0);
				return o;
			}

			//fragment function
			float4 frag(vertexOutput i) : COLOR
			{
				//return _Color;
				return fixed4 (0,0,0,0);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}