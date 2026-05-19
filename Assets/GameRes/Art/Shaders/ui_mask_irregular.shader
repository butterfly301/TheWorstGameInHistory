Shader "aounity/ui/ui_mask_irregular"
{
	Properties
	{
		[PerRendererData] _MainTex("Main Tex", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "white" {}
		_ScaleFactor("ScaleFactor", Float) = 1 //可修改来放缩纹理uv的大小
		[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
	
		[HideInInspector]_ColorMask("Color Mask", Float) = 15
	}

	SubShader
	{

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Pass
		{
			Stencil
			{
				Ref[_Stencil]		
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask] 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScaleFactor;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			float4 _ClipRect;
			float _StencilComp;

			struct a2v
			{
				float4 vertex: POSITION;
				float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0; //两个纹理公用一个变量，减少使用一个寄存器
				float4 worldPosition : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _MaskTex);/*v.texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;*/
				o.worldPosition = v.vertex;//mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float move = _ScaleFactor * (-0.5) + 0.5;
				float3x3 mt = float3x3(_ScaleFactor, 0, move, 0, _ScaleFactor, move, 0, 0, 1); //放缩矩阵
				fixed4 c1 = tex2D(_MainTex, mul(mt, float3(i.uv.xy,1)));
				//fixed4 c1 = tex2D(_MainTex, (i.uv.xy - (0.5, 0.5)) * _ScaleFactor + (0.5,0.5));
				fixed4 c2 = tex2D(_MaskTex, i.uv.zw);
				c1 = c1 * i.color;
				c1.a *= c2.a;
				c1.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				return c1;
			}

			ENDCG
		}
	}
}
