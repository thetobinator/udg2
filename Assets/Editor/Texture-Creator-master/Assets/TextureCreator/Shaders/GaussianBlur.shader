Shader "Hidden/GaussianBlur" {

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
	
	float _BlurRadius;

	float4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy =  v.texcoord.xy;
		
		return o; 
	}
	
	
	float4 fragFull (v2f i) : COLOR
	{
			
		float4 original = tex2D(_MainTex, i.uv);
					
		float4 c =  original;
			
		float tot = 1.0;
		float dx_ =   _MainTex_TexelSize.x/4;
		float dy_ =   _MainTex_TexelSize.y/4;

		float dx, dy, w;

		for(int k=1; k<30;k++){
			for(int j=1; j<30;j++){

				dx = dx_ * j*_BlurRadius*1.0;
				dy = dy_ * k*_BlurRadius*1.0;

				w = exp(-(dx*dx+dy*dy));

				tot += 4*w;

				c +=  w*tex2D(_MainTex, i.uv + float2(+dx, +dy));
				c +=  w*tex2D(_MainTex, i.uv + float2(-dx, +dy));
				c +=  w*tex2D(_MainTex, i.uv + float2(+dx, -dy));
				c +=  w*tex2D(_MainTex, i.uv + float2(-dx, -dy));

			}
		}
		
							
		return c/tot;
	}

	float4 fragH (v2f i) : COLOR
	{
			
		float4 c = tex2D(_MainTex, i.uv);
			
		float tot = 1.0;
		float dx_ =   _MainTex_TexelSize.x;

		float dx, w;
		float sigma2 = _BlurRadius*_BlurRadius/4000;

		for(int k=1; k<60;k++){
			
				dx = dx_ * k;
				w = exp(-(dx*dx)/sigma2);

				tot += 2*w;

				c +=  w*tex2D(_MainTex, i.uv + float2(+dx, 0));
				c +=  w*tex2D(_MainTex, i.uv + float2(-dx, 0));			
		}				
		return c/tot;
	}

	float4 fragV (v2f i) : COLOR
	{
			
		float4 c = tex2D(_MainTex, i.uv);
			
		float tot = 1.0;
		float dx_ = _MainTex_TexelSize.x;

		float dx, w;
		float sigma2 = _BlurRadius*_BlurRadius/4000;

		for(int k=1; k<60;k++){
			
				dx = dx_ * k;
				w = exp(-(dx*dx)/sigma2);

				tot += 2*w;

				c +=  w*tex2D(_MainTex, i.uv + float2(0, +dx));
				c +=  w*tex2D(_MainTex, i.uv + float2(0, -dx));			
		}				
		return c/tot;
	}



	ENDCG
	
Subshader 
{
 Blend One Zero
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragFull
      
      ENDCG
  } // Pass

   Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragH
      
      ENDCG
  } 

    Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragV
      
      ENDCG
  } 

} // Subshader

Fallback off

} // shader

