#region License
//   Copyright 2013-2016 Kastellanos Nikolaos
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
using Microsoft.Xna.Framework.Graphics;

namespace tainicom.Aether.Shaders
{
    public class FXAAGreenLumaLowEffect : FXAAEffect 
    {
#if ((MG && WINDOWS) || W8_1 || W10)
        static readonly String resourceName = "tainicom.Aether.Shaders.Resources.FXAAGreenLumaLow.dx11.mgfxo";
#elif (XNA && WINDOWS)
        static readonly String resourceName = "tainicom.Aether.Shaders.Resources.FXAAGreenLumaLow.xna.WinHiDef";
#endif

        public FXAAGreenLumaLowEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, LoadEffectResource(resourceName))
        {    
        }        
    }
}