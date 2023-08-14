Shader "PPUI/Rectangle"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderWidth ("Border Width", Range(0, 0.1)) = 0.01
        _CornerRadius ("Corner Radius", Float) = 0.01
        
        _RectWidth ("Rect Width", Float) = 100
        _RectHeight ("Rect Height", Float) = 100
        
        // --- Mask support ---
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
        Tags { "RenderType"="Transparent" "PreviewType"="Plane" "Queue"="Transparent" "IgnoreProjector"="True" }
        
        // --- Mask support ---
         Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
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
            #pragma vertex vert_img
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 world_position : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert_img (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.world_position = v.vertex;
                return o;
            }

            float _RectWidth;
            float _RectHeight;
            float _CornerRadius;
            float _BorderWidth;
            float4 _MainColor;
            float4 _BorderColor;
            
            float4 _ClipRect;
            
            fixed4 frag (v2f i) : SV_Target
            {
                const float xPos = abs(i.uv.x) * _RectWidth;
                const float yPos = abs(i.uv.y) * _RectHeight;
                const float xPosInversed = _RectWidth - xPos;
                const float yPosInversed = _RectHeight - yPos;
                const float xPosCorner = xPos - (_RectWidth - _CornerRadius);
                const float yPosCorner = yPos - (_RectHeight - _CornerRadius);
                float distance;
                // Inner rectangle
                if (xPosInversed > _CornerRadius || yPosInversed > _CornerRadius)
                    distance = min(xPosInversed, yPosInversed);
                // Corner segment distance from closest point on rounded corner
                else
                    distance = _CornerRadius - sqrt(xPosCorner * xPosCorner + yPosCorner * yPosCorner);
                if (distance < 0)
                    discard;

                float4 color;
                
                if (distance < _BorderWidth)
                    color = _BorderColor;
                else
                    color = _MainColor;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.world_position.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                
                return color;
            }
            ENDCG
        }
    }
}
