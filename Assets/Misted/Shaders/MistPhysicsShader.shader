Shader "Custom Render Texture Effect/MistPhysics"
{
    Properties
    {
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            sampler2D _TexDirt;
            sampler2D _Mask;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                fixed2 uv_dirt = IN.localTexcoord.xy;
                float4 color_dirt = tex2D(_TexDirt, uv_dirt);
                fixed2 uv_mask = IN.localTexcoord.xy;
                float4 color_mask = tex2D(_Mask, uv_mask);
                float4 color_res = color_dirt;
                color_res.a = color_mask.r;

                return color_res;
            }
            ENDCG
        }
    }
}
