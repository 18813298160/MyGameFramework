// 血条，可以控制图片显示百分比
Shader "Shader/UI/Heathbar"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_FillAmount("FillAmount", Range(0, 1)) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
	#pragma vertex SpriteVert
	#pragma fragment SpriteFrag
	#pragma target 2.0


#include "UnityCG.cginc"

#ifdef UNITY_INSTANCING_ENABLED

		UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
		// SpriteRenderer.Color while Non-Batched/Instanced.
		UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
		// this could be smaller but that's how bit each entry is regardless of type
		UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
		UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

		#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
		#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

#endif // instancing

		CBUFFER_START(UnityPerDrawSprite)

#ifndef UNITY_INSTANCING_ENABLED
		fixed4 _RendererColor;
		fixed2 _Flip;
#endif
		float _EnableExternalAlpha;
		CBUFFER_END

		// Material Color.
		half _FillAmount;
		half4 _Color;
		sampler2D _MainTex;

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f SpriteVert(appdata_t IN)
		{
			v2f OUT;

			UNITY_SETUP_INSTANCE_ID(IN);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
			IN.vertex.xy *= _Flip;
#endif

			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.texcoord = half2(IN.texcoord.x, IN.texcoord.y);
			OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
			return OUT;
		}

		half4 SpriteFrag(v2f IN) : SV_Target
		{
			half4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
			c.a *= step(IN.texcoord.x, _FillAmount);
			c.rgb *= c.a;
			return c;
		}
			ENDCG
		}
	}
}
