Shader "Custom/InvertColor"
{
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Overlay+100" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Blend OneMinusDstColor OneMinusSrcColor
            ColorMask RGB
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Apply UV transformation for custom sprites
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * _TintColor;
            }
            ENDCG
        }
    }
}
