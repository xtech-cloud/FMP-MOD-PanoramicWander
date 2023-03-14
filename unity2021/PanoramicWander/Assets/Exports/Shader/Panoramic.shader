Shader "XTC_PanoramicWander/Panoramic" {
	Properties {
		_MainTex    ("Main", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
        ZWrite On
		Cull Front
        Lighting Off

		CGPROGRAM
		#pragma surface surf Unlit noambient 
		#pragma target 3.0

		sampler2D _MainTex;

        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) 
        {
            return fixed4(s.Albedo, s.Alpha);
        }

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float4 vertex: POSITION;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
            IN.uv_MainTex = float2(1.0-IN.uv_MainTex.x, IN.uv_MainTex.y);
			float4 tc = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tc.rgb;
			o.Alpha = tc.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
