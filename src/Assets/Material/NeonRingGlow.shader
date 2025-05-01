Shader "Custom/NeonRingGlowFixed"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0.1, 1, 0.4, 1)
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 2
        _GlowWidth ("Glow Width", Range(0.01,5)) = 0.15
        _TimeSpeed ("Glow Pulse Speed", Range(0.1, 5)) = 1
        _RingRadius ("Ring Radius", Range(0,5)) = 0.32
        _RingThickness ("Ring Thickness", Range(0.01,0.25)) = 0.08
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowWidth;
            float _TimeSpeed;
            float _RingRadius;
            float _RingThickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Градиент для ореола
            float NeonGlow(float2 uv, float ringR, float ringT, float glowW, float time)
            {
                float2 center = float2(0.5,0.5);
                float dist = distance(uv, center);

                float outer = ringR + ringT/2;
                float glowStart = outer;
                float glowEnd = outer + glowW;

                float glow = 1.0 - smoothstep(glowStart, glowEnd, dist);
                float pulse = 0.55 + 0.45 * sin(time + dist*7); // добавить динамики

                return clamp(glow * pulse, 0.0, 1.0);
            }

            // Фрагмент
            fixed4 frag (v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);

                float time = _Time.y * _TimeSpeed;

                float ringR = _RingRadius;
                float ringT = _RingThickness;
                float glowW = _GlowWidth;

                float2 center = float2(0.5,0.5);
                float dist = distance(i.uv, center);
                float outer = ringR + ringT/2;

                // Свечение выводится только снаружи (альфа PNG мала)
                float useGlow = step(outer, dist); // 1 если dist>=outer, 0 иначе

                float glow = NeonGlow(i.uv, ringR, ringT, glowW, time) * _GlowIntensity * useGlow;

                // Берём просто исходный пиксель и накладываем только свечение
                float4 final = baseCol;
                final.rgb += _GlowColor.rgb * glow;
                final.a = max(baseCol.a, glow * _GlowColor.a);

                return final;
            }
            ENDCG
        }
    }
}