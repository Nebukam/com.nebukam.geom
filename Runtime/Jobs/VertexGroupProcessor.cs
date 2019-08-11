using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Common;
using Nebukam.Utils;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{

    public interface IVertexGroupProcessor : IProcessor, IVerticesProvider
    {
        IVertexGroup vertices { get;  set; }
    }

    public abstract class VertexGroupProcessor<T> : Processor<T>, IVertexGroupProcessor
        where T : struct, IJob
    {

        protected IVertexGroup m_vertices = null;
        public IVertexGroup vertices { get { return m_vertices; } set { m_vertices = value; } }

        protected List<IVertex> m_lockedVertices = new List<IVertex>();

        protected NativeList<float3> m_outputVertices = new NativeList<float3>(0, Allocator.Persistent);
        public NativeList<float3> outputVertices { get { return m_outputVertices; } }

        protected override void InternalLock()
        {
            int count = m_vertices.Count;

            m_lockedVertices.Clear();
            m_lockedVertices.Capacity = count;

            for (int i = 0; i < count; i++) { m_lockedVertices.Add(m_vertices[i]); }

        }

        protected override void Prepare(ref T job, float delta)
        {
            int vCount = m_lockedVertices.Count;
            m_outputVertices.Clear();
            m_outputVertices.Capacity = vCount;

            for (int i = 0; i < vCount; i++)
            {
                m_outputVertices.Add(m_lockedVertices[i].pos);
            }
        }
        
        protected override void InternalUnlock()
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) { return; }

            m_vertices = null;
            m_outputVertices.Dispose();
            
        }

    }
}
