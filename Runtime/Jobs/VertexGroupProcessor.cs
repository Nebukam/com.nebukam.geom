// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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

using Nebukam.JobAssist;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Nebukam.Common;

namespace Nebukam.Geom
{

    public interface IVertexGroupProcessor : IProcessor, IVerticesProvider
    {
        IVertexGroup<IVertex> vertices { get; set; }
    }

    public abstract class VertexGroupProcessor<T> : Processor<T>, IVertexGroupProcessor
        where T : struct, IJob
    {

        protected IVertexGroup<IVertex> m_vertices = null;
        public IVertexGroup<IVertex> vertices { get { return m_vertices; } set { m_vertices = value; } }

        protected List<IVertex> m_lockedVertices = new List<IVertex>();

        protected NativeList<float3> m_outputVertices = new NativeList<float3>(0, Allocator.Persistent);
        public NativeList<float3> outputVertices { get { return m_outputVertices; } }

        protected override void InternalLock()
        {
            int count = m_vertices.Count;

            m_lockedVertices.Clear();
            m_lockedVertices.Capacity = count;

            for (int i = 0; i < count; i++) { m_lockedVertices.Add(m_vertices[i]); }

        }

        protected override void Prepare(ref T job, float delta)
        {
            int vCount = m_lockedVertices.Count;
            m_outputVertices.Clear();
            m_outputVertices.Capacity = vCount;

            for (int i = 0; i < vCount; i++)
            {
                m_outputVertices.Add(m_lockedVertices[i].pos);
            }
        }

        protected override void InternalDispose()
        {
            m_vertices = null;
            m_outputVertices.Dispose();
        }

    }
}
