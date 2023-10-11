Shader "Shader Graphs/cloud"
{
    Properties
    {
        _Scroll_Speed("Scroll Speed", Float) = 0.5
        _Color_1("Color 1", Color) = (1, 1, 1, 0)
        _Color_2("Color 2", Color) = (0.5377358, 0.5377358, 0.5377358, 0)
        _Cloud_Cover("Cloud Cover", Float) = 0.5
        _Additional_falloff("Additional falloff", Float) = 0.5
        _Density("Density", Float) = 0.5
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "DisableBatching"="LODFading"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define USE_UNITY_CROSSFADE 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_5a2b7e0c3ead41c8bd0812b62cefe33d_Out_0_Vector4 = _Color_1;
            float4 _Property_4a7ce838b5374ba4ad7938b2952b2b23_Out_0_Vector4 = _Color_2;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float;
            Unity_Saturate_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float);
            float4 _Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4;
            Unity_Lerp_float4(_Property_5a2b7e0c3ead41c8bd0812b62cefe33d_Out_0_Vector4, _Property_4a7ce838b5374ba4ad7938b2952b2b23_Out_0_Vector4, (_Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float.xxxx), _Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4);
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.BaseColor = (_Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4.xyz);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define USE_UNITY_CROSSFADE 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define _ALPHATEST_ON 1
        #define USE_UNITY_CROSSFADE 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define USE_UNITY_CROSSFADE 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP0;
            #endif
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_5a2b7e0c3ead41c8bd0812b62cefe33d_Out_0_Vector4 = _Color_1;
            float4 _Property_4a7ce838b5374ba4ad7938b2952b2b23_Out_0_Vector4 = _Color_2;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float;
            Unity_Saturate_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float);
            float4 _Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4;
            Unity_Lerp_float4(_Property_5a2b7e0c3ead41c8bd0812b62cefe33d_Out_0_Vector4, _Property_4a7ce838b5374ba4ad7938b2952b2b23_Out_0_Vector4, (_Saturate_9f05f72b06ac4ad7bab01a78c076f646_Out_1_Float.xxxx), _Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4);
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.BaseColor = (_Lerp_0149cb2af22d40f487c0416dfe4442f5_Out_3_Vector4.xyz);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_DEPTH_TEXTURE
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 TimeParameters;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Scroll_Speed;
        float4 _Color_1;
        float4 _Color_2;
        float _Cloud_Cover;
        float _Additional_falloff;
        float _Density;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
        {
            float x; Hash_Tchou_2_1_float(p, x);
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }
        
        void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
        {
            float2 p = UV * Scale.xy;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            if (unity_OrthoParams.w == 1.0)
            {
                Out = LinearEyeDepth(ComputeWorldSpacePosition(UV.xy, SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), UNITY_MATRIX_I_VP), UNITY_MATRIX_V);
            }
            else
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float;
            Unity_Subtract_float(_Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, 0.5, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float);
            float3 _Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3 = float3(0, _Subtract_18b463195a66434890c8f559f8bf2f8e_Out_2_Float, 0);
            float3 _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Vector3_137c5ad5d0c64cada3e7aecc2534ca4a_Out_0_Vector3, float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                                     length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                                     length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z))), _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3);
            float3 _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3;
            Unity_Add_float3(IN.WorldSpacePosition, _Multiply_aaff45020b1c4564af091b0a7d333b2f_Out_2_Vector3, _Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3);
            float3 _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3 = TransformWorldToObject(_Add_88164fc6b00f49a58b6d591de7051c52_Out_2_Vector3.xyz);
            description.Position = _Transform_2c59efd4dbca409c9eee0d4cf43112c7_Out_1_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float = _Cloud_Cover;
            float _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float;
            Unity_Multiply_float_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, 2, _Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float);
            float _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float = _Additional_falloff;
            float _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float;
            Unity_Add_float(_Multiply_43c58d3174e147af8cec752c026bc45a_Out_2_Float, _Property_018ca068a0ba47dca3402b6d9d9433ba_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float);
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float = IN.WorldSpacePosition[0];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_G_2_Float = IN.WorldSpacePosition[1];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float = IN.WorldSpacePosition[2];
            float _Split_7e0162b988dc4c85ab8144d4ef025f6f_A_4_Float = 0;
            float2 _Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2 = float2(_Split_7e0162b988dc4c85ab8144d4ef025f6f_R_1_Float, _Split_7e0162b988dc4c85ab8144d4ef025f6f_B_3_Float);
            float _Property_c638379d76714bd98109e586837fa88c_Out_0_Float = _Scroll_Speed;
            float _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_c638379d76714bd98109e586837fa88c_Out_0_Float, _Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float);
            float _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 0.5, _Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float);
            float2 _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_0e0d23494e8946b7b90cbdbd5c6d335e_Out_2_Float.xx), _Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2);
            float _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_501ba437d7a34708beb4d0e43dd8287f_Out_2_Vector2, 0.1, _GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float);
            float _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1, _Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float);
            float2 _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_fd833e77709849aa8401d2647cb47dc2_Out_2_Float.xx), _Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2);
            float _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_74d23a0d2ea8459794ea05b3613bb64e_Out_2_Vector2, 1, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float);
            float _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float;
            Unity_Add_float(_GradientNoise_6dc1b87851b44aadb6100aad46e41513_Out_2_Float, _GradientNoise_69f5ebfea32946cfb617bc4825fe8540_Out_2_Float, _Add_169c667a058e4226b0b55f359033e31a_Out_2_Float);
            float _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_b1dd774dfec94848925beafe59c70e6d_Out_2_Float, 1.5, _Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float);
            float2 _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2;
            Unity_Add_float2(_Vector2_7a734d5e975d4175bb494ebde8ae7a35_Out_0_Vector2, (_Multiply_9aa5e021b59d4a879b5b7008de3ca443_Out_2_Float.xx), _Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2);
            float _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float;
            Unity_GradientNoise_Deterministic_float(_Add_398ec844c6bc4ccab7998edba807e58b_Out_2_Vector2, 0.5, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float);
            float _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float;
            Unity_Multiply_float_float(_Add_169c667a058e4226b0b55f359033e31a_Out_2_Float, _GradientNoise_30dfc125d3e748d0bcfb485f43cd040d_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float);
            float _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float;
            Unity_Smoothstep_float(_Property_eb4473d24ab94964a0e33003fda7e0bb_Out_0_Float, _Add_a37f2f1fd9af4d2aa2103118fd288317_Out_2_Float, _Multiply_dff24ba451244f98a9cc7c9d0c57e046_Out_2_Float, _Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float);
            float _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float;
            Unity_SceneDepth_Eye_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float);
            float4 _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4 = IN.ScreenPosition;
            float _Split_ed7d977b722b4188869acfc600e14bff_R_1_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[0];
            float _Split_ed7d977b722b4188869acfc600e14bff_G_2_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[1];
            float _Split_ed7d977b722b4188869acfc600e14bff_B_3_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[2];
            float _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float = _ScreenPosition_cb09f9dfcaee4b6a99e788a7f254e46e_Out_0_Vector4[3];
            float _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float;
            Unity_Subtract_float(_SceneDepth_58663f43033b4ad48af29f231ec1b6fe_Out_1_Float, _Split_ed7d977b722b4188869acfc600e14bff_A_4_Float, _Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float);
            float _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float = _Density;
            float _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_030246c2d4fc40ae978267064b0ce4c5_Out_2_Float, _Property_1212d4fc7f1d41698702956f5772aa1a_Out_0_Float, _Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float);
            float _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float;
            Unity_Saturate_float(_Multiply_78dd5673a4064079b1bd81a0c293ec3e_Out_2_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float);
            float _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_f7b751e24eb349a4b9da70cdc7d03a84_Out_3_Float, _Saturate_efce74dddd424aee9519a9d861101c25_Out_1_Float, _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float);
            surface.Alpha = _Multiply_5fcc6389192d4e4686979fbfa2d5c23c_Out_2_Float;
            surface.AlphaClipThreshold = 0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
            output.TimeParameters =                             _TimeParameters.xyz;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}