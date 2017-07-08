// Planet Clouds Surface Shader by Matt Mechtley
// Licensed Under a Creative Commons Attribution license
// http://creativecommons.org/licenses/by/3.0/
Shader "Custom/Clouds" {
	Properties {
		_MainTex ("Alpha (A)", 2D) = "white" {}
		_AtmosRamp ("Atmosphere Ramp (RG or B)", 2D) = "black" {}
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		
		CGPROGRAM
		#pragma surface surf WrapLambert alpha
		
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};
		sampler2D _MainTex;
		sampler2D _AtmosRamp;
	
		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv_Ramp = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Specular = tex2D (_AtmosRamp, uv_Ramp).r;
			o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a + o.Specular;
		}
		
		half4 _RimColor;
		
		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot (s.Normal, lightDir);
			half diffuse = max(0, NdotL* 0.9 + 0.1);
			half4 c;
			c.rgb = (atten * 2) * _LightColor0.rgb * diffuse * (1.0 - s.Specular)
				+ (diffuse * s.Specular * _RimColor);
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
