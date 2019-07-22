namespace Nebukam.Geom
{
    public class HalfEdge
    {
        /// <summary>
        /// The vertex the edge points to
        /// </summary>
        public Vertex vertex = null;

        /// <summary>
        /// The face this edge is a part of
        /// </summary>
        public ManagedTriangle triangle = null;


        /// <summary>
        /// Next half edge
        /// </summary>
        public HalfEdge nextEdge = null;

        /// <summary>
        /// Previous half edge
        /// </summary>
        public HalfEdge prevEdge = null;

        /// <summary>
        /// The opposite HalfEdge
        /// </summary>
        public HalfEdge oppositeEdge = null;


        //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
        //and a face (triangle) class with a reference to a half edge which is a part of this face 
        public HalfEdge(Vertex v)
        {
            vertex = v;
        }
    }
}
