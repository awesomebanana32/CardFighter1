Shader "Custom/URP/SimpleFogOfWar_SmoothEdges_Translucent"
{
    Properties
    {
        _MainTex ("Fog Texture", 2D) = "white" {}
        _TexelSize ("Texel Size", Vector) = (1,1,0,0)
        _FogColor ("Fog Color", Color) = (0.02, 0.02, 0.02, 1)
        _FogOpacity ("Fog Opacity", Range(0,1)) = 0.65
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _TexelSize;
            float4 _FogColor;
            float _FogOpacity;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // Sample current alpha
                float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;

                // Sample neighbors for smoothing
                float up    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, _TexelSize.y)).a;
                float down  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(0, _TexelSize.y)).a;
                float left  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(_TexelSize.x, 0)).a;
                float right = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(_TexelSize.x, 0)).a;

                float edge =
                    step(0.01, abs(alpha - up)) +
                    step(0.01, abs(alpha - down)) +
                    step(0.01, abs(alpha - left)) +
                    step(0.01, abs(alpha - right));

                float soften = saturate(edge * 0.25);

                // Smooth edges
                alpha = lerp(alpha, 0.0, soften * 0.5);

                // Global translucency
                alpha *= _FogOpacity;

                return float4(_FogColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
