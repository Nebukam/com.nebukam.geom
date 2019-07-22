using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nebukam.Geom
{

    //And edge between two vertices
    public class Edge
    {
        public Vertex A = null;
        public Vertex B = null;

        /// <summary>
        /// Is this edge intersecting with another edge?
        /// </summary>
        public bool isIntersecting = false;

        public Edge(Vertex a, Vertex b)
        {
            A = a;
            B = b;
        }

        public Edge(Vector3 a, Vector3 b)
        {
            A = new Vertex(a);
            B = new Vertex(b);
        }

        //Get vertex in 2d space (assuming x, z)
        public Vector2 GetVertex2D(Vertex v)
        {
            return new Vector2(v.x, v.z);
        }

        //Flip edge
        public void FlipEdge()
        {
            Vertex temp = A;
            A = B;
            B = temp;
        }
    }

}