#region License
//   Copyright 2014-2016 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nkast.Aether.Shaders
{
    public class DeferredCombineEffect : Effect
    {
        #region Effect Parameters

        EffectParameter colorMapParam;
        EffectParameter lightMapParam;
        EffectParameter halfPixelParam;

        #endregion

        #region Fields


        static readonly string ResourceName = "nkast.Aether.Shaders.Resources.DeferredCombine";

        internal static byte[] LoadEffectResource(GraphicsDevice graphicsDevice, string name)
        {
            name = GetResourceName(graphicsDevice, name);
            using (Stream stream = typeof(DeferredCombineEffect).Assembly.GetManifestResourceStream(name))
            {
                byte[] bytecode = new byte[stream.Length];
                stream.Read(bytecode, 0, (int)stream.Length);
                return bytecode;
            }
        }

        private static string GetResourceName(GraphicsDevice graphicsDevice, string name)
        {
            string platformName = "";
            string version = "";

#if XNA
            platformName = ".xna.WinReach";
#else
            switch (graphicsDevice.Adapter.Backend)
            {
                case GraphicsBackend.DirectX11:
                    platformName = ".dx11.fxo";
                    break;
                case GraphicsBackend.OpenGL:
                    platformName = ".ogl.fxo";
                    break;
                case GraphicsBackend.GLES:
                case GraphicsBackend.WebGL:
                    platformName = ".gles.fxo";
                    break;
                default:
                    throw new NotSupportedException("Backend");
            }

            // Detect version
            version = ".11";
            Version kniVersion = typeof(Effect).Assembly.GetName().Version;
            {
                if (kniVersion.Minor ==  9
                ||  kniVersion.Minor == 10
                ||  kniVersion.Minor == 11
                ||  kniVersion.Minor == 12
                ||  kniVersion.Minor == 13
                ||  kniVersion.Minor == 14)
                    version = ".10";
            }
            if (kniVersion.Major == 4)
            {
                if (kniVersion.Minor == 0
                ||  kniVersion.Minor == 1)
                    version = ".10";
                if (kniVersion.Minor == 2)
                    version = ".11";
            }
#endif

            return name + platformName + version;
        }

        #endregion
        
        #region Public Properties

        public RenderTarget2D ColorMap
        {
            get { return colorMapParam.GetValueTexture2D() as RenderTarget2D; }
            set { colorMapParam.SetValue(value); }
        }

        public RenderTarget2D LightMap
        {
            get { return lightMapParam.GetValueTexture2D() as RenderTarget2D; }
            set { lightMapParam.SetValue(value); }
        }

        public Vector2 HalfPixel
        {
            get { return halfPixelParam.GetValueVector2(); }
            set { halfPixelParam.SetValue(value); }
        }

        #endregion

        #region Methods

         public DeferredCombineEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, LoadEffectResource(graphicsDevice, ResourceName))
        {
            CacheEffectParameters(null);
        }

        public DeferredCombineEffect(GraphicsDevice graphicsDevice, byte[] Bytecode): base(graphicsDevice, Bytecode)
        {   
            CacheEffectParameters(null);
        }

        protected DeferredCombineEffect(DeferredCombineEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);
        }
        
        public override Effect Clone()
        {
            return new DeferredCombineEffect(this);
        }

        void CacheEffectParameters(DeferredCombineEffect cloneSource)
        {
            colorMapParam = Parameters["colorMap"];
            lightMapParam = Parameters["lightMap"];
            halfPixelParam = Parameters["halfPixel"];
        }
        
        #endregion
    }
}
