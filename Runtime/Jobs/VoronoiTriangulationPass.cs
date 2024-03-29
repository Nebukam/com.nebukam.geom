﻿// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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
using Nebukam.Common;

namespace Nebukam.Geom
{
    public class VoronoiTriangulationPass : Processor<VoronoiTriangulationJob>, IVerticesProvider, ITriadProvider
    {

        protected AxisPair plane = AxisPair.XY;

        protected IVerticesProvider m_verticesProvider = null;
        protected IVoronoiProvider m_voronoiProvider = null;

        protected bool m_computeTriadCentroid = false;
        protected NativeList<float3> m_outputVertices = new NativeList<float3>(0, Allocator.Persistent);
        protected NativeList<Triad> m_outputTriangles = new NativeList<Triad>(0, Allocator.Persistent);
        protected NativeList<int> m_outputHullVertices = new NativeList<int>(0, Allocator.Persistent);
        protected NativeParallelHashMap<int, UIntPair> m_outputUnorderedHull = new NativeParallelHashMap<int, UIntPair>(0, Allocator.Persistent);

        public NativeList<float3> outputVertices { get { return m_outputVertices; } }
        public bool computeTriadCentroid { get { return m_computeTriadCentroid; } set { m_computeTriadCentroid = value; } }
        public NativeList<Triad> outputTriangles { get { return m_outputTriangles; } }
        public NativeList<int> outputHullVertices { get { return m_outputHullVertices; } }
        public NativeParallelHashMap<int, UIntPair> outputUnorderedHull { get { return m_outputUnorderedHull; } }

        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }
        public IVoronoiProvider voronoiProvider { get { return m_voronoiProvider; } }

        protected override void Prepare(ref VoronoiTriangulationJob job, float delta)
        {

            if (!TryGetFirstInCompound(out m_voronoiProvider))
            {
                throw new System.Exception("No IVerticesProvider in chain !");
            }

            m_verticesProvider = m_voronoiProvider.verticesProvider;

            m_outputVertices.Clear();
            m_outputTriangles.Clear();
            m_outputHullVertices.Clear();
            m_outputUnorderedHull.Clear();

            job.plane = plane;
            job.inputVertices = m_verticesProvider.outputVertices;
            job.inputSitesVertices = m_voronoiProvider.outputVertices;
            job.inputSites = m_voronoiProvider.outputSites;
            job.inputUnorderedHullEdges = m_voronoiProvider.triadProvider.outputUnorderedHull;

            job.computeTriadCentroid = m_computeTriadCentroid;
            job.outputVertices = m_outputVertices;
            job.outputTriangles = m_outputTriangles;
            job.outputHullVertices = m_outputHullVertices;
            job.outputUnorderedHullEdges = m_outputUnorderedHull;

        }

        protected override void InternalDispose()
        {
            m_outputVertices.Release();
            m_outputTriangles.Release();
            m_outputHullVertices.Release();
            m_outputUnorderedHull.Release();
        }
    }
}
