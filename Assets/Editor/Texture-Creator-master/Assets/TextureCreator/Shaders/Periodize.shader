Shader "Hidden/Periodize" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MainX ("Base (RGB)", 2D) = "white" {}
	_MainY ("Base (RGB)", 2D) = "white" {}

    _MaskX ("Base (RGB)", 2D) = "white" {}
	_MaskY ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _MainX;
uniform sampler2D _MainY;
uniform sampler2D _MaskX;
uniform sampler2D _MaskY;
uniform half _x;
uniform half _y;

fixed4 frag (v2f_img i) : SV_Target
{
	fixed4 a = tex2D(_MainTex, i.uv + float2(_x,_y));

	fixed4 output = a;

	return output;
}

fixed4 fragCombine (v2f_img i) : SV_Target
{
	fixed4 a = tex2D(_MainTex, i.uv + float2(_x,_y));
	fixed4 b = tex2D(_MainTex, i.uv + float2(0.5,0.0));
	fixed4 c = tex2D(_MainTex, i.uv + float2(0.0,0.5));

	fixed4 mx = tex2D(_MaskX, i.uv);
	fixed4 my = tex2D(_MaskY, i.uv);

	fixed4 output = my*b + mx*c + (1-my-mx)*a;

	return output;
}


ENDCG

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"
		ENDCG
	}

	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragCombine
		#include "UnityCG.cginc"
		ENDCG
	}
	
}

Fallback off

}
