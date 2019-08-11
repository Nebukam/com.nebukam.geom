using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Utils;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{

    public interface IUrquhartProvider : IEdgesProvider
    {
        NativeMultiHashMap<int, int> outputConnections { get; }
    }

    /// <summary>
    /// Create voronoi edges based on a given set of input triangles & vertices.
    /// </summary>
    public class UrquhartPass : Processor<UrquhartJob>, IUrquhartProvider
    {

        protected IVerticesProvider m_verticesProvider = null;
        protected ITriadProvider m_triadProvider = null;

        protected NativeList<UnsignedEdge> m_outputEdges = new NativeList<UnsignedEdge>(0, Allocator.Persistent);
        protected NativeMultiHashMap<int, int> m_outputConnections = new NativeMultiHashMap<int, int>(0, Allocator.Persistent);

        /// <summary>
        /// Voronoi edges
        /// </summary>
        public NativeList<UnsignedEdge> outputEdges { get { return m_outputEdges; } }

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

        protected override void InternalLock() { }
        protected override void InternalUnlock() { }

        protected override void Prepare(ref UrquhartJob job, float delta)
        {

            if (!TryGetFirstInGroup(out m_verticesProvider)
                || !TryGetFirstInGroup(out m_triadProvider))
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

        protected override void Apply(ref UrquhartJob job)
        {

        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputEdges.Dispose();
            m_outputConnections.Dispose();

        }

    }
}
