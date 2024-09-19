Shader "Custom Render Texture Effect/MaskProcessing"
{
    Properties
    {
    }

    CGINCLUDE
    #include "UnityCustomRenderTexture.cginc"
    
    float   _BrushSize;
    float   _AspectRatio;

    float4 get_clean_res() : COLOR
    {
        float4 color_res;
        color_res.r = 0;
        color_res.g = 1;
        color_res.b = 1;
        color_res.a = 1;
        return color_res;
    }
    ENDCG
    
    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass // 0
        {
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4  _DrawPosition;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float3 real_coord = IN.localTexcoord;
                real_coord.y *= _AspectRatio;
                if (distance(_DrawPosition, real_coord) < _BrushSize)
                {
                    return get_clean_res();
                }
                return tex2D(_SelfTexture2D, IN.localTexcoord.xy);
            }
            ENDCG
        }
        
        Pass // 1
        {
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4  _DrawPositionA;
            float4  _DrawPositionB;
            float   _DistanceAB;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float3 real_coord = IN.localTexcoord;
                real_coord.y *= _AspectRatio;
                float x_ab = _DrawPositionB.x - _DrawPositionA.x;
                float y_ab = _DrawPositionB.y - _DrawPositionA.y;
                float x_ap = _DrawPositionA.x - real_coord.x;
                float y_ap = _DrawPositionA.y - real_coord.y;
                float x_bp = _DrawPositionB.x - real_coord.x;
                float y_bp = _DrawPositionB.y - real_coord.y;
                float dot_ap = x_ap * x_ab + y_ap * y_ab;
                float dot_bp = x_bp * x_ab + y_bp * y_ab;
                if (abs(x_ab * y_ap - x_ap * y_ab) < _DistanceAB * _BrushSize && dot_ap < 0 && dot_bp > 0 ||
                    distance(_DrawPositionB, real_coord) < _BrushSize)
                {
                    return get_clean_res();
                }
                return tex2D(_SelfTexture2D, IN.localTexcoord.xy);
            }
            ENDCG
        }
    }
}
