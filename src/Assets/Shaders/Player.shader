Shader "Custom/Player"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Size("Size", Vector) = (1,1,0,0)
        _Radius("Radius", Float) = 0.1
        _WaveAmplitude("Wave Amplitude", Float) = 5
        _WaveSpeed("Wave Speed", Float) = 20
        _AnimTime("Anim  Time", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Size;
            float4 _Color;
            float _Radius;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _AnimTime;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float sdRoundedBoxWave(float2 p, float2 b, float r, float time, float waveAmp, float waveFreq, float waveSpeed)
            {
                float angle = atan2(p.y, p.x);
                float waveR = r + sin(angle * waveFreq + time * waveSpeed) * waveAmp;
                float2 q = abs(p) - b + waveR;
                return length(max(q, 0.0)) - waveR + min(max(q.x, q.y), 0.0);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 size = _Size.xy * 0.5;
                float2 p = (IN.uv - 0.5) * _Size.xy;
                float d = sdRoundedBoxWave(p, size - _Radius, _Radius, _AnimTime, _WaveAmplitude, _WaveFrequency, _WaveSpeed);
                float alpha = smoothstep(0.01, 0.0, d); // antialiasing
                return half4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}