using UnityEngine;

namespace Nebukam.Geom
{
    
    public class Vertex
    {
        
        protected Vector3 _v = Vector3.zero;
        public virtual Vector3 v { get { return _v; } set { _v = value; } }

        public virtual float x { get { return _v.x; } set { _v.x = value; } }
        public virtual float y { get { return _v.y; } set { _v.y = value; } }
        public virtual float z { get { return _v.z; } set { _v.z = value; } }
        
        /// <summary>
        /// The outgoing halfedge (a halfedge that starts at this vertex)
        /// </summary>
        public HalfEdge halfEdge = null;

        /// <summary>
        /// Triangle this vertex is a part of.
        /// </summary>
        public ManagedTriangle triangle = null;
        
        /// <summary>
        /// The previous vertex this vertex is attached to
        /// </summary>
        public Vertex prev = null;
        
        /// <summary>
        /// The next vertex this vertex is attached to
        /// </summary>
        public Vertex next = null;
        
        public bool isReflex = false;
        public bool isConvex = false;        
        public bool isEar = false;

        public VertexGroup group = null;

        public Vertex() { }

        public Vertex(Vector3 v3)
        {
            _v = v3;
        }


        public static implicit operator Vector3(Vertex p) { return p.v; }
        public static implicit operator Vertex(Vector3 p)
        {
            return new Vertex(p);
        }

    }


}
