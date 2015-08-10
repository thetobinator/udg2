Shader "Hidden/HSV" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE
	
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform half _SaturationUniform;
uniform half _ValueUniform;

uniform half _Hue;
uniform half _Saturation;
uniform half _Value;

float4 rgb2hsv(float4 c)
{
	float4 output;
	float mi, ma, delta;
	mi = min(min( c.r, c.g), c.b );
	ma = max(max( c.r, c.g), c.b );
	output.b = ma;		 		// v
	
	delta = ma - mi;

	if( ma != 0 )
		output.g = delta / ma;		// s
	else {
		// r = g = b = 0		
		output.r = 0;
		output.g = 0;
		output.b = 0;
	}
	if( c.r == ma )
		output.r = (c.g - c.b ) / delta;		// between yellow & magenta
	else if( c.g == ma )
		output.r = 2 + ( c.b - c.r ) / delta;	// between cyan & yellow
	else
		output.r = 4 + ( c.r - c.g ) / delta;	// between magenta & cyan

	output.r = output.r/6;
	if(output.r < 0)
		output.r += 1;

	return output;

}

float4 hsv2rgb( float4 a ){

	float4 output;

	int i;
	float f, p, q, t;
	if( a.g == 0 ) {
		// aca.hromatic (grey)
		output = a.b;
		return output;
	}
	a.r *= 6;			// sector 0 to 5
	i = floor( a.r );
	f = a.r - i;			// factorial part of a.r
	p = a.b * ( 1 - a.g );
	q = a.b * ( 1 - a.g * f );
	t = a.b * ( 1 - a.g * ( 1 - f ) );
	if( i == 0 ){
		output.r = a.b;
		output.g = t;
		output.b = p;
	}
	if( i == 1 ){
		output.r = q;
		output.g = a.b;
		output.b = p;
	}
	if( i == 2 ){
		output.r = p;
		output.g = a.b;
		output.b = t;
	}
	if( i == 3 ){
		output.r = p;
		output.g = q;
		output.b = a.b;
	}
	if( i == 4 ){
		output.r = t;
		output.g = p;
		output.b = a.b;
	}
	if( i == 5 ){
		output.r = a.b;
		output.g = p;
		output.b = q;
	}
	
	return output;
} 


float4 fragRgb2hsv (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	float4 output = rgb2hsv(a);
	
	return output;
}

float4 fragHsv2rgb (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	float4 output = hsv2rgb(a);
	
	return output;
}

float4 fragGetColor (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	float4 output = a;
	output.g = _SaturationUniform;
	output.b = _ValueUniform;
	output = hsv2rgb(output);
	
	return output;
}

float4 fragApplyCoef (v2f_img i) : SV_Target
{
	float4 a = tex2D(_MainTex, i.uv);
	a.r +=  _Hue;
	a.g *=  exp(_Saturation);
	a.b *=  exp(_Value);

	if( a.r > 1.0 ){
		a.r -= 1.0;
	}

	return a;
}

ENDCG

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragRgb2hsv
		#include "UnityCG.cginc"
		ENDCG
	}
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragHsv2rgb
		#include "UnityCG.cginc"
		ENDCG

	}
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragGetColor
		#include "UnityCG.cginc"
		ENDCG
	}
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment fragApplyCoef
		#include "UnityCG.cginc"
		ENDCG
	}

	
}

Fallback off

}
