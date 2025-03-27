Shader "Unlit/HeightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SnowHeight ("Snow Height", Range(0.0, 100.0)) = 20.0
        _MountainHeight ("Mountain Height", Range(0.0, 100.0)) = 10.0
        _PlainsHeight ("Plains Height", Range(0.0, 100.0)) = 3.0
        _BlendDistance ("Blend Distance", Range(0.0, 100.0)) = 5.0

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SnowHeight;
            float _MountainHeight;
            float _PlainsHeight;
            float _BlendDistance;
            
            float heightblend(float height, float mid)
            {
                return (height - mid + _BlendDistance) / (_BlendDistance*2);
            }
            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.height = v.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = 0.0;
                fixed4 col;
                fixed4 col_water = fixed4(0.1, 0.1, 0.8, 1.0);
                fixed4 col_plains = fixed4(0.5, 0.5, 0.0, 1.0);
                fixed4 col_mountain = fixed4(0.2, 0.2, 0.3, 1.0);
                fixed4 col_snow = fixed4(1.0, 1.0, 1.0, 1.0);

                ////////////////////////////
                // homework: ADD TEXTURES //
                ////////////////////////////
                
                //snow color
                if (i.height > _SnowHeight + _BlendDistance)
                {
                    col = col_snow;
                }
                //snow & mountain mix
                else if (i.height > _SnowHeight - _BlendDistance)
                {
                    t = heightblend(i.height, _SnowHeight);
                    col = (1-t)*col_mountain + t*col_snow;
                }
                //mountain color
                else if (i.height > _MountainHeight + _BlendDistance)
                {
                    col = col_mountain;
                }
                //mountain & plains mix
                else if (i.height > _MountainHeight - _BlendDistance)
                {
                    t = heightblend(i.height, _MountainHeight);
                    col = (1-t)*col_plains + t*col_mountain;
                }
                //plains color
                else if (i.height > _PlainsHeight - _BlendDistance)
                {
                    col = col_plains;
                }
                //water color
                else
                {
                    col = col_water;
                }
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
