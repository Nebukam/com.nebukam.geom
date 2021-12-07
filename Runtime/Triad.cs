// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using Unity.Burst;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{

    [BurstCompile]
    public struct Triad : System.IEquatable<Triad>
    {

        public int A, B, C;
        public float sqRadius;
        public float3 circumcenter, centroid;

        public Triad(int a, int b, int c, float3 cc, float rad)
        {
            A = a;
            B = b;
            C = c;

            centroid = float3(0f);
            circumcenter = cc;
            sqRadius = rad;
        }

        public UIntPair OppositeEdge(int v)
        {
            return v == A ? new UIntPair(B, C)
                : v == B ? new UIntPair(A, C)
                : new UIntPair(A, B);
        }

        public UIntPair AB { get { return new UIntPair(A, B); } }
        public UIntPair BC { get { return new UIntPair(B, C); } }
        public UIntPair CA { get { return new UIntPair(C, A); } }

        public bool SharesEdgeWith(Triad tri)
        {
            int oA = tri.A, oB = tri.B, oC = tri.C;
            int i = 0;
            if (A == oA || A == oB || A == oC) { i++; };
            if (B == oA || B == oB || B == oC) { i++; };
            if (C == oA || C == oB || C == oC) { i++; };
            return (i == 2);
        }

        public bool SharesVertexWith(Triad tri)
        {
            return (A == tri.A || A == tri.B || A == tri.C
                || B == tri.A || B == tri.B || B == tri.C
                || C == tri.A || C == tri.B || C == tri.C);
        }

        public bool CircumcircleContainsXY(float3 v)
        {
            return ((v.x - circumcenter.x) * (v.x - circumcenter.x) + (v.y - circumcenter.y) * (v.y - circumcenter.y)) < sqRadius;
        }

        public bool CircumcircleContainsXZ(float3 v)
        {
            return ((v.x - circumcenter.x) * (v.x - circumcenter.x) + (v.z - circumcenter.z) * (v.z - circumcenter.z)) < sqRadius;
        }

        public bool Equals(Triad tri)
        {
            return this == tri;
        }

        public override bool Equals(object obj)
        {
            return this == (Triad)obj;
        }

        public static bool operator ==(Triad a, Triad b)
        {
            return ((a.A == b.A && a.B == b.B && a.C == b.C)
                || (a.A == b.A && a.B == b.C && a.C == b.B)
                || (a.A == b.B && a.B == b.A && a.C == b.C)
                || (a.A == b.B && a.B == b.C && a.C == b.A)
                || (a.A == b.C && a.B == b.A && a.C == b.B)
                || (a.A == b.C && a.B == b.B && a.C == b.A));
        }

        public static bool operator !=(Triad a, Triad b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.A.GetHashCode() ^ this.B.GetHashCode() ^ this.C.GetHashCode();
        }

    }

}
