Shader "Hidden/Heightmap" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform half _Coef;
uniform half _Center;
uniform half _Min;
uniform half _Max;

float4 frag (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	

	float mi = min(a.r,a.b);
	mi = min(mi,a.g);
	mi = Luminance(a);

	mi = (a.r+a.g+a.b)/3;
	//mi = mi*_Coef*2;

	_Coef = exp(_Coef);
	mi = (mi-_Center) *_Coef + _Center;

	float4 output = mi;

	output = clamp(output,0,1);

	//_Max = max(_Max,_Min);

	output = output * (_Max - _Min) + _Min;


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

}

Fallback off

}