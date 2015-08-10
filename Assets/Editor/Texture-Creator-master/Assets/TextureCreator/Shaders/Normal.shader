Shader "Hidden/Normal" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE
		
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
		
	sampler2D _MainTex;
	
	float _NormalCoef;
	float _Offset;

	float4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy =  v.texcoord.xy;
		
		return o; 
	}
	
	float4 gray(float4 a){
		return (a.r + a.g + a.b)/3;
	}

	
	fixed3 frag (v2f i) : COLOR
	{
			
										
		float dx =   _MainTex_TexelSize.x;
		float dy =   _MainTex_TexelSize.y;

		float4 gx,gy,gxb,gyb;

		gx = tex2D(_MainTex, i.uv + float2(+dx, 0));
		gy = tex2D(_MainTex, i.uv + float2(0, +dy));

		gxb = tex2D(_MainTex, i.uv + float2(-dx, 0));
		gyb = tex2D(_MainTex, i.uv + float2(0, -dy));

		gx  = saturate( gray(gx + _Offset) );
		gy  = saturate( gray(gy + _Offset) );
		gxb = saturate( gray(gxb + _Offset) );
		gyb = saturate( gray(gyb + _Offset) );


		gx = -(gx - gxb);
		gy = -(gy - gyb);

		fixed3 output = fixed3(0,0,0);

		float A = sqrt( gx*gx + gy*gy + 1 );
			
		if(A>0){
			output.r = 10*_NormalCoef*gx/A/2+0.5 ;
			output.g = 10*_NormalCoef*gy/A/2+0.5 ;
			output.b = 1/A ;
		} 
							
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