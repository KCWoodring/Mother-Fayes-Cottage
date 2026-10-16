Shader "Custom/StirSwirl"
{
    Properties//variables that can be set in the material inspector
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.3
        _Angle ("Angle (radians)", Float) = 0
        _Aspect ("Quad Width / Height", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }//render after opaque objects, but before other transparent objects
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off//don't write to the depth buffer
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;//more variables
            float4 _MainTex_ST;
            float4 _Center;
            float _Radius, _Angle, _Aspect;

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 d = i.uv - _Center.xy;
                d.x *= _Aspect;
                float dist = length(d);
                float mask = 1.0 - smoothstep(_Radius * 0.97, _Radius, dist);

                float s, c;
                sincos(_Angle * mask, s, c);
                float2 r = float2(c * d.x - s * d.y, s * d.x + c * d.y);
                r.x /= _Aspect;

                return tex2D(_MainTex, _Center.xy + r);
            }
            ENDCG
        }
    }
}
