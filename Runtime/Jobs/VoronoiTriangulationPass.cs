using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.JobAssist;

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
        protected NativeHashMap<int, UnsignedEdge> m_outputUnorderedHull = new NativeHashMap<int, UnsignedEdge>(0, Allocator.Persistent);

        public NativeList<float3> outputVertices { get { return m_outputVertices; } }
        public bool computeTriadCentroid { get { return m_computeTriadCentroid; } set { m_computeTriadCentroid = value; } }
        public NativeList<Triad> outputTriangles { get { return m_outputTriangles; } }
        public NativeList<int> outputHullVertices { get { return m_outputHullVertices; } }
        public NativeHashMap<int, UnsignedEdge> outputUnorderedHull { get { return m_outputUnorderedHull; } }

        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }
        public IVoronoiProvider voronoiProvider { get { return m_voronoiProvider; } }

        protected override void InternalLock() { }
        protected override void InternalUnlock() { }

        protected override void Prepare(ref VoronoiTriangulationJob job, float delta)
        {

            if (!TryGetFirstInGroup(out m_voronoiProvider))
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

        protected override void Apply(ref VoronoiTriangulationJob job)
        {

        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputVertices.Dispose();
            m_outputTriangles.Dispose();
            m_outputHullVertices.Dispose();
            m_outputUnorderedHull.Dispose();

        }

    }
}
