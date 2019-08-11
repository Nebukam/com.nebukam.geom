using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Nebukam.Utils;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{

    /// <summary>
    /// A Delaunay triangulation pass, to be use in a ProcessingChain.
    /// Requires an IVerticesProvider.
    /// </summary>
    public class DelaunayPass : Processor<DelaunayJob>, ITriadProvider
    {
        protected IVerticesProvider m_verticesProvider;
        protected bool m_computeTriadCentroid = false;
        protected NativeList<Triad> m_outputTriangles = new NativeList<Triad>(0, Allocator.Persistent);
        protected NativeList<int> m_outputHullVertices = new NativeList<int>(0, Allocator.Persistent);
        protected NativeHashMap<int, UnsignedEdge> m_outputUnorderedHull = new NativeHashMap<int, UnsignedEdge>(0, Allocator.Persistent);

        /// <summary>
        /// The IVerticesProvider used for the triangulation.
        /// </summary>
        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }
        /// <summary>
        /// Whether or not this pass should compute Triad centroids.
        /// </summary>
        public bool computeTriadCentroid { get { return m_computeTriadCentroid; } set { m_computeTriadCentroid = value; } }

        /// <summary>
        /// The Delaunay triangulation of the input set of points
        /// </summary>
        public NativeList<Triad> outputTriangles { get { return m_outputTriangles; } }
        /// <summary>
        /// Unordered vertices lying on the hull of the triangulation.
        /// Due to the DelaunayJob using Bowyer-Watson, the hull is not guaranteed to be convex.
        /// </summary>
        public NativeList<int> outputHullVertices { get { return m_outputHullVertices; } }
        /// <summary>
        /// Unordered hull edges
        /// </summary>
        public NativeHashMap<int, UnsignedEdge> outputUnorderedHull { get { return m_outputUnorderedHull; } }

        protected override void InternalLock()
        {

        }

        protected override void Prepare(ref DelaunayJob job, float delta)
        {

            if(!TryGetFirstInGroup(out m_verticesProvider))
            {
                throw new System.Exception("No IVerticesProvider in chain !");
            }

            m_outputTriangles.Clear();
            m_outputHullVertices.Clear();
            m_outputUnorderedHull.Clear();

            job.inputVertices = m_verticesProvider.outputVertices;
            job.computeTriadCentroid = m_computeTriadCentroid;
            job.outputTriangles = m_outputTriangles;
            job.outputHullVertices = m_outputHullVertices;
            job.outputUnorderedHullEdges = m_outputUnorderedHull;

        }

        protected override void Apply(ref DelaunayJob job)
        {

        }
        
        protected override void InternalUnlock()
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputTriangles.Dispose();
            m_outputHullVertices.Dispose();
            m_outputUnorderedHull.Dispose();
        }

    }
}
