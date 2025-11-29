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
    public class DeferredBasicEffect : Effect, IEffectMatrices
    {
        #region Effect Parameters
            
        // IEffectMatrices
        EffectParameter projectionParam;
        EffectParameter viewParam;
        EffectParameter worldParam;

        #endregion

        #region Fields


        static readonly string ResourceName = "nkast.Aether.Shaders.Resources.DeferredBasicEffect";

        internal static byte[] LoadEffectResource(GraphicsDevice graphicsDevice, string name)
        {
            name = GetResourceName(graphicsDevice, name);
            using (Stream stream = typeof(DeferredBasicEffect).Assembly.GetManifestResourceStream(name))
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

        public Matrix Projection
        {
            get { return projectionParam.GetValueMatrix(); }
            set { projectionParam.SetValue(value); }
        }

        public Matrix View
        {
            get { return viewParam.GetValueMatrix(); }
            set { viewParam.SetValue(value); }
        }

        public Matrix World
        {
            get { return worldParam.GetValueMatrix(); }
            set { worldParam.SetValue(value); }
        }

        #endregion

        #region Methods

         public DeferredBasicEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, LoadEffectResource(graphicsDevice, ResourceName))
        {    
            CacheEffectParameters(null);
        }

        public DeferredBasicEffect(GraphicsDevice graphicsDevice, byte[] Bytecode): base(graphicsDevice, Bytecode)
        {   
            CacheEffectParameters(null);
        }

        protected DeferredBasicEffect(DeferredBasicEffect cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);
        }
        
        public override Effect Clone()
        {
            return new DeferredBasicEffect(this);
        }

        void CacheEffectParameters(DeferredBasicEffect cloneSource)
        {    

            // IEffectMatrices
            projectionParam = Parameters["Projection"];
            viewParam = Parameters["View"];
            worldParam = Parameters["World"];
        }
        
        #endregion
    }
}
