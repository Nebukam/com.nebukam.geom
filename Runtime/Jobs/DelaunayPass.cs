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
               
        /// <summary>
        /// The IVerticesProvider used during preparation.
        /// </summary>
        public IVerticesProvider verticesProvider { get { return m_verticesProvider; } }
        /// <summary>
        /// Whether or not this pass should compute Triad centroids.
        /// </summary>
        public bool computeTriadCentroid { get { return m_computeTriadCentroid; } set { m_computeTriadCentroid = value; } }

        public NativeList<Triad> outputTriangles { get { return m_outputTriangles; } }

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

            job.inputVertices = m_verticesProvider.outputVertices;
            job.computeTriadCentroid = m_computeTriadCentroid;
            job.outputTriangles = m_outputTriangles;

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
        }

    }
}
