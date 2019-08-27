// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
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
    public struct Segment
    {

        public static Segment zero = new Segment();

        public float3 A, B;

        public Segment(float3 a, float3 b) { A = a; B = b; }
        public Segment(float3 b) { A = float3(0f); B = b; }

        /// <summary>
        /// Center point on the segment
        /// </summary>
        public float3 center
        {
            get
            {
                float t = 0.5f;
                return float3(A.x + (B.x - A.x) * t, A.y + (B.y - A.y) * t, A.z + (B.z - A.z) * t);
            }
        }

        /// <summary>
        /// Distance from A to B
        /// </summary>
        public float distance
        {
            get
            {
                float3 C = float3(B.x - A.x, B.y - A.y, B.z - A.z);
                return sqrt(C.x * C.x + C.y * C.y + C.z * C.z);
            }
        }

        /// <summary>
        /// Squared distance from A to B
        /// </summary>
        public float sqrDistance
        {
            get
            {
                float3 C = float3(B.x - A.x, B.y - A.y, B.z - A.z);
                return C.x * C.x + C.y * C.y + C.z * C.z;
            }
        }

        /// <summary>
        /// Direction of the segment
        /// </summary>
        public float3 dir { get { return float3(B.x - A.x, B.y - A.y, B.z - A.z); } }

        /// <summary>
        /// Compute the normalized direction (A -> B) of this segment
        /// </summary>
        public float3 normalizedDir
        {
            get
            {
                float3 C = float3(B.x - A.x, B.y - A.y, B.z - A.z);
                float d = sqrt(C.x * C.x + C.y * C.y + C.z * C.z);
                return float3(C.x / d, C.y / d, C.z / d);
            }
        }

        public float3 normal
        {
            get
            {
                return cross(float3(B.x - A.x, B.y - A.y, B.z - A.z), float3(A.x - A.x, A.y - A.y, (A.z - A.z) + 1f));
            }
        }


        /// <summary>
        /// Return a point at a given percentage from A to B
        /// </summary>
        public float3 At(float t)
        {
            return float3(A.x + (B.x - A.x) * t, A.y + (B.y - A.y) * t, A.z + (B.z - A.z) * t);
        }

        /// <summary>
        /// Find the closest point on the segment to a given point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public float3 ClosestPoint(float3 pt)
        {

            float3 C = float3(pt.x - A.x, pt.y - A.y, pt.z - A.z),//pt - A
                D = float3(B.x - A.x, B.y - A.y, B.z - A.z);//B - A

            float d = D.x * D.x + D.y * D.y + D.z * D.z; //Square distance

            D = normalize(D);
            float t = D.x * C.x + D.y * C.y + D.z * C.z; //Dot

            if (t <= 0)
                return A;

            if ((t * t) >= d) //Distance check
                return B;

            C.x = A.x + D.x * t; C.y = A.y + D.y * t; C.z = A.z + D.z * t;
            return C;

        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XY (2D) plane,
        /// Faster alternative, since the intersection point is not required.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public bool IsIntersectingXY(Segment segment)
        {
            float2 A2 = float2(segment.A.x, segment.A.y), B2 = float2(segment.B.x, segment.B.y);

            var d = (B.x - A.x) * (B2.y - A2.y) - (B.y - A.y) * (B2.x - A2.x);

            if (d == 0.0f)
                return false;

            float u = ((A2.x - A.x) * (B2.y - A2.y) - (A2.y - A.y) * (B2.x - A2.x)) / d;
            float v = ((A2.x - A.x) * (B.y - A.y) - (A2.y - A.y) * (B.x - A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
                return false;

            return true;
        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XY (2D) plane
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="intersectPoint"></param>
        /// <returns></returns>
        public bool IsIntersectingXY(Segment segment, out float3 intersection)
        {
            intersection = float3(0f);
            float2 A2 = float2(segment.A.x, segment.A.y), B2 = float2(segment.B.x, segment.B.y);

            var d = (B.x - A.x) * (B2.y - A2.y) - (B.y - A.y) * (B2.x - A2.x);

            if (d == 0.0f)
                return false;

            float u = ((A2.x - A.x) * (B2.y - A2.y) - (A2.y - A.y) * (B2.x - A2.x)) / d;
            float v = ((A2.x - A.x) * (B.y - A.y) - (A2.y - A.y) * (B.x - A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
                return false;

            intersection.x = A.x + u * (B.x - A.x);
            intersection.y = A.y + u * (B.y - A.y);
            return true;
        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XZ plane
        /// Faster alternative, since the intersection point is not required.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public bool IsIntersectingXZ(Segment segment)
        {
            float2 A2 = float2(segment.A.x, segment.A.z), B2 = float2(segment.B.x, segment.B.z);

            var d = (B.x - A.x) * (B2.y - A2.y) - (B.z - A.z) * (B2.x - A2.x);

            if (d == 0.0f)
                return false;

            float u = ((A2.x - A.x) * (B2.y - A2.y) - (A2.y - A.y) * (B2.x - A2.x)) / d;
            float v = ((A2.x - A.x) * (B.z - A.z) - (A2.y - A.z) * (B.x - A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
                return false;

            return true;
        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XZ plane
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="intersectPoint"></param>
        /// <returns></returns>
        public bool IsIntersectingXZ(Segment segment, out float3 intersection)
        {
            intersection = float3(0f);
            float2 A2 = float2(segment.A.x, segment.A.z), B2 = float2(segment.B.x, segment.B.z);

            var d = (B.x - A.x) * (B2.y - A2.y) - (B.z - A.z) * (B2.x - A2.x);

            if (d == 0.0f)
                return false;

            float u = ((A2.x - A.x) * (B2.y - A2.y) - (A2.y - A.y) * (B2.x - A2.x)) / d;
            float v = ((A2.x - A.x) * (B.z - A.z) - (A2.y - A.z) * (B.x - A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
                return false;

            intersection.x = A.x + u * (B.x - A.x);
            intersection.z = A.z + u * (B.z - A.z);
            return true;
        }

    }

}
