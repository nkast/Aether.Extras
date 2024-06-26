#region License
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

namespace nkast.Aether.Animation
{
    public struct Keyframe
    {
        internal int _bone;
        internal TimeSpan _time;
        internal Matrix _transform;
        
        public int Bone 
        { 
            get {return _bone;}
            internal set { _bone = value; }
        }

        public TimeSpan Time
        {
            get { return _time; }
            internal set { _time = value; }
        }

        public Matrix Transform
        {
            get { return _transform; }
            internal set { _transform = value; }
        }
        
        public Keyframe(int bone, TimeSpan time, Matrix transform)
        {
            this._bone = bone;
            this._time = time;
            this._transform = transform;
        }

        public override string ToString()
        {
            return string.Format("{{Time:{0} Bone:{1}}}",
                new object[] { Time, Bone });
        }
    }
}
