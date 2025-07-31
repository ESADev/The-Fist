Shader "UI/BehindObjectsOnlySimple"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color   ("Tint",  Color) = (1,1,1,1)
    }

    SubShader
    {
        // Render after opaque objects but with normal transparency sorting
        Tags { "Queue"="Transparent"  "RenderType"="Transparent"
               "IgnoreProjector"="True"  "PreviewType"="Plane" }

        // 👇 KEY LINES
        ZWrite Off        // don’t touch the depth buffer
        ZTest  Greater    // draw *only* where we’re farther from the camera
        Cull   Off
        Blend  SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D   (_MainTex);  SAMPLER(sampler_MainTex);
            float4      _MainTex_ST;
            float4      _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
            }
            ENDHLSL
        }
    }
}
