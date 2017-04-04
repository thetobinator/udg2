// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DownSample" {
		Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
		// Shader code pasted into all further CGPROGRAM blocks
		CGINCLUDE
		
		#include "UnityCG.cginc"
	
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 blurVector : TEXCOORD1;
		};
		
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		
		v2f vert( appdata_img v ) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy =  v.texcoord.xy;
		
			return o; 
		}
		
		float4 frag(v2f i) : SV_Target
		{
			float4 tapA = tex2D(_MainTex, i.uv + _MainTex_TexelSize * 0.5);
			float4 tapB = tex2D(_MainTex, i.uv - _MainTex_TexelSize * 0.5);
			float4 tapC = tex2D(_MainTex, i.uv + _MainTex_TexelSize * float2(0.5,-0.5));
			float4 tapD = tex2D(_MainTex, i.uv - _MainTex_TexelSize * float2(0.5,-0.5));
		
			float4 average = (tapA+tapB+tapC+tapD)/4;
			//average.y = max(max(tapA.y,tapB.y), max(tapC.y,tapD.y));
		
			return average;
		}

		ENDCG

Subshader 
{
 Blend One Zero
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      ENDCG
  } // Pass
} // Subshader

Fallback off

} // shader
