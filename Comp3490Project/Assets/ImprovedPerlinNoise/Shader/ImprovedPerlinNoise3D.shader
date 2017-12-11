// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Noise/ImprovedPerlinNoise3D" 
{
	SubShader 
	{
	    Pass 
	    {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0
			
			sampler2D _PermTable2D, _Gradient3D;
			float _Frequency, _Lacunarity, _Gain;

			struct v2f 
			{
			    float4 pos : SV_POSITION;
				fixed4 diff : COLOR0; // diffuse lighting color
			    float4 uv : TEXCOORD;
			};
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos(v.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
			    o.uv = v.vertex;
			    return o;
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
				float3 g = tex2D(_Gradient3D, float2(x, 0) ).rgb *2.0 - 1.0;
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
			  	return lerp( lerp( lerp( gradperm(AA.x, p ),  
			                             gradperm(AA.z, p + float3(-1, 0, 0) ), f.x),
			                       lerp( gradperm(AA.y, p + float3(0, -1, 0) ),
			                             gradperm(AA.w, p + float3(-1, -1, 0) ), f.x), f.y),
			                             
			                 lerp( lerp( gradperm(AA.x+one, p + float3(0, 0, -1) ),
			                             gradperm(AA.z+one, p + float3(-1, 0, -1) ), f.x),
			                       lerp( gradperm(AA.y+one, p + float3(0, -1, -1) ),
			                             gradperm(AA.w+one, p + float3(-1, -1, -1) ), f.x), f.y), f.z);
			}
			
			// fractal sum, range -1.0 - 1.0
			float fBm(float3 p, int octaves)
			{
				float freq = _Frequency, amp = 0.5;
				float sum = 0;	
				for(int i = 0; i < octaves; i++) 
				{
					sum += inoise(p * freq) * amp;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			half4 frag (v2f i) : COLOR
			{
				float n = clamp(fBm(i.uv.xyz, 4) + 0.1, 0.1, 1) * i.diff;

			    return half4(n,n,n,1);
			}
			
			ENDCG
	
	    }
	}
	Fallback "VertexLit"
}

