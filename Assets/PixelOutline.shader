Shader "Unlit/PixelOutline"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
        _ShowOutline("Show Outline", Float) = 0 //이게 1이 되면 아웃라인이 뜨도록 설정
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSize;
            float _ShowOutline;
            float4 _MainTex_TexelSize;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float alpha = tex2D(_MainTex, uv).a;

                if (_ShowOutline > 0.5 && alpha == 0)
                {
                    float2 offset = _MainTex_TexelSize.xy * _OutlineSize;
                    float outline = 0.0;

                    outline += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                    outline += tex2D(_MainTex, i.uv - float2(offset.x, 0)).a;
                    outline += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                    outline += tex2D(_MainTex, i.uv - float2(0, offset.y)).a;

                    if (outline > 0)
                        return _OutlineColor;
                }

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
