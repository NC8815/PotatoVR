Shader "Hidden/Border"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		// Horizontal
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _BorderSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv);
				s = max(s, tex2D(_MainTex, i.uv + float2(_BorderSize.x * 1, 0)));
				s = max(s, tex2D(_MainTex, i.uv + float2(_BorderSize.x * 2, 0)));
				s = max(s, tex2D(_MainTex, i.uv + float2(_BorderSize.x * -2, 0)));
				s = max(s, tex2D(_MainTex, i.uv + float2(_BorderSize.x * -1, 0)));

				return s;
//				fixed4 s = 
//				tex2D(_MainTex, i.uv) + 
//				tex2D(_MainTex, i.uv + float2(_BlurSize.x * 1, 0)) + 
//				tex2D(_MainTex, i.uv + float2(_BlurSize.x * 2, 0)) +
//				tex2D(_MainTex, i.uv + float2(_BlurSize.x * -1, 0)) +
//				tex2D(_MainTex, i.uv + float2(_BlurSize.x * -2, 0));
//
//				fixed4 black = (0,0,0,1);
//
//				return any(s.rgb) ? black : tex2D(_MainTex, i.uv);
			}
			ENDCG
		}

		// Vertical
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _BorderSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv);
				s = max(s, tex2D(_MainTex, i.uv + float2(0, _BorderSize.y * 2)));
				s = max(s, tex2D(_MainTex, i.uv + float2(0, _BorderSize.y * 1)));			
				s = max(s, tex2D(_MainTex, i.uv + float2(0, _BorderSize.y * -1)));
				s = max(s, tex2D(_MainTex, i.uv + float2(0, _BorderSize.y * -2)));
				return s;
//				fixed4 s = 
//				tex2D(_MainTex, i.uv) + 
//				tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * 1)) + 
//				tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * 2)) +
//				tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -1)) +
//				tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -2));
//
//				fixed4 black = (0,0,0,1);
//
//				return any(s.rgb) ? black : tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
