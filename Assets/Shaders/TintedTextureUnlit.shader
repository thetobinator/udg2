Shader "Unlit/TintedTextureUnlit"
{
	Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	cull off

CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	o.Emission = _Color.rgb;
	o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
