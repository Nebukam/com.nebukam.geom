using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Nebukam.Utils;
using Nebukam.JobAssist;

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
        protected NativeHashMap<int, UnsignedEdge> m_outputUnorderedHull = new NativeHashMap<int, UnsignedEdge>(0, Allocator.Persistent);

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
        public NativeHashMap<int, UnsignedEdge> outputUnorderedHull { get { return m_outputUnorderedHull; } }

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputTriangles.Dispose();
        }

    }
}
