Shader "XTC_PanoramicWander/ClipPanoramic" {
	Properties {
		_MainTex    ("Main", 2D) = "black" {}
		_NoiseTex   ("Noise", 2D) = "black" {}
		_Clip       ("Clip", Float) = 1
		_Halo       ("Halo", Float) = 0.5
		_HaloColor  ("Halo Color", Color) = (1, 1, 0, 1)
		_Bloom      ("Bloom", Float) = 1.5
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
        ZWrite On
		Cull Front
        Lighting Off

		CGPROGRAM
		#pragma surface surf Unlit noambient finalcolor:teleport
		#pragma target 3.0

		sampler2D _MainTex, _NoiseTex;
		half _Bloom, _Clip, _Halo, _Metallic, _Glossiness;
		half4 _HaloColor;

		float HTClipFrag (float3 wldpos, float2 uv)
		{
			float dt = wldpos.y - _Clip;

			float3 ns3 = tex2D(_NoiseTex, uv).rgb;
			float ns = (ns3.x + ns3.y + ns3.z) / 3.0;
			dt += ns;

			float s = step(abs(dt), _Halo);
			float f = max(dt, 0.0);

			clip(0.01 - f);
			return s;
		}

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
		void teleport (Input IN, SurfaceOutput o, inout fixed4 color)
		{
			float s = HTClipFrag(IN.worldPos, IN.uv_MainTex);
			color.rgb = lerp(color.rgb, _HaloColor.rgb * _Bloom, s);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
