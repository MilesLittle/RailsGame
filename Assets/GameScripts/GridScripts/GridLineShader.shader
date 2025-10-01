Shader "Unlit/GridLineShader"
{
    Properties {
        _Color("Color", Color) = (0,1,1,0.3)
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        Pass {
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            Fog { Mode Off }

            Color [_Color]

            BindChannels {
                Bind "vertex", vertex
                Bind "color", color
            }
        }
    }
}
