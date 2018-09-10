Shader "Custom/Grid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Lighting off //关闭光照
		ZWrite off //关闭深度缓存

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

            uniform float _Rows;
            uniform float _Columns;

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				//做乘法分区
				i.uv *= fixed2(_Columns, _Rows);
			    return tex2D(_MainTex, i.uv);

			}
			ENDCG
		}
	}
}
