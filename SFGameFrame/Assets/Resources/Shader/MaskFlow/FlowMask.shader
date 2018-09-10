// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33097,y:32707,varname:node_3138,prsc:2|emission-3517-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32235,y:32877,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:208,x:31980,y:32551,ptovrint:False,ptlb:tex,ptin:_tex,varname:node_208,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7068-OUT;n:type:ShaderForge.SFN_Multiply,id:6917,x:31728,y:32985,varname:node_6917,prsc:2|A-2818-T,B-1755-OUT;n:type:ShaderForge.SFN_Slider,id:999,x:31190,y:33173,ptovrint:False,ptlb:speed_v,ptin:_speed_v,varname:node_999,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Append,id:1755,x:31493,y:33039,varname:node_1755,prsc:2|A-6388-OUT,B-999-OUT;n:type:ShaderForge.SFN_Slider,id:6388,x:31190,y:33039,ptovrint:False,ptlb:speed_u,ptin:_speed_u,varname:node_6388,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Time,id:2818,x:31248,y:32837,varname:node_2818,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:9758,x:31371,y:32593,varname:node_9758,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:7068,x:31745,y:32705,varname:node_7068,prsc:2|A-9758-UVOUT,B-6917-OUT;n:type:ShaderForge.SFN_Multiply,id:9490,x:32254,y:32671,varname:node_9490,prsc:2|A-208-RGB,B-7272-OUT;n:type:ShaderForge.SFN_Slider,id:7272,x:31939,y:32903,ptovrint:False,ptlb:light,ptin:_light,varname:node_7272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Multiply,id:1782,x:32491,y:32880,varname:node_1782,prsc:2|A-9490-OUT,B-7241-RGB,C-3053-RGB,D-3053-A;n:type:ShaderForge.SFN_VertexColor,id:3053,x:32235,y:33042,varname:node_3053,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3517,x:32721,y:32918,varname:node_3517,prsc:2|A-1782-OUT,B-2706-RGB;n:type:ShaderForge.SFN_Tex2d,id:2706,x:32460,y:33090,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_2706,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:7241-208-999-6388-7272-2706;pass:END;sub:END;*/

Shader "Shader Forge/mask_add" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _tex ("tex", 2D) = "white" {}
        _speed_v ("speed_v", Range(-3, 3)) = 0
        _speed_u ("speed_u", Range(-3, 3)) = 0
        _light ("light", Range(0, 5)) = 1
        _mask ("mask", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
			Lighting Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "UnityUI.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _tex; uniform float4 _tex_ST;
            uniform float _speed_v;
            uniform float _speed_u;
            uniform float _light;
            uniform sampler2D _mask; uniform float4 _mask_ST;
			float4 _ClipRect;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
				float4 worldPosition : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
				o.worldPosition = v.vertex;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 node_2818 = _Time + _TimeEditor;
                float2 node_7068 = (i.uv0+(node_2818.g*float2(_speed_u,_speed_v)));
                float4 _tex_var = tex2D(_tex,TRANSFORM_TEX(node_7068, _tex));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 emissive = (((_tex_var.rgb*_light)*_Color.rgb*i.vertexColor.rgb*i.vertexColor.a)*_mask_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor, _tex_var.a * UnityGet2DClipping(i.worldPosition.xy, _ClipRect));
            }
            ENDCG
        }
    }

}
