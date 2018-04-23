
Shader "Toon/Ice Effect" {

	Properties{
		_TColor("Top Color", Color) = (0.64,0.94,0.64,1)// top gradient, light green
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	_BottomColor("Bottom Color", Color) = (0.23,0,0.95,1)// bottom gradient, blue
		_RimBrightness("Rim Brightness", Range(3,4)) = 3.2 // ice rim brightness


	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }

		LOD 200
		CGPROGRAM
#pragma surface surf ToonRamp 

		sampler2D _Ramp;

	// custom lighting function that uses a texture ramp based
	// on angle between light direction and normal
#pragma lighting ToonRamp 
	inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
#endif

		half d = dot(s.Normal, lightDir)*0.5 + 0.5;
		half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
		c.a = 0;
		return c;
	}


	float4 _TColor;
	float4 _BottomColor;// bottom gradient color
	float _RimBrightness;// ice rim brightness


	struct Input {
		float3 viewDir; // view direction
		float3 worldPos; // world position

	};

	void surf(Input IN, inout SurfaceOutput o) {
		float3 localPos = saturate(IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz) + 0.4;// local position of the object, with an offset, clamped to make sure it doesn't go into negative

		float softRim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));// calculate a soft fresnel based on the view direction and the normals of the object
		float hardRim = round(softRim); // round it up for a harder edge
		o.Emission = _TColor * lerp(hardRim, softRim, (localPos.x + localPos.y))  * (_RimBrightness*localPos.y);	 // lerp the emission from the hard rim to the softer one, based on the position
		float innerRim = 1.5 + saturate(dot(normalize(IN.viewDir), o.Normal)); // softer inner rim 

		o.Albedo = _TColor *pow(innerRim, 0.7)*lerp(_BottomColor, _TColor, localPos.y); // multiply the main color by the inner rim, multiply that by the gradient color lerp

	}
	ENDCG

	}

		Fallback "Diffuse"
}