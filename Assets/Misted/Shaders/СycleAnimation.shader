Shader "Sprites/CycleAnimation"
{
    Properties
    {
        [Header(Texture Sheet)]        
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [Header(Settings)]
        _ColumnsX("Columns (X)", int) = 1
        _RowsY("Rows (Y)", int) = 1
        _AnimationSpeed("Frames Per Seconds", float) = 10
        
        //Compatibility 
        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
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
            uint _ColumnsX;
            uint _RowsY;
            float _AnimationSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // get single sprite size
                float2 size = float2(1.0f / _ColumnsX, 1.0f / _RowsY);
                uint totalFrames = _ColumnsX * _RowsY;

                // use timer to increment index
                uint index = _Time.y * _AnimationSpeed;

                // wrap x and y indexes
                uint indexX = index % _ColumnsX;
                uint indexY = floor((index % totalFrames) / _ColumnsX);

                // get offsets to our sprite index
                float2 offset = float2(size.x * indexX,-size.y * indexY);

                // get single sprite UV
                float2 newUV = v.uv * size;

                // flip Y (to start 0 from top)
                newUV.y = newUV.y + size.y * (_RowsY - 1);

                o.uv = newUV + offset;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}