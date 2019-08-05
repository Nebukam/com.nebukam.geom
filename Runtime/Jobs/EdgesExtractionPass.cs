using Unity.Collections;
using Nebukam.JobAssist;
using UnityEngine;
namespace Nebukam.Geom
{

    /// <summary>
    /// An edge extraction pass, to be use in a ProcessingChain.
    /// Requires an ITriadProvider.
    /// </summary>
    public class EdgesExtractionPass : Processor<EdgesExtractionJob>, IEdgesProvider
    {

        protected ITriadProvider m_triadProvider = null;
        protected NativeList<Triad> m_inputTriangles;
        protected NativeList<UnsignedEdge> m_outputEdges = new NativeList<UnsignedEdge>(0, Allocator.Persistent);

        /// <summary>
        /// The ITriadProvider used during preparation.
        /// </summary>
        public ITriadProvider triadProvider { get { return m_triadProvider; } }
        
        public NativeList<UnsignedEdge> outputEdges { get { return m_outputEdges; } }

        protected override void Prepare(ref EdgesExtractionJob job, float delta)
        {            

            if (!TryGetFirstInGroup(out m_triadProvider))
            {
                throw new System.Exception("No ITriadProvider in chain !");
            }

            m_outputEdges.Clear();

            job.inputTriangles = m_triadProvider.outputTriangles;
            job.outputEdges = m_outputEdges;
        }

        protected override void Apply(ref EdgesExtractionJob job)
        {

        }

        protected override void InternalLock()
        {
            
        }

        protected override void InternalUnlock()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_outputEdges.Dispose();
        }

    }
}
