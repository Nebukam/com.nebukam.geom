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

using Unity.Collections;
using static Nebukam.JobAssist.Extensions;

namespace Nebukam.Geom
{

    /// <summary>
    /// A Delaunay Processor is a triangulator that takes a VertexGroup as input
    /// and output a Delaunay-compliant triangulation.
    /// </summary>
    public class DelaunayProcessor : VertexGroupProcessor<DelaunayJob>, ITriadProvider
    {

        protected bool m_computeTriadCentroid = false;
        protected NativeList<Triad> m_outputTriangles = new NativeList<Triad>(0, Allocator.Persistent);
        protected NativeList<int> m_outputHullVertices = new NativeList<int>(0, Allocator.Persistent);
        protected NativeHashMap<int, UIntPair> m_outputUnorderedHull = new NativeHashMap<int, UIntPair>(0, Allocator.Persistent);

        /// <summary>
        /// The IVerticesProvider used during preparation.
        /// </summary>
        public IVerticesProvider verticesProvider { get { return this; } }

        /// <summary>
        /// Whether or not computing
        /// </summary>
        public bool computeTriadCentroid { get { return m_computeTriadCentroid; } set { m_computeTriadCentroid = value; } }

        /// <summary>
        /// Generated triangulation
        /// </summary>
        public NativeList<Triad> outputTriangles { get { return m_outputTriangles; } }
        public NativeList<int> outputHullVertices { get { return m_outputHullVertices; } }
        public NativeHashMap<int, UIntPair> outputUnorderedHull { get { return m_outputUnorderedHull; } }

        protected override void Prepare(ref DelaunayJob job, float delta)
        {
            base.Prepare(ref job, delta);

            //Clear previously built triangles
            m_outputTriangles.Clear();
            m_outputHullVertices.Clear();
            m_outputUnorderedHull.Clear();

            job.inputVertices = m_outputVertices;
            job.computeTriadCentroid = m_computeTriadCentroid;
            job.outputTriangles = m_outputTriangles;
            job.outputHullVertices = m_outputHullVertices;
            job.outputUnorderedHullEdges = m_outputUnorderedHull;
        }

        protected override void Apply(ref DelaunayJob job)
        {

        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            m_outputTriangles.Release();
        }

    }
}
