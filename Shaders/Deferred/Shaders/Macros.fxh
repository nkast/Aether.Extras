//-----------------------------------------------------------------------------
// Macros.fxh
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_2_0 vsname (); PixelShader = compile ps_2_0 psname(); } }

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)


#ifdef SM4  // Macros for targetting HLSL 4.0


#define BEGIN_CONSTANTS     cbuffer Parameters : register(b0) {
#define MATRIX_CONSTANTS
#define END_CONSTANTS       };

#define _vs(r)
#define _ps(r)
#define _cb(r)


#else  // Macros for targetting shader model 2.0 (XNA & MojoShader)


#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)


#endif
