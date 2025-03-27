Shader "Unlit/CrudeTerrain"
{
    Properties
    {
        _PlainTex ("Plains Texture", 2D) = "white" {}
        _MountainTex ("Mountain Texture", 2D) = "white" {}
        _SnowTex ("Snow Texture", 2D) = "white" {}
        _WaterTex ("Water Texture", 2D) = "white" {}
        [NoScaleOffset] _FlowTex ("Flow Map", 2D) = "black" {}
        _MaxHeight ("Max Height", Range(0.0, 1000.0)) = 100.0
        _SnowHeight ("Snow Height", Range(0.0, 1000.0)) = 100.0
        _MountainHeight ("Mountain Height", Range(0.0, 1000.0)) = 100.0
        _WaterHeight ("Water Height", Range(0.0, 1000.0)) = 10.0
        _BlendDistance ("Blend Distance", Range(0.0, 100.0)) = 20.0
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float height : TEXCOORD1;
            };

            sampler2D _PlainTex;
            sampler2D _MountainTex;
            float4 _MountainTex_ST;  //  What is this?!?!?
            sampler2D _SnowTex;
            sampler2D _WaterTex;
            sampler2D _FlowTex;
            float _MaxHeight;
            float _WaterHeight;
            float _MountainHeight;
            float _SnowHeight;
            float _BlendDistance;


            float heightblend(float height, float mid)
            {
                return (height - mid + _BlendDistance) / (_BlendDistance*2);
            }
            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            float3 FlowUVW (float2 uv, float2 flowVector, float time, bool bFlag) {
                float phaseOffset = bFlag ? 0.5 : 0.0;
                float progress = frac(time + phaseOffset);
                
	            float3 uvw;
	            uvw.xy = uv - flowVector * progress;
	            uvw.z = 1-abs(1-2*progress);
	            return uvw;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                //if (v.vertex.y < _WaterHeight)
                //{
                //    v.vertex.y += sin(_Time)*15.0f;
                //}
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MountainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.height = v.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col_water = tex2D(_WaterTex, i.uv);
                fixed4 col_low = tex2D(_PlainTex, i.uv);
                fixed4 col_rock = tex2D(_MountainTex, i.uv);
                fixed4 col_snow = tex2D(_SnowTex, i.uv);

                fixed4 col;
                float t = 0.0;
                if (i.height > _SnowHeight + _BlendDistance)
                {
                    col = col_snow;
                    col.a = 1;
                    //col = fixed4(1,1,1,1);
                }
                else if (i.height > _SnowHeight - _BlendDistance)
                {
                    t = heightblend(i.height, _SnowHeight);
                    col = (1-t)*col_rock + t*col_snow;
                    
                    //col = fixed4(t,t,t,t);
                }
                else if (i.height > _MountainHeight + _BlendDistance)
                {
                    col = col_rock;
                    //col = fixed4(1,1,0,1);
                }
                else if (i.height > _MountainHeight - _BlendDistance){
                    t = heightblend(i.height, _MountainHeight);
                    col = (1-t)*col_low + t*col_rock;
                    //col = fixed4(1,0,0,1);
                }
                else if (i.height > _WaterHeight + _BlendDistance)
                {
                    col = col_low;
                }
                 else if (i.height > _WaterHeight - _BlendDistance)
                 {
                     t = heightblend(i.height, _WaterHeight);
                     t = clamp(t,0.0,1.0);
                     t = 1 - (1-t)*(1-t);
                     col = (1-t)*col_water + t*col_low;
                 }
                else {
                    //col = col_water;
                    //col = tex2D(_WaterTex, i.uv);
                    // Water effect???
                    //fixed4 col_rand1 = tex2D(_WaterTex, float2(sin(i.uv.x+_Time.y/150.f),
                    //                                          sin(i.uv.y+_Time.y/100.0f)));
                    //fixed4 col_rand2 = tex2D(_WaterTex, float2(sin(i.uv.x+_Time.y/100.f),
                    //                                          sin(i.uv.y+_Time.y/150.0f)));
                    //col = col_rand1 + col_rand2;
                    //col = normalize(col);

                    float2 flowVector = tex2D(_FlowTex, i.uv).rg * 2 - 1;
                    float noise = tex2D(_FlowTex, i.uv).a;
                    float time = _Time.y + noise;
                    float3 uvwA = FlowUVW(i.uv, flowVector, time*0.1f, true);
                    float3 uvwB = FlowUVW(i.uv, flowVector, time*0.1f, false);
			        //col = tex2D(_WaterTex, uvwA.xy) * uvwA.z;
			        col = tex2D(_WaterTex, uvwA.xy) * uvwA.z + tex2D(_WaterTex, uvwB.xy) * uvwB.z;
                    //col = fixed4(uvw.x, uvw.y, uvw.z, 0);
                    //col = fixed4(0,0,1,1);
                }
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
