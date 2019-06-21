// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33304,y:32786,varname:node_3138,prsc:2;n:type:ShaderForge.SFN_LightVector,id:1474,x:32214,y:32840,varname:node_1474,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:4949,x:32171,y:32984,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:3166,x:32466,y:32897,varname:node_3166,prsc:2,dt:0|A-1474-OUT,B-4949-OUT;n:type:ShaderForge.SFN_Step,id:6248,x:32652,y:32589,varname:node_6248,prsc:2|A-8087-OUT,B-3166-OUT;n:type:ShaderForge.SFN_Tex2d,id:7052,x:32786,y:33187,ptovrint:False,ptlb:Ramp,ptin:_Ramp,varname:node_7052,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b52a2f53b5cd7084baaadadf95c1fb6a,ntxv:0,isnm:False|UVIN-2659-OUT;n:type:ShaderForge.SFN_Vector1,id:8087,x:32439,y:32623,varname:node_8087,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Append,id:2659,x:32569,y:33187,varname:node_2659,prsc:2|A-5236-OUT,B-9957-OUT;n:type:ShaderForge.SFN_Vector1,id:9957,x:32395,y:33221,varname:node_9957,prsc:2,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:2197,x:32540,y:33385,varname:node_2197,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1222-OUT;n:type:ShaderForge.SFN_Transform,id:711,x:32241,y:33299,varname:node_711,prsc:2,tffrom:0,tfto:3|IN-4949-OUT;n:type:ShaderForge.SFN_RemapRange,id:1222,x:32402,y:33442,varname:node_1222,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-711-XYZ;n:type:ShaderForge.SFN_Fresnel,id:5236,x:32864,y:32910,varname:node_5236,prsc:2|EXP-7001-OUT;n:type:ShaderForge.SFN_Slider,id:7595,x:32637,y:32803,ptovrint:False,ptlb:node_7595,ptin:_node_7595,varname:node_7595,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:7001,x:32694,y:32935,varname:node_7001,prsc:2|A-3166-OUT,B-7595-OUT;n:type:ShaderForge.SFN_Color,id:317,x:33079,y:33255,ptovrint:False,ptlb:node_317,ptin:_node_317,varname:node_317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5800,x:33099,y:33096,ptovrint:False,ptlb:node_5800,ptin:_node_5800,varname:node_5800,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:7052-7595-5800-317;pass:END;sub:END;*/

Shader "Shader Forge/Radar" {
    Properties {
        _Ramp ("Ramp", 2D) = "white" {}
        _node_7595 ("node_7595", Range(0, 1)) = 1
        _node_5800 ("node_5800", Float ) = 0
        _node_317 ("node_317", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float3 finalColor = 0;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
