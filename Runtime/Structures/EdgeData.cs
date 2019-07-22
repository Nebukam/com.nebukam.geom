using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nebukam.Geom
{

    //And edge between two vertices
    public class EdgeData
    {

        public int index;
        public int vertexAIndex, vertexBIndex;

        /// <summary>
        /// Is this edge intersecting with another edge?
        /// </summary>
        public bool isIntersecting;

        public EdgeData(int iA, int iB)
        {
            vertexAIndex = iA;
            vertexBIndex = iB;
        }

        //Flip edge
        public void FlipEdge()
        {
            int temp = vertexAIndex;
            vertexAIndex = vertexBIndex;
            vertexBIndex = temp;
        }
    }

}