using UnityEngine;

namespace Nebukam.Geom
{
    /// <summary>
    /// Job-friendly snapshot of a Vertex instance.
    /// </summary>
    public struct VertexData
    {
        public int index;
        public Vector3 v;

        public VertexData(int i, Vector3 vector)
        {
            index = i;
            v = vector;
        }

        public VertexData(int i, Vertex vertex)
            : this(i, vertex.v)
        {

        }

    }
}
