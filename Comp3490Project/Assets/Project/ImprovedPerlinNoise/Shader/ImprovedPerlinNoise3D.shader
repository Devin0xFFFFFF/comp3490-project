// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cg per-pixel lighting with vertex lights" {
	Properties{
		_Color("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 10
	}
		SubShader{
		Pass{
		Tags{ "LightMode" = "ForwardBase" } // pass for 
											// 4 vertex lights, ambient light & first pixel light

		CGPROGRAM
#pragma multi_compile_fwdbase 
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc" 
		uniform float4 _LightColor0;
	// color of light source (from "Lighting.cginc")

	sampler2D _PermTable2D, _Gradient3D;
	float _Frequency, _Lacunarity, _Gain;

	// User-specified properties
	uniform float4 _Color;
	uniform float4 _SpecColor;
	uniform float _Shininess;

	struct vertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
		float3 vertexLighting : TEXCOORD2;
		float4 noiseuv : TEXCOORD3;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		float4x4 modelMatrix = unity_ObjectToWorld;
		float4x4 modelMatrixInverse = unity_WorldToObject;

		output.posWorld = mul(modelMatrix, input.vertex);
		output.normalDir = normalize(
			mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
		output.pos = UnityObjectToClipPos(input.vertex);

		// Diffuse reflection by four "vertex lights"            
		output.vertexLighting = float3(0.0, 0.0, 0.0);
#ifdef VERTEXLIGHT_ON
		for (int index = 0; index < 4; index++)
		{
			float4 lightPosition = float4(unity_4LightPosX0[index],
				unity_4LightPosY0[index],
				unity_4LightPosZ0[index], 1.0);

			float3 vertexToLightSource =
				lightPosition.xyz - output.posWorld.xyz;
			float3 lightDirection = normalize(vertexToLightSource);
			float squaredDistance =
				dot(vertexToLightSource, vertexToLightSource);
			float attenuation = 1.0 / (1.0 +
				unity_4LightAtten0[index] * squaredDistance);
			float3 diffuseReflection = attenuation
				* unity_LightColor[index].rgb * _Color.rgb
				* max(0.0, dot(output.normalDir, lightDirection));

			output.vertexLighting =
				output.vertexLighting + diffuseReflection;
		}
#endif

		output.noiseuv = input.vertex;

		return output;
	}

	float3 fade(float3 t)
	{
		return t * t * t * (t * (t * 6 - 15) + 10); // new curve
	}

	float4 perm2d(float2 uv)
	{
		return tex2D(_PermTable2D, uv);
	}

	float gradperm(float x, float3 p)
	{
		float3 g = tex2D(_Gradient3D, float2(x, 0)).rgb *2.0 - 1.0;
		return dot(g, p);
	}

	float inoise(float3 p)
	{
		float3 P = fmod(floor(p), 256.0);	// FIND UNIT CUBE THAT CONTAINS POINT
		p -= floor(p);                      // FIND RELATIVE X,Y,Z OF POINT IN CUBE.
		float3 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y,Z.

		P = P / 256.0;
		float3 one = 1.0 / 256.0;

		// HASH COORDINATES OF THE 8 CUBE CORNERS
		float4 AA = perm2d(P.xy) + P.z;

		// AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
		return lerp(lerp(lerp(gradperm(AA.x, p),
			gradperm(AA.z, p + float3(-1, 0, 0)), f.x),
			lerp(gradperm(AA.y, p + float3(0, -1, 0)),
				gradperm(AA.w, p + float3(-1, -1, 0)), f.x), f.y),

			lerp(lerp(gradperm(AA.x + one, p + float3(0, 0, -1)),
				gradperm(AA.z + one, p + float3(-1, 0, -1)), f.x),
				lerp(gradperm(AA.y + one, p + float3(0, -1, -1)),
					gradperm(AA.w + one, p + float3(-1, -1, -1)), f.x), f.y), f.z);
	}

	// fractal sum, range -1.0 - 1.0
	float fBm(float3 p, int octaves)
	{
		float freq = _Frequency, amp = 0.5;
		float sum = 0;
		for (int i = 0; i < octaves; i++)
		{
			sum += inoise(p * freq) * amp;
			freq *= _Lacunarity;
			amp *= _Gain;
		}
		return sum;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		float3 normalDirection = normalize(input.normalDir);
		float3 viewDirection = normalize(
			_WorldSpaceCameraPos - input.posWorld.xyz);
		float3 lightDirection;
		float attenuation;

		if (0.0 == _WorldSpaceLightPos0.w) // directional light?
		{
			attenuation = 1.0; // no attenuation
			lightDirection =
				normalize(_WorldSpaceLightPos0.xyz);
		}
		else // point or spot light
		{
			float3 vertexToLightSource =
				_WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation 
			lightDirection = normalize(vertexToLightSource);
		}

		float3 ambientLighting =
			UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

		float3 diffuseReflection =
			attenuation * _LightColor0.rgb * _Color.rgb
			* max(0.0, dot(normalDirection, lightDirection));

		float3 specularReflection;
		if (dot(normalDirection, lightDirection) < 0.0)
			// light source on the wrong side?
		{
			specularReflection = float3(0.0, 0.0, 0.0);
			// no specular reflection
		}
		else // light source on the right side
		{
			specularReflection = attenuation * _LightColor0.rgb
				* _SpecColor.rgb * pow(max(0.0, dot(
					reflect(-lightDirection, normalDirection),
					viewDirection)), _Shininess);
		}

		float n = clamp(fBm(input.noiseuv.xyz, 4) + 0.1, 0.1, 1);

		return float4((input.vertexLighting + ambientLighting
			+ diffuseReflection + specularReflection) * n, 1.0);
	}
		ENDCG
	}

		Pass{
		Tags{ "LightMode" = "ForwardAdd" }
		// pass for additional light sources
		Blend One One // additive blending 

		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

#include "UnityCG.cginc" 
		uniform float4 _LightColor0;
	// color of light source (from "Lighting.cginc")

	// User-specified properties
	uniform float4 _Color;
	uniform float4 _SpecColor;
	uniform float _Shininess;

	struct vertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float3 normalDir : TEXCOORD1;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		float4x4 modelMatrix = unity_ObjectToWorld;
		float4x4 modelMatrixInverse = unity_WorldToObject;

		output.posWorld = mul(modelMatrix, input.vertex);
		output.normalDir = normalize(
			mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
		output.pos = UnityObjectToClipPos(input.vertex);
		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		float3 normalDirection = normalize(input.normalDir);

		float3 viewDirection = normalize(
			_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
		float3 lightDirection;
		float attenuation;

		if (0.0 == _WorldSpaceLightPos0.w) // directional light?
		{
			attenuation = 1.0; // no attenuation
			lightDirection =
				normalize(_WorldSpaceLightPos0.xyz);
		}
		else // point or spot light
		{
			float3 vertexToLightSource =
				_WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation 
			lightDirection = normalize(vertexToLightSource);
		}

		float3 diffuseReflection =
			attenuation * _LightColor0.rgb * _Color.rgb
			* max(0.0, dot(normalDirection, lightDirection));

		float3 specularReflection;
		if (dot(normalDirection, lightDirection) < 0.0)
			// light source on the wrong side?
		{
			specularReflection = float3(0.0, 0.0, 0.0);
			// no specular reflection
		}
		else // light source on the right side
		{
			specularReflection = attenuation * _LightColor0.rgb
				* _SpecColor.rgb * pow(max(0.0, dot(
					reflect(-lightDirection, normalDirection),
					viewDirection)), _Shininess);
		}

		return float4(diffuseReflection
			+ specularReflection, 1.0);
		// no ambient lighting in this pass
	}

		ENDCG
	}

	}
		Fallback "Specular"
}