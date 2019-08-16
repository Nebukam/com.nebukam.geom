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

using Nebukam.JobAssist;
using Unity.Collections;
using Unity.Mathematics;

namespace Nebukam.Geom
{

    public interface IVoronoiProvider : IVerticesProvider, IEdgesProvider
    {
        float centroidWeight { get; set; }
        IVerticesProvider verticesProvider { get; }
        NativeMultiHashMap<int, int> outputSites { get; } //Key is input vertice, Values are connected Sites index
    }

    /// <summary>
    /// Create voronoi edges based on a given set of input triangles & vertices.
    /// </summary>
    public class VoronoiPass : Processor<VoronoiJob>, IVoronoiProvider
    {

        protected float m_centroidWeight = 0f;
        protected IVerticesProvider m_verticesProvider = null;
        protected ITriadProvider m_triadProvider = null;

        protected NativeList<float3> m_outputVertices = new NativeList<float3>(0, Allocator.Persistent);
        protected NativeMultiHashMap<int, int> m_outputSites = new NativeMultiHashMap<int, int>(0, Allocator.Persistent);
        protected NativeList<UnsignedEdge> m_outputEdges = new NativeList<UnsignedEdge>(0, Allocator.Persistent);

        /// <summary>
        /// Weight of the centroid vs circumcenter when determining voronoi site position.
        /// 0 = use circumcenter, 1 = use centroid, anything in-between is used as interpolation value between
        /// the two (and as such isn't free). 
        /// </summary>
        public float centroidWeight { get { return m_centroidWeight; } set { m_centroidWeight = value; } }

        /// <summary>
        /// Voronoi sites
        /// </summary>
        public NativeList<float3> outputVertices { get { return m_outputVertices; } }
        
        /// <summary>
        /// Neighbors site for each input vertices
        /// </summary>
        public NativeMultiHashMap<int, int> outputSites { get { return m_outputSites; } }
        
        /// <summary>
        /// Voronoi edges
        /// </summary>
        public NativeList<UnsignedEdge> outputEdges { get { return m_outputEdges; } }
        
        /// <summary>
        /// Vertices provider used for the Voronoi pass
        /// </summary>
        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }

        /// <summary>
        /// Triangulation provider used for the Voronoi pass
        /// </summary>
        public ITriadProvider triadProvider { get { return m_triadProvider; } }

        protected override void InternalLock() { }
        protected override void InternalUnlock() { }

        protected override void Prepare(ref VoronoiJob job, float delta)
        {

            if(!TryGetFirstInGroup(out m_verticesProvider)
                || !TryGetFirstInGroup(out m_triadProvider))
            {
                throw new System.Exception("Missing providers");
            }
            
            //Clear previously built triangles
            m_outputVertices.Clear();
            m_outputSites.Clear();
            m_outputEdges.Clear();

            job.centroidWeight = m_centroidWeight;

            job.inputVertices = m_verticesProvider.outputVertices;

            job.inputTriangles = m_triadProvider.outputTriangles;
            job.inputHullVertices = m_triadProvider.outputHullVertices;
            job.inputUnorderedHull = m_triadProvider.outputUnorderedHull;

            job.outputVertices = m_outputVertices;
            job.outputSites = m_outputSites;
            job.outputEdges = m_outputEdges;

        }

        protected override void Apply(ref VoronoiJob job)
        {

        }

        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputVertices.Dispose();
            m_outputSites.Dispose();
            m_outputEdges.Dispose();

        }

    }
}
