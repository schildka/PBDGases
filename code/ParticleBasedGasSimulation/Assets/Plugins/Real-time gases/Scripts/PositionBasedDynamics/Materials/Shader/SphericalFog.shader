Shader "FX/Spherical Fog" {
	Properties {
		_FogBaseColor ("Fog Base Color", Color) = (0,1,1,1)
		_FogDenseColor ("Fog Dense Color", Color) = (1,1,1,1)
		_InnerRatio ("Inner Ratio", Range (0.0, 0.9999)) = 0.5
		_Density ("Density", Range (0.0, 10.0)) = 10.0
		_ColorFalloff ("Color Falloff", Range (0.0, 50.0)) = 16.0
		
		_ClipPositionY ("Clip Position Y", vector) = (0,10.5,0,0)
		_ClipNormalY ("Clip Normal Y", vector) = (0,1,0,0)  
		
		_ClipPositionX ("Clip Position X", vector) = (6,4,0,0) 
		_ClipNormalX ("Clip Normal X", vector) = (1,0,0,0) 

		_ClipPositionMX ("Clip Position MX", vector) = (-6,4,0,0)  
		_ClipNormalMX ("Clip Normal MX", vector) = (-1,0,0,0)  
		
		_ClipPositionZ ("Clip Position Z", vector) = (0,4,6,0) 
		_ClipNormalZ ("Clip Normal Z", vector) = (0,0,1,0)
		
		_ClipPositionMZ ("Clip Position MZ", vector) = (0,4,-6,0) 
		_ClipNormalMZ ("Clip Normal MZ", vector) = (0,0,-1,0)
	}
	 
	Category {
		Tags { "Queue"="Transparent+99" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off
		ZTest Always
	 	
		SubShader {
		
			Pass {
			
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				 
				static half _Frequency = 10;
                static half _Amplitude = 0.1;
                static half _WaveFalloff = 4;
                static half _MinWaveSize = 1;
                static half _MaxWaveDistortion = 1;
                static half _ImpactSpeed = 0.2;

                // #pragma instancing_options assumeuniformscaling
                UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_INSTANCING_BUFFER_END(Props)

				inline float CalcVolumeFogIntensity (
					float3 sphereCenter,
					float sphereRadius,
					float innerRatio,
					float density,
					float3 cameraPosition,
					float3 viewDirection,
					float maxDistance
				) {
					// Local is the cam position in local space of the sphere.
					float3 local = cameraPosition - sphereCenter;
					
					// Calculate ray-sphere intersection
					float fA = dot (viewDirection, viewDirection);
					float fB = 2 * dot (viewDirection, local);
					float fC = dot(local, local) - sphereRadius * sphereRadius;
					float fD = fB * fB - 4 * fA * fC;
					
					// Early out of no intersection.
					if (fD <= 0.0f)
						return 0;
				 	
					float recpTwoA = 0.5 / fA;
					float DSqrt = sqrt (fD);
					// Distance to front of sphere (0 if inside sphere).
					float dist = max ((-fB - DSqrt) * recpTwoA, 0);
					// Distance to back of sphere
					float dist2 = max ((-fB + DSqrt) * recpTwoA, 0);
				 	
				 	// Max sampling depth should be minimum of back of sphere or solid surface hit.
					float backDepth = min (maxDistance, dist2);
					// Calculate initial sample dist and distance between samples.
					float sample = dist;
					float step_distance = (backDepth - dist) / 10;
					
					// The step_contribution is a value where 0 means no reduction in clarity and
					float step_contribution = (1 - 1 / pow (2, step_distance)) * density;
					
					// Calculate value at the center needed to make the value be 1 at the desired inner ratio.
					float centerValue = 1 / (1 - innerRatio);
					
					// Initially there's no fog, which is full clarity.
					float clarity = 1;
					for ( int seg = 0; seg < 10; seg++ )
					{
						float3 position = local + viewDirection * sample;
						float val = saturate (centerValue * (1 - length (position) / sphereRadius));
						float sample_fog_amount = saturate (val * step_contribution);
						clarity *= (1 - sample_fog_amount);
						sample += step_distance;
					}
					return 1 - clarity;
				}
				 
				fixed4 _FogBaseColor;
				fixed4 _FogDenseColor;
				float _InnerRatio;
				float _Density;
				float _ColorFalloff;
				sampler2D _CameraDepthTexture;
				uniform float4 FogParam;
				uniform float4 _ClipPositionY;
				uniform float4 _ClipNormalY;
				uniform float4 _ClipPositionX;
				uniform float4 _ClipNormalX;
				uniform float4 _ClipPositionMX;
				uniform float4 _ClipNormalMX;
				uniform float4 _ClipPositionZ;
				uniform float4 _ClipNormalZ;
				uniform float4 _ClipPositionMZ;
				uniform float4 _ClipNormalMZ;
				uniform int ClipY;
				uniform int ClipX;
				uniform int ClipMX;
				uniform int ClipZ;
				uniform int ClipMZ;
				
				
				 // Input to vertex shader
                struct vertexInput {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
                
				 
				struct v2f {
					float4 pos : SV_POSITION;
					float3 view : TEXCOORD0;
					float4 projPos : TEXCOORD1;
				};
				 
				 
				v2f vert (appdata_base v) {
					
					v2f o;
					
				    float4 world_space_vertex = mul(unity_ObjectToWorld, v.vertex);
				    float4 origin = float4(1.0 - _Time.y * _ImpactSpeed, 0.0, 0.0, 0.0);
				    
				    
				    //Get the distance in world space from our vertex to the wave origin.                    
                    float dist = max(pow(distance(world_space_vertex, origin), _WaveFalloff), _MaxWaveDistortion);

				    v.vertex.xyz += v.normal * sin(v.vertex.x * _Frequency + _Time.y) * _Amplitude;// *  (1/dist);
				
					
					float4 wPos = mul (unity_ObjectToWorld, v.vertex);
					o.pos = UnityObjectToClipPos (v.vertex);
					o.view = wPos - _WorldSpaceCameraPos;
					o.projPos = ComputeScreenPos (o.pos);
				 	
					// Move projected z to near plane if point is behind near plane.
					float inFrontOf = ( o.pos.z / o.pos.w ) > 0;
					o.pos.z *= inFrontOf;
					
					return o;
				}

				half4 frag (v2f i) : COLOR {
				
					float3 objWorldSpace = i.view + _WorldSpaceCameraPos;

				    // Find if fragment is behind ClippingPlanes				
				    float3 AB = _ClipPositionY - objWorldSpace;
				    float isToClipY = _ClipNormalY.x*AB.x+_ClipNormalY.y*AB.y+_ClipNormalY.z*AB.z;

				    AB = _ClipPositionX - objWorldSpace;
				    float isToClipX = _ClipNormalX.x*AB.x+_ClipNormalX.y*AB.y+_ClipNormalX.z*AB.z;
				    
				    AB = _ClipPositionMX - objWorldSpace;
				    float isToClipMX = _ClipNormalMX.x*AB.x+_ClipNormalMX.y*AB.y+_ClipNormalMX.z*AB.z;

				    AB = _ClipPositionZ - objWorldSpace;
				    float isToClipZ = _ClipNormalZ.x*AB.x+_ClipNormalZ.y*AB.y+_ClipNormalZ.z*AB.z;
				    
				    AB = _ClipPositionMZ - objWorldSpace;
				    float isToClipMZ = _ClipNormalMZ.x*AB.x+_ClipNormalMZ.y*AB.y+_ClipNormalMZ.z*AB.z;
				    
				    // Find if Camera is behind ClippingPlanes 
				    AB = _ClipPositionY - _WorldSpaceCameraPos;
				    float isInsidePlaneY = _ClipNormalY.x*AB.x+_ClipNormalY.y*AB.y+_ClipNormalY.z*AB.z; 

				    AB = _ClipPositionX - _WorldSpaceCameraPos;
				    float isInsidePlaneX = _ClipNormalX.x*AB.x+_ClipNormalX.y*AB.y+_ClipNormalX.z*AB.z; 
				    		   
				    AB = _ClipPositionMX - _WorldSpaceCameraPos;
				    float isInsidePlaneMX = _ClipNormalMX.x*AB.x+_ClipNormalMX.y*AB.y+_ClipNormalMX.z*AB.z; 

				    AB = _ClipPositionZ - _WorldSpaceCameraPos;
				    float isInsidePlaneZ = _ClipNormalZ.x*AB.x+_ClipNormalZ.y*AB.y+_ClipNormalZ.z*AB.z; 
				    
				    AB = _ClipPositionMZ - _WorldSpaceCameraPos;
				    float isInsidePlaneMZ = _ClipNormalMZ.x*AB.x+_ClipNormalMZ.y*AB.y+_ClipNormalMZ.z*AB.z; 
				    
				    // Find if Camera is inside GasSphere 
				    float4 sphereInWorld = FogParam;
				    AB = sphereInWorld.xyz - _WorldSpaceCameraPos;
                    float isInsideSphere = sqrt(AB.x*AB.x + AB.y*AB.y + AB.z*AB.z);
                    
                    if(isInsideSphere > sphereInWorld.w || isInsidePlaneY < 0 && ClipY || isInsidePlaneX < 0 && ClipX || isInsidePlaneMX < 0 && ClipMX || isInsidePlaneZ < 0 && ClipZ || isInsidePlaneMZ < 0 && ClipMZ){
                        if(ClipY)clip(isToClipY);
                        if(ClipX)clip(isToClipX);
                        if(ClipMX)clip(isToClipMX);
                        if(ClipZ)clip(isToClipZ);
                        if(ClipMZ)clip(isToClipMZ);
                    }
                     				
					half4 color = half4 (1,1,1,1);
					float depth = LinearEyeDepth (UNITY_SAMPLE_DEPTH (tex2Dproj (_CameraDepthTexture, UNITY_PROJ_COORD (i.projPos))));
					float3 viewDir = normalize (i.view);
	
					// Calculate fog density.
					float fog = CalcVolumeFogIntensity (
						FogParam.xyz * 0.001,
						FogParam.w * 0.001,
						_InnerRatio,
						_Density * 1000,
						_WorldSpaceCameraPos * 0.001,
						viewDir,
						depth * 0.001);
					
					// Calculate ratio of dense color.
					float denseColorRatio = pow (fog, _ColorFalloff);
					
					// Set color based on denseness and alpha based on raw calculated fog density.
					color.rgb = lerp (_FogBaseColor.rgb, _FogDenseColor.rgb, denseColorRatio);
					
                    color.a = fog * lerp (_FogBaseColor.a, _FogDenseColor.a, denseColorRatio); 

					return color;
				}
				ENDCG
			}
		}
	}
	Fallback "VertexLit"
}
