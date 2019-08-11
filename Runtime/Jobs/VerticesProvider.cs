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

    public interface IVerticesProvider : IProcessor
    {
        NativeList<float3> outputVertices { get; }
    }

    public class VerticesProvider : Processor<Unemployed>, IVerticesProvider
    {

        protected IVertexGroup m_vertices = null;
        public IVertexGroup vertices { get { return m_vertices; } set { m_vertices = value; } }

        protected List<IVertex> lockedVertices = new List<IVertex>();

        protected NativeList<float3> m_outputVertices = new NativeList<float3>(0, Allocator.Persistent);
        public NativeList<float3> outputVertices { get { return m_outputVertices; } }

        protected override void InternalLock()
        {
            int count = m_vertices.Count;
            lockedVertices.Clear();
            lockedVertices.Capacity = count;
            for (int i = 0; i < count; i++) { lockedVertices.Add(m_vertices[i]); }
        }

        protected override void Prepare(ref Unemployed job, float delta)
        {

            int vCount = lockedVertices.Count;
            m_outputVertices.Clear();
            m_outputVertices.Capacity = vCount;

            for (int i = 0; i < vCount; i++)
            {
                m_outputVertices.Add(lockedVertices[i].pos);
            }
        }

        protected override void Apply(ref Unemployed job)
        {
            
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
