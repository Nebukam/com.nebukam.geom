using Nebukam.Utils;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{
    /*
        idea on how to make it JobFriendly :
        have a generic class holding "flat geometry" structure as follow :
        - NativeList A of VertexData (holding their own index + a float3)
        - NativeList B of TriangleData (holding their own index as well as VertexData indexes in the above list)
        - NativeList C of NativeList D, in order to store "shared edges" of each triangle. (indices must be unique entries)

        That means :
        - each time a triangle is added to B, an entry must be registered to C with the same index.
        - each Triangle created must have its index updated at all times

        Note : this doesn't solve the problem of edges...
        https://github.com/RafaelKuebler/DelaunayVoronoi/blob/master/DelaunayVoronoi/Delaunay.cs

     */

    public struct Triad
    {

        public int A, B, C;

        public float3 circumcenter, centroid;
        public float sqRadius;
        
        public Triad(int a, int b, int c, float3 cc, float rad)
        {
            A = a;
            B = b;
            C = c;

            centroid = float3(false);
            circumcenter = cc;
            sqRadius = rad;
        }

        public bool SharesEdgeWith(Triad other)
        {
            int oA = other.A, oB = other.B, oC = other.C;
            int i = 0;
            if ( A == oA || A == oB || A == oC ) { i++; };
            if ( B == oA || B == oB || B == oC ) { i++; };
            if ( C == oA || C == oB || C == oC ) { i++; };
            return ( i == 2 );
        }
        
        public bool CircumcircleContainsXY(float3 v)
        {
            return ((v.x - circumcenter.x) * (v.x - circumcenter.x) + (v.y - circumcenter.y) * (v.y - circumcenter.y)) < sqRadius;
        }

        public bool CircumcircleContainsXZ(float3 v)
        {
            return ((v.x - circumcenter.x) * (v.x - circumcenter.x) + (v.z - circumcenter.z) * (v.z - circumcenter.z)) < sqRadius;
        }

        /*
        #region CW / CCW

        private bool IsCounterClockwiseXY(Vertex a, Vertex b, Vertex c)
        {
            float3 vA = a.pos, vB = b.pos, vC = c.pos;
            return ((vB.x - vA.x) * (vC.y - vA.y) - (vC.x - vA.x) * (vB.y - vA.y)) > 0;
        }

        private bool IsCounterClockwiseXZ(Vertex a, Vertex b, Vertex c)
        {
            float3 vA = a.pos, vB = b.pos, vC = c.pos;
            return ((vB.x - vA.x) * (vC.z - vA.z) - (vC.x - vA.x) * (vB.z - vA.z)) > 0;
        }

        #endregion
        */
    }

}
