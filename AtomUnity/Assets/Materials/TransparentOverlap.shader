Shader "Unlit/TransparentOverlap"
{
    Properties
    {
        _Color("Main Color, Alpha", Color) = (1,1,1,1)
    }
	Category{
		ZWrite Off
		Tags {Queue = Transparent}
		//Blend One One
		//Blend DstColor OneMinusSrcAlpha //??? no idea why
		//Blend DstAlpha OneMinusSrcColor
		Blend DstAlpha SrcColor
		Color[_Color]
		SubShader {
			Pass {
				Cull Back
			}
		}
	}
}
