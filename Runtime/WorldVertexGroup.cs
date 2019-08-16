// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Nebukam.Utils;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{

    public interface IWorldVertexGroup : IVertexGroup
    {
        void Update();
        float3 pivot { get; set; }
        float3 rotation { get; set; }
    }
    
    public class WorldVertexGroup<T> : VertexGroup<T>, IWorldVertexGroup
        where T : WorldVertex, IVertex, new()
    {

        protected float3 m_pivot = float3(false);
        protected float3 m_rotation = float3(false);

        public float3 pivot { get { return m_pivot; } set { m_pivot = value; } }
        public float3 rotation { get { return m_rotation; } set { m_rotation = value; } }

        protected override void OnVertexAdded(T v)
        {
            base.OnVertexAdded(v);
            v.local = v.pos;
            Transform(v);
        }

        public virtual void Update()
        {
            for (int i = 0, count = m_vertices.Count; i < count; i++)
            {
                Transform(m_vertices[i] as T);
            }
        }

        protected virtual void Transform(T vertex)
        {
            vertex.world = Maths.RotateAroundPivot(vertex.local + m_pivot, m_pivot, m_rotation);
        }

    }
}
