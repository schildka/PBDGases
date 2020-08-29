﻿Shader "Hidden/CalculateFogDensity"
{

	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	CGINCLUDE
	        
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "DistanceFunc.cginc"
            #include "noiseSimplex.cginc"
            
            // compile multiple variants of shaders so switching at runtime is faster
            #pragma multi_compile SHADOWS_ON SHADOWS_OFF
            #pragma shader_feature __ HEIGHTFOG
            #pragma shader_feature __ RAYLEIGH_SCATTERING
            #pragma shader_feature __ HG_SCATTERING
            #pragma shader_feature __ CS_SCATTERING
            #pragma shader_feature __ SCHLICK_HG_SCATTERING
            #pragma shader_feature __ LIMITFOGSIZE
            #pragma shader_feature __ NOISE2D
            #pragma shader_feature __ NOISE3D
            #pragma shader_feature __ SNOISE
            

            UNITY_DECLARE_SHADOWMAP(ShadowMap);
            
            uniform sampler2D _MainTex,
                              _CameraDepthTexture,
                              _NoiseTexture,
                              _BlueNoiseTexture;
                              
            uniform sampler3D _NoiseTex3D;
                              
            uniform float4    _MainTex_TexelSize,
                              _CameraDepthTexture_TexelSize;
                              
            uniform float3    _ShadowColor,
                              _LightColor,
                              _FogColor,
                              _FogWorldPosition,
                              _LightDir,
                              _FogDirection;
                              
            uniform float     _FogDensity,
                              _RayleighScatteringCoef,
                              _MieScatteringCoef,
                              _ExtinctionCoef,
                              _Anisotropy,
                              _kFactor,
                              _LightIntensity,
                              _FogSize,
                              _RaymarchSteps,
                              _AmbientFog,
                              _BaseHeightDensity,
                              _HeightDensityCoef,
                              _NoiseScale,
                              _FogSpeed;
                              
            uniform float4x4  InverseViewMatrix,                   
                              InverseProjectionMatrix;
                              

            #define STEPS _RaymarchSteps
            #define STEPSIZE 1/STEPS
            
            #define e 2.718281828459
            #define pi 3.14159265358
            
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                
                //transform clip pos to view space
                float4 clipPos = float4( v.texcoord * 2.0 - 1.0, 1.0, 1.0); 
                float4 cameraRay = mul(InverseProjectionMatrix, clipPos);
                
                //o.ray = mul(unity_ObjectToWorld, v.vertex);
                o.ray = cameraRay / cameraRay.w;

                return o; 
			}
			
			
			// http://byteblacksmith.com/improvements-to-the-canonical-one-liner-glsl-rand-for-opengl-es-2-0/
	        float rand(float2 co) {
                float a = 12.9898;
                float b = 78.233;
                float c = 43758.5453;
                float dt = dot(co.xy, float2(a, b));
                float sn = fmod(dt, 3.14);
            
                return 2.0 * frac(sin(sn) * c) - 1.0;
            }
			
			// This is the distance field function.  The distance field represents the closest distance to the surface
			// of any object we put in the scene.  If the given point (point p) is inside of an object, we return a
			// negative answer.
			// return : result of distance field
			float map(float3 p) {
			                                                               
				float d_box = sdBox(p - float3(_FogWorldPosition), _FogSize);			
				return d_box;
			}		
			
			
            //-----------------------------------------------------------------------------------------
            // GetCascadeWeights_SplitSpheres
            // from https://github.com/SlightlyMad/VolumetricLights/blob/master/Assets/Shaders/VolumetricLight.shader 
            //-----------------------------------------------------------------------------------------
            inline fixed4 GetCascadeWeights_SplitSpheres(float3 wpos)
            {
                float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
                float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
                float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
                float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
                float4 distances2 = float4(dot(fromCenter0, fromCenter0), dot(fromCenter1, fromCenter1), dot(fromCenter2, fromCenter2), dot(fromCenter3, fromCenter3));
    
                fixed4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
                weights.yzw = saturate(weights.yzw - weights.xyz);
                return weights;
            }
            
            //-----------------------------------------------------------------------------------------
            // GetCascadeShadowCoord
            //-----------------------------------------------------------------------------------------
            inline float4 GetCascadeShadowCoord(float4 wpos, fixed4 cascadeWeights)
            {
                float3 sc0 = mul(unity_WorldToShadow[0], wpos).xyz;
                float3 sc1 = mul(unity_WorldToShadow[1], wpos).xyz;
                float3 sc2 = mul(unity_WorldToShadow[2], wpos).xyz;
                float3 sc3 = mul(unity_WorldToShadow[3], wpos).xyz;
                
                float4 shadowMapCoordinate = float4(sc0 * cascadeWeights[0] + sc1 * cascadeWeights[1] + sc2 * cascadeWeights[2] + sc3 * cascadeWeights[3], 1);
#if defined(UNITY_REVERSED_Z)
                float  noCascadeWeights = 1 - dot(cascadeWeights, float4(1, 1, 1, 1));
                shadowMapCoordinate.z += noCascadeWeights;
#endif
                return shadowMapCoordinate;
            }
	        
	        // https://docs.unity3d.com/Manual/DirLightShadows.html
	        // get the coefficients of each shadow cascade
			fixed4 getCascadeWeights(float z){
			
                float4 zNear = float4( z >= _LightSplitsNear ); 
                float4 zFar = float4( z < _LightSplitsFar ); 
                float4 weights = zNear * zFar; 
                
			    return weights;
			}
			
			// combines cascades to get shadowmap coordinate for later sampling of shadowmap
			fixed4 getShadowCoord(float4 worldPos, float4 weights){
			    float3 shadowCoord = float3(0,0,0);
			    
			    // find which cascades need sampling and then transform the positions to light space using worldtoshadow
			    if(weights[0] == 1){
			        shadowCoord += mul(unity_WorldToShadow[0], worldPos).xyz; 
			    }
			    if(weights[1] == 1){
			        shadowCoord += mul(unity_WorldToShadow[1], worldPos).xyz; 
			    }
			    if(weights[2] == 1){
			        shadowCoord += mul(unity_WorldToShadow[2], worldPos).xyz; 
			    }
			    if(weights[3] == 1){
			        shadowCoord += mul(unity_WorldToShadow[3], worldPos).xyz; 
			    }
			   
                return float4(shadowCoord,1);            
			} 
		
			
			// https://cs.dartmouth.edu/~wjarosz/publications/dissertation/chapter4.pdf
			fixed4 getHenyeyGreenstein(float cosTheta){
			
				float n = 1 - (_Anisotropy * _Anisotropy); // 1 - (g * g)
                float c = cosTheta; // cos(x)
                float d = 1 + _Anisotropy * _Anisotropy - 2 * _Anisotropy * c; // 1 + g^2 - 2g*cos(x)
                return n / (4 * pi * pow(d, 1.5));      
			
			}
			
		    fixed4 getRayleighPhase(float cosTheta){
		        return (3.0 / (16.0 * pi)) * (1 + (cosTheta * cosTheta));
		    }
		    
			
			// gpu pro 6 p. 224
			fixed4 getHeightDensity(float height){
			    float ePow = pow(e, (-height * _HeightDensityCoef));
			    
			    return _BaseHeightDensity * ePow;
			}
			
			
			// http://publications.lib.chalmers.se/records/fulltext/203057/203057.pdf p. 12
			float getCornetteShanks(float costheta){
			     float g2 = _Anisotropy * _Anisotropy;
			     
			     float term1 = (3 * ( 1 -  g2)) / (2 * (2 + g2));
			     
			     float cos2 = costheta * costheta;
			     
			     float term2 = (1 + cos2) / (pow((1 + g2 - 2 * _Anisotropy * cos2),3/2));  
			     return term1 * term2;
			}
			
			// https://media.contentapi.ea.com/content/dam/eacom/frostbite/files/s2016-pbs-frostbite-sky-clouds-new.pdf
			float getSchlickScattering(float costheta){ 
			     float o1 = 1 - (_kFactor * _kFactor);
			     
			     float sqr = (1 + _kFactor * costheta) * (1 + _kFactor * costheta);
			     
			     float o2 = 4 * pi * sqr;
			     
			     return o1/o2;
			}
			
			float getBeerLaw(float density, float stepSize){
			    return saturate(exp(-density * stepSize));	
			}

	
	        float sampleNoise(float3 position){
	            
	            float3 offSet = float3(_Time.yyy) * _FogSpeed * _FogDirection;

	            position *= _NoiseScale;
	            position += offSet;
	            
	            float noiseValue = 0;
   
#if defined(SNOISE)

                noiseValue = snoise(float4(position,_SinTime.y)) * 0.1;
#elif defined(NOISE2D)
                noiseValue = tex2D(_NoiseTexture, position);
#elif defined(NOISE3D)                        
                noiseValue = tex3D(_NoiseTex3D, position);
#endif    
                return noiseValue;   
                            
	        }
			
			// idea for inscattering : https://cboard.cprogramming.com/game-programming/116931-rayleigh-scattering-shader.html
			float getMieScattering(float cosTheta){
			
			    float inScattering = 0;
			    
#if defined(SCHLICK_HG_SCATTERING)
                float Schlickscattering = getSchlickScattering(cosTheta) * _MieScatteringCoef;
                inScattering += Schlickscattering;

#elif defined(HG_SCATTERING)
                float HGscattering = getHenyeyGreenstein(cosTheta) * _MieScatteringCoef;
                inScattering += HGscattering;
                            
#elif defined(CS_SCATTERING) 
                float CSscattering = getCornetteShanks(cosTheta) * _MieScatteringCoef;
                inScattering += CSscattering;
#endif                  

                return inScattering;
                        
			}
			
			float getRayleighScattering(float cosTheta){
			
			    float inScattering = 0;
			    
#if defined(RAYLEIGH_SCATTERING)
                float Rayleighscattering = getRayleighPhase(cosTheta) * _RayleighScatteringCoef;
                inScattering += Rayleighscattering;
#endif

                return inScattering;
			}
			
			
			float getScattering(float cosTheta){
			    
			    float inScattering = 0;
			    
			    inScattering += getMieScattering(cosTheta);
			    
			    inScattering += getRayleighScattering(cosTheta);
			    
			    return inScattering;
			}
				
			fixed4 frag (v2f i) : SV_Target
			{
                
               // read depth and reconstruct world position
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                
                //linearise depth		
                float lindepth = Linear01Depth(depth);
                
                // calculate view position of the pixel	
                float4 viewPos = float4(i.ray.xyz * lindepth,1);
                
                // world position of the pixel
                float3 worldPos = mul(InverseViewMatrix, viewPos).xyz;	
                
                //calculate weights for cascade split selection  
                float4 weights = getCascadeWeights(-viewPos.z);
             
                // ray direction in world space
                float3 rayDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
                  
                float rayDistance = length(worldPos - _WorldSpaceCameraPos.xyz);
                
                //calculate step size for raymarching
                float stepSize = rayDistance / STEPS;
            
                float3 currentPos = _WorldSpaceCameraPos.xyz;
                        
              //  currentPos += _ProjectionParams.y; // start at camera's near plane             
    
                // https://github.com/SlightlyMad/VolumetricLights/blob/master/Assets/Shaders/VolumetricLight.shader
                float2 interleavedPosition = (fmod(floor(i.pos.xy), 8.0));
                float offset = tex2D(_BlueNoiseTexture, interleavedPosition / 8.0 + float2(0.5/8.0, 0.5/8.0)).w;
                
                currentPos += stepSize * rayDir * offset;
                
                float3 litFogColor = _LightIntensity * _FogColor;
                
                float transmittance = 1;
                float extinction = 0;
                float3 result = 0;
                        
                // cosine between the ray direction and the light direction
                // TODO : Will not work with non-directional lights
                float cosTheta = dot(rayDir, _LightDir);
                
                int currentSteps = 0;
                
                [loop]
                for(; currentSteps < STEPS ; currentSteps++)
                {	
                                    			
                    if(transmittance < 0.0000000001){
                        break;
                    }  
                    
                    float distanceSample = 0;
                    
#if defined(LIMITFOGSIZE)
                    distanceSample = map(currentPos); // sample distance field at current position
#endif                    

                    if(distanceSample < 0.0001){ // we are inside the predefined cube

                        float noiseValue = sampleNoise(currentPos);
                        
                        //modulate fog density by a noise value to make it more interesting
                        float fogDensity = noiseValue * _FogDensity;
                        
   
#if defined(HEIGHTFOG)
                        float heightDensity = getHeightDensity(currentPos.y);  
                        fogDensity *= saturate(heightDensity);
#endif                            

                        extinction = _ExtinctionCoef * fogDensity;
                        
                        
                         //calculate transmittance by applying Beer law
                        transmittance *= getBeerLaw(extinction, stepSize);
                        
                        float inScattering = getScattering(cosTheta); 
                        inScattering *= fogDensity;      
#if SHADOWS_ON
                
                     //   float4 weights = GetCascadeWeights_SplitSpheres(currentPos);
                     
                        float4 shadowCoord = getShadowCoord(float4(currentPos,1), weights);
                       // float4 shadowCoord = GetCascadeShadowCoord(float4(currentPos,1), weights);
    
                        //do shadow test and store the result
                        float shadowTerm = UNITY_SAMPLE_SHADOW(ShadowMap, shadowCoord);

                        //use shadow term to lerp between shadowed and lit fog colour, so as to allow fog in shadowed areas,
                        //add a bit of ambient fog so shadowed areas get some lit fog too
                        float3 fColor = lerp(_ShadowColor, litFogColor, shadowTerm + _AmbientFog);                 
#endif

#if SHADOWS_OFF
                        float3 fColor = litFogColor;   
#endif
                        
                        //accumulate light
                       // result += saturate(inScattering) * transmittance * stepSize * fColor;
                        result += inScattering  * stepSize * fColor;
                       // result += inScattering * transmittance * 1/STEPS * fColor; PREMULTIPLIED ALPHA?
                                              
                    }
                    else
                    {
                        result += _FogColor * _LightIntensity;
                    }
   
                    currentPos += rayDir * stepSize; // step forward along ray

                } // raymarch loop   
                     
              return float4(result , transmittance);          

            }  // frag
            

	
	ENDCG
	
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}