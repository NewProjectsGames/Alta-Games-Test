// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Shaders Test/Woobling Jelly" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    _Metallic ("Metallic", Range(0,1)) = 0.0
        _Warp ("Warp Strenght", Range(0,1000)) = 50		//How much woobly is the material?

  }

  SubShader {
    Tags {
           	"Queue" = "Transparent"
           	"IgnoreProjector" = "True"
           	"RenderType" = "Transparent"
    }
    LOD 200
    
    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard fullforwardshadows vertex:vert alpha

    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

    sampler2D _MainTex;
    float _Warp;

    struct Input {
      float2 uv_MainTex;
      float3 vertColor;
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
    };

    Input vert (inout appdata_full v)
    {
      Input o;

            v.vertex.x += sign(v.vertex.x) * sin(_Time.w)/_Warp;
      v.vertex.y += sign(v.vertex.y) * cos(_Time.w)/_Warp;
      
      o.vertColor = float3(v.vertex.x, v.vertex.y, v.vertex.z);
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;

      return o;
    }

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
      // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

    void surf (Input IN, inout SurfaceOutputStandard o) {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      // Metallic and smoothness come from slider variables
      o.Metallic = _Metallic;
      o.Smoothness = _Glossiness;
      //o.Alpha = c.a;
      o.Alpha = _Color.a;
    }
    ENDCG
  }
  FallBack "Diffuse"
}