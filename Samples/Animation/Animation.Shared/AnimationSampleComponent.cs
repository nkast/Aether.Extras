﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Animation;
using nkast.Aether.Shaders.Components;


namespace Samples.Animation
{
    internal class AnimationSampleComponent : DrawableGameComponent
    {
        enum DrawMode : int
        {
            GPU,
            CPU,
        }

        ContentManager Content;
        SpriteBatch spriteBatch;
        SpriteFont font;
        InfiniteGridComponent grid;

        Model _model_CPU;
        Model _model_GPU;
        Animations _animations;
        DrawMode drawMode = DrawMode.GPU;

        KeyboardState prevKeyboardState;

        public AnimationSampleComponent(Game game) : base(game)
        {
        }

        /// <summary>Initializes the component. Used to load non-graphical resources.</summary>
        public override void Initialize()
        {
            Content = new ContentManager(Game.Services, "Content");

            base.Initialize();
        }

        /// <summary>Load graphical resources needed by this component.</summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            grid = new InfiniteGridComponent(GraphicsDevice, Content);
            grid.Initialize();

            _model_CPU = Content.Load<Model>("Dude/dude");
            _model_GPU = Content.Load<Model>("Dude/dude_GPU");

            _animations = _model_CPU.GetAnimations(); // Animation Data are the same between the two models
            var clip = _animations.Clips["Take 001"];
            _animations.SetClip(clip);
        }

        /// <summary>Unload graphical resources needed by this component.</summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>Update the component.</summary>
        /// <param name="gameTime">GameTime of the Game.</param>
        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if ((keyboardState.IsKeyDown(Keys.Space) && prevKeyboardState.IsKeyUp(Keys.Space)) || gamePadState.Buttons.A == ButtonState.Pressed)
            {
                int drawModesCount = Enum.GetValues(drawMode.GetType()).Length;
                drawMode = (DrawMode)(((int)drawMode + 1) % drawModesCount);
            }

            prevKeyboardState = keyboardState;

            _animations.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

        }

        private Vector3 Position = Vector3.Zero;
        private float Zoom = 100f;
        private float RotationY = 0.0f;
        private float RotationX = 0.0f;
        private Matrix gameWorldRotation = Matrix.Identity;


        Stopwatch sw = new Stopwatch();
        double msecMin = double.MaxValue;
        double msecMax = 0;
        double avg = 0;
        double acc = 0;
        int c;

        /// <summary>Draw this component.</summary>
        /// <param name="gameTime">The time elapsed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {

            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.01f, 500.0f);
            Matrix view = Matrix.CreateLookAt(
                new Vector3(0.0f, 35.0f, -Zoom),
                new Vector3(0.0f, 35.0f, 0),
                Vector3.Up);

            // Draw Grid
            grid.Projection = projection;
            grid.View = view;
            //grid.EditMatrix = Matrix.Identity; // XY plane
            grid.EditMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-90)); // XZ plane
            grid.Draw(gameTime);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Model m = _model_CPU;
            if (drawMode == DrawMode.CPU)
                m = _model_CPU;
            else if (drawMode == DrawMode.GPU)
                m = _model_GPU;

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            sw.Reset();
            sw.Start();
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    if (drawMode == DrawMode.CPU)
                        ((BasicEffect)part.Effect).SpecularColor = Vector3.Zero;
                    else if (drawMode == DrawMode.GPU)
                        ((SkinnedEffect)part.Effect).SpecularColor = Vector3.Zero;
                    ConfigureEffectMatrices((IEffectMatrices)part.Effect, Matrix.Identity, view, projection);
                    ConfigureEffectLighting((IEffectLights)part.Effect);

                    if (drawMode == DrawMode.CPU)
                        part.UpdateVertices(_animations.AnimationTransforms); // animate vertices on CPU
                    else if (drawMode == DrawMode.GPU)
                        ((SkinnedEffect)part.Effect).SetBoneTransforms(_animations.AnimationTransforms);// animate vertices on GPU
                }
                mesh.Draw();
            }
            sw.Stop();

            double msec = sw.Elapsed.TotalMilliseconds;
            msecMin = Math.Min(msecMin, msec);
            if (avg != 0)
                msecMax = Math.Max(msecMax, msec);
            acc += msec; c++;
            if (c > 60 * 2)
            {
                avg = acc / c;
                acc = c = 0;
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Draw Mode: " + drawMode, new Vector2(32, 32), Color.White);
            spriteBatch.DrawString(font, msec.ToString("#0.000", CultureInfo.InvariantCulture) + "ms", new Vector2(32, GraphicsDevice.Viewport.Height - 130), Color.White);
            spriteBatch.DrawString(font, avg.ToString("#0.000", CultureInfo.InvariantCulture) + "ms (avg)", new Vector2(32, GraphicsDevice.Viewport.Height - 100), Color.White);
            spriteBatch.DrawString(font, msecMin.ToString("#0.000", CultureInfo.InvariantCulture) + "ms (min)", new Vector2(32, GraphicsDevice.Viewport.Height - 70), Color.White);
            spriteBatch.DrawString(font, msecMax.ToString("#0.000", CultureInfo.InvariantCulture) + "ms (max)", new Vector2(32, GraphicsDevice.Viewport.Height - 40), Color.White);
            spriteBatch.End();
        }

        private void ConfigureEffectMatrices(IEffectMatrices effect, Matrix world, Matrix view, Matrix projection)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
        }

        private void ConfigureEffectLighting(IEffectLights effect)
        {
            effect.EnableDefaultLighting();
            effect.DirectionalLight0.Direction = Vector3.Backward;
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight1.Enabled = false;
            effect.DirectionalLight2.Enabled = false;
        }
    }
}
