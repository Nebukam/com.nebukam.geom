using UnityEngine;

namespace Nebukam.Geom
{
    /// <summary>
    /// Job-friendly snapshot of a HalfEdge instance.
    /// </summary>
    public struct HalfEdgeData
    {
        public int index;

        public int vertexIndex;
        public int triangleIndex;
        
        public int nextEdgeIndex;
        public int prevEdgeIndex;
        public int pppositeEdgeIndex;

        public HalfEdgeData(int i)
        {
            index = i;
            vertexIndex = -1;

            triangleIndex = -1;

            nextEdgeIndex = -1;
            prevEdgeIndex = -1;
            pppositeEdgeIndex = -1;
    }

    }
}
