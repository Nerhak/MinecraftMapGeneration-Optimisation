Shader "Custom/VoxelUnlitArray"
{
	Properties
	{
		_MainTex("2DArrayTexture", 2DArray) = "white" {}
		_Intensity("Intensity", Range(0, 1)) = 0.0
	}
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 100
		Lighting Off

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma require 2darray
			#include "UnityCG.cginc"

			UNITY_DECLARE_TEX2DARRAY(_MainTex);
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				int textureLayerIndex : TEXCOORD2;
				float4 color : COLOR;
			};

			float GlobalLightLevel;
			float MinGlobalLightLevel;
			float MaxGlobalLightLevel;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				o.textureLayerIndex = v.uv.z;
				o.color = v.color;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(i.uv, i.textureLayerIndex));
				clip(c.a - 1);
				c *= i.color.a;
				return c;
			}
			ENDCG
		}
	}
}
