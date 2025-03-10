#region License
//   Copyright 2017 Kastellanos Nikolaos
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
    public class InfiniteGridEffect : Effect //, IEffectMatrices
    {
        #region Effect Parameters

        EffectParameter diffuseColorParam;
        EffectParameter worldViewProjectionParam;
        EffectParameter texelSizeParam;
        EffectParameter invProjectionParam;
        EffectParameter InvViewParam;
        EffectParameter planeNormalParam;
        EffectParameter planeDParam;
        EffectParameter InvPlaneMatrixParam;

        #endregion

        #region Fields

        static readonly string ResourceName = "nkast.Aether.Shaders.Resources.InfiniteGridEffect";

        private static string GetResourceName(GraphicsDevice graphicsDevice, string name)
        {
            string profileName = (graphicsDevice.GraphicsProfile == GraphicsProfile.Reach) ? ".Reach" : ".HiDef";
            string platformName = "";
            string version = "";

#if XNA
            platformName = ".xna";
#else
            switch (graphicsDevice.Adapter.Backend)
            {
                case GraphicsBackend.DirectX11:
                    platformName = ".dx11.fxo";
                    break;
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.GLES:
                case GraphicsBackend.WebGL:
                    platformName = ".ogl.fxo";
                    break;
                default:
                    throw new NotSupportedException("platform");
            }

            // Detect version
            version = ".10";
            Version kniVersion = typeof(Effect).Assembly.GetName().Version;
            if (kniVersion.Major == 3)
            {
                if (kniVersion.Minor == 9)
                {
                    version = ".10";
                }
                if (kniVersion.Minor == 10)
                {
                    version = ".10";
                }
            }
#endif

            return name + profileName + platformName + version;
        }

        internal static byte[] LoadEffectResource(GraphicsDevice graphicsDevice, string name)
        {
            name = GetResourceName(graphicsDevice, name);
            using (Stream stream = typeof(InfiniteGridEffect).Assembly.GetManifestResourceStream(name))
            {
                byte[] bytecode = new byte[stream.Length];
                stream.Read(bytecode, 0, (int)stream.Length);
                return bytecode;
            }
        }

        #endregion
        
        #region Public Properties

        public Vector4 DiffuseColor
        {
            get { return diffuseColorParam.GetValueVector4(); }
            set { diffuseColorParam.SetValue(value); }
        }

        public Vector2 TexelSize
        {
            get { return texelSizeParam.GetValueVector2(); }
            set { texelSizeParam.SetValue(value); }
        }

        public Matrix InvProjection
        {
            get { return invProjectionParam.GetValueMatrix(); }
            set { invProjectionParam.SetValue(value); }
        }

        public Matrix InvView
        {
            get { return InvViewParam.GetValueMatrix(); }
            set { InvViewParam.SetValue(value); }
        }
        public Matrix InvPlaneMatrix
        {
            get { return InvPlaneMatrixParam.GetValueMatrix(); }
            set { InvPlaneMatrixParam.SetValue(value); }
        }

        public Vector3 PlaneNormal
        {
            get { return planeNormalParam.GetValueVector3(); }
            set { planeNormalParam.SetValue(value); }
        }

        public float PlaneD
        {
            get { return planeDParam.GetValueSingle(); }
            set { planeDParam.SetValue(value); }
        }

        #endregion

        #region Methods

        public InfiniteGridEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, LoadEffectResource(graphicsDevice, ResourceName))
        {
            CacheEffectParameters(null);
            worldViewProjectionParam.SetValue(Matrix.Identity);
            DiffuseColor = new Vector4(1.0f, 0.64f, 0.0f, 1.0f);
        }

        public InfiniteGridEffect(GraphicsDevice graphicsDevice, byte[] Bytecode): base(graphicsDevice, Bytecode)
        {   
            CacheEffectParameters(null);
            worldViewProjectionParam.SetValue(Matrix.Identity);
            DiffuseColor = new Vector4(1.0f, 0.64f, 0.0f, 1.0f);
        }

        protected InfiniteGridEffect(InfiniteGridEffect cloneSource) : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);
            worldViewProjectionParam.SetValue(Matrix.Identity);
            
            DiffuseColor    = cloneSource.DiffuseColor;
            TexelSize       = cloneSource.TexelSize;
            InvProjection   = cloneSource.InvProjection;
            InvView         = cloneSource.InvView;
            InvPlaneMatrix  = cloneSource.InvPlaneMatrix;
            //PlaneNormal     = cloneSource.PlaneNormal;
            //PlaneD          = cloneSource.PlaneD;
        }
        
        public override Effect Clone()
        {
            return new InfiniteGridEffect(this);
        }

        void CacheEffectParameters(InfiniteGridEffect cloneSource)
        {
            diffuseColorParam = Parameters["DiffuseColor"];
            worldViewProjectionParam = Parameters["WorldViewProjection"];
            texelSizeParam = Parameters["TexelSize"];
            invProjectionParam = Parameters["InvProjection"];
            InvViewParam = Parameters["InvView"];
            planeNormalParam = Parameters["PlaneNormal"];
            planeDParam = Parameters["PlaneD"];
            InvPlaneMatrixParam = Parameters["InvPlaneMatrix"];
        }

        internal void SetDefaultParameters(Viewport viewport, Matrix projection, Matrix view, Matrix EditMatrix)
        {
            Plane editPlane = new Plane(EditMatrix.Forward, Vector3.Dot(EditMatrix.Forward, EditMatrix.Translation));

            if (this.GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
                TexelSize = new Vector2(1f / viewport.Width, 1f / viewport.Height);
            InvProjection = Matrix.Invert(projection);
            InvView = Matrix.Invert(view);
            InvPlaneMatrix = Matrix.Invert(EditMatrix);
            //PlaneNormal = editPlane.Normal;
            //PlaneD = editPlane.D;
        }

        #endregion
    }
}
