Shader "Hidden/SpecShadows" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_HSV ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _HSV;
uniform half _Specular;
uniform half _Shadows;

float4 fragSpec (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	float4 b = tex2D(_HSV, i.uv);
	
	float ma = max(a.r,a.b);
	ma = max(ma,a.g);

	float mi = min(a.r,a.b);
	mi = min(mi,a.g);

	float range = ma-mi + 0.001; //I had a small number in case ma==mi
	//float Q = ma / range;
	float coef = sign(_Specular)*(25*_Specular)*(25*_Specular);

	float s = max( ma -coef*range, 0); 

	float4 output = a-s*(1-b);//clamp(Q,0.0,10.0);
	
	output = clamp(output,0,1);

	return output;
}

float4 fragShadows (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	float4 b = tex2D(_HSV, i.uv);
	
	float ma = max(a.r,a.b);
	ma = max(ma,a.g);

	float mi = min(a.r,a.b);
	mi = min(mi,a.g);

	float range = ma-mi + 0.001; //I had a small number in case ma==mi
	
	float coef = sign(_Shadows)*(25*_Shadows)*(25*_Shadows);

	float s = 1-min( mi + coef*range,1);

	float4 output = a+s*b;
	
	output = clamp(output,0,1);

	return output;
}
ENDCG

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragSpec
		#include "UnityCG.cginc"
		ENDCG
	}
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragShadows
		#include "UnityCG.cginc"
		ENDCG
	}
}

Fallback off

}
