﻿#region License
//   Copyright 2011-2016 Kastellanos Nikolaos
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using nkast.Aether.Graphics;

namespace nkast.Aether.Animation.Content
{
    public class CpuAnimatedVertexBufferReader : ContentTypeReader<CpuAnimatedVertexBuffer>
    {
        protected override CpuAnimatedVertexBuffer Read(ContentReader input, CpuAnimatedVertexBuffer buffer)
        {
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            GraphicsDevice device = graphicsDeviceService.GraphicsDevice;

            // read standard VertexBuffer
            VertexDeclaration declaration = input.ReadRawObject<VertexDeclaration>();
            int vertexCount = (int)input.ReadUInt32();
            // int dataSize = vertexCount * declaration.VertexStride;
            //byte[] data = new byte[dataSize];
            //input.Read(data, 0, dataSize);

            //read data                      
            VertexElement[] channels = declaration.GetVertexElements();
            var cpuVertices = new VertexIndicesWeightsPositionNormal[vertexCount];
            var gpuVertices = new VertexPositionNormalTexture[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                foreach (VertexElement channel in channels)
                {
                    switch (channel.VertexElementUsage)
                    {
                        case VertexElementUsage.Position:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            Vector3 pos = input.ReadVector3();
                            if (channel.UsageIndex == 0)
                            {
                                cpuVertices[i].Position = pos;
                                gpuVertices[i].Position = pos;
                            }
                            break;

                        case VertexElementUsage.Normal:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            Vector3 nor = input.ReadVector3();
                            if (channel.UsageIndex == 0)
                            {
                                cpuVertices[i].Normal = nor;
                                gpuVertices[i].Normal = nor;
                            }
                            break;

                        case VertexElementUsage.TextureCoordinate:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector2);
                            Vector2 tex = input.ReadVector2();
                            if (channel.UsageIndex == 0)
                            {
                                gpuVertices[i].TextureCoordinate = tex;
                            }
                            break;                            

                        case VertexElementUsage.BlendWeight:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector4);
                            Vector4 wei = input.ReadVector4();
                            if (channel.UsageIndex == 0)
                            {
                                cpuVertices[i].BlendWeights = wei;
                            }
                            break;

                        case VertexElementUsage.BlendIndices:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Byte4);
                            byte i0 = input.ReadByte();
                            byte i1 = input.ReadByte();
                            byte i2 = input.ReadByte();
                            byte i3 = input.ReadByte();
                            if (channel.UsageIndex == 0)
                            {
                                cpuVertices[i].BlendIndex0 = i0;
                                cpuVertices[i].BlendIndex1 = i1;
                                cpuVertices[i].BlendIndex2 = i2;
                                cpuVertices[i].BlendIndex3 = i3;
                            }
                            break;

                        default:
                            throw new Exception();
                    }
                }
            }
            

            // read extras
            bool IsWriteOnly = input.ReadBoolean();

            if (buffer == null)
            {
                BufferUsage usage = (IsWriteOnly) ? BufferUsage.WriteOnly : BufferUsage.None;
                buffer = new CpuAnimatedVertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexCount, usage);
            }

            buffer.SetData(gpuVertices, 0, vertexCount);
            buffer.SetGpuVertices(gpuVertices);
            buffer.SetCpuVertices(cpuVertices);

            return buffer;
        }
    }
}
