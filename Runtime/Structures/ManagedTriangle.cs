using UnityEngine;

namespace Nebukam.Geom
{
    
    public class ManagedTriangle
    {

        public Vertex A = null, B = null, C = null;

        protected HalfEdge halfEdge;

        public ManagedTriangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }

        public ManagedTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = new Vertex(a);
            B = new Vertex(b);
            C = new Vertex(c);
        }

        public ManagedTriangle(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        /// <summary>
        /// Change orientation of triangle from cw -> ccw or ccw -> cw
        /// </summary>
        public void ChangeOrientation()
        {
            Vertex temp = A;
            A = B;
            B = temp;
        }

        public static implicit operator Triangle(ManagedTriangle t)
        {
            return new Triangle(t.A.v, t.B.v, t.C.v);
        }


    }
}
