Shader "Hidden/Contrast" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform half _Contrast;
uniform half _Center;

fixed4 frag (v2f_img i) : SV_Target
{
	fixed4 a = tex2D(_MainTex, i.uv);
	fixed grayscale = Luminance(a.rgb);	

	half c = exp( _Contrast );
	half th = _Center;
	
	fixed4  output = (a-th) * c + th;

	//fixed4 output = pow(a, c) / (pow(a, c) + pow(0.5, c) );
	//output = clamp(output,0.0,1.0);
	
	output = clamp(output,0.0,1.0);
	
	return output;
}
ENDCG

	}
}

Fallback off

}
