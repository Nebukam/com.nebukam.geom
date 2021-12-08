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
using Unity.Collections;

namespace Nebukam.Geom
{

    public interface IUrquhartProvider : IEdgesProvider
    {
        IVerticesProvider verticesProvider { get; }
        NativeMultiHashMap<int, int> outputConnections { get; }
    }

    /// <summary>
    /// Create voronoi edges based on a given set of input triangles & vertices.
    /// </summary>
    public class UrquhartPass : Processor<UrquhartJob>, IUrquhartProvider
    {

        protected IVerticesProvider m_verticesProvider = null;
        protected ITriadProvider m_triadProvider = null;

        protected NativeList<UIntPair> m_outputEdges = new NativeList<UIntPair>(0, Allocator.Persistent);
        protected NativeMultiHashMap<int, int> m_outputConnections = new NativeMultiHashMap<int, int>(0, Allocator.Persistent);

        /// <summary>
        /// Voronoi edges
        /// </summary>
        public NativeList<UIntPair> outputEdges { get { return m_outputEdges; } }

        /// <summary>
        /// Alternative edge representation
        /// </summary>
        public NativeMultiHashMap<int, int> outputConnections { get { return m_outputConnections; } }

        /// <summary>
        /// Vertices provider used for the Urquhart measuring
        /// </summary>
        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }

        /// <summary>
        /// Triangulation provider used for the Urquhart pass
        /// </summary>
        public ITriadProvider triadProvider { get { return m_triadProvider; } }

        protected override void Prepare(ref UrquhartJob job, float delta)
        {

            if (!TryGetFirstInCompound(out m_verticesProvider)
                || !TryGetFirstInCompound(out m_triadProvider))
            {
                throw new System.Exception("Missing providers");
            }

            m_outputEdges.Clear();
            m_outputConnections.Clear();

            job.inputVertices = m_verticesProvider.outputVertices;

            job.inputTriangles = m_triadProvider.outputTriangles;
            job.inputHullVertices = m_triadProvider.outputHullVertices;
            job.inputUnorderedHull = m_triadProvider.outputUnorderedHull;

            job.outputEdges = m_outputEdges;
            job.outputConnections = m_outputConnections;

        }

        protected override void InternalDispose()
        {
            m_outputEdges.Dispose();
            m_outputConnections.Dispose();
        }

    }
}
