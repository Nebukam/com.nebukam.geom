using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Utils;

namespace Nebukam.Geom
{

    public interface IWorldVertexGroup : IVertexGroup
    {
        void Update();
        float3 pivot { get; set; }
        float3 rotation { get; set; }
    }
    
    public class WorldVertexGroup<T> : VertexGroup<T>, IWorldVertexGroup
        where T : WorldVertex, IVertex, new()
    {

        protected float3 m_pivot = float3(false);
        protected float3 m_rotation = float3(false);

        public float3 pivot { get { return m_pivot; } set { m_pivot = value; } }
        public float3 rotation { get { return m_rotation; } set { m_rotation = value; } }

        protected override void OnVertexAdded(T v)
        {
            base.OnVertexAdded(v);
            v.local = v.pos;
            Transform(v);
        }

        public virtual void Update()
        {
            for (int i = 0, count = m_vertices.Count; i < count; i++)
            {
                Transform(m_vertices[i] as T);
            }
        }

        protected virtual void Transform(T vertex)
        {
            vertex.world = Maths.RotateAroundPivot(vertex.local + m_pivot, m_pivot, m_rotation);
        }

    }
}
