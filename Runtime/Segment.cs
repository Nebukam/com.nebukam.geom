using UnityEngine;
using Nebukam.Utils;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{
    public struct Segment
    {

        public static Segment zero = new Segment();

        public float3 A, B;

        public Segment(float3 a, float3 b) { A = a; B = b; }
        public Segment(float3 b) { A = float3(false); B = b; }

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
                return Mathf.Sqrt(C.x * C.x + C.y * C.y + C.z * C.z);
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
                float d = Mathf.Sqrt(C.x * C.x + C.y * C.y + C.z * C.z);
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
            float2 SA = float2(segment.A.x, segment.A.y), SB = float2(segment.B.x, segment.B.y);
            float denominator = (SB.y - SA.y) * (B.x - A.x) - (SB.x - SA.x) * (B.y - A.y);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((SB.x - SA.x) * (A.y - SA.y) - (SB.y - SA.y) * (A.x - SA.x)) / denominator;
                float u_b = ((B.x - A.x) * (A.y - SA.y) - (B.y - A.y) * (A.x - SA.x)) / denominator;

                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XY (2D) plane
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="intersectPoint"></param>
        /// <returns></returns>
        public bool IsIntersectingXY(Segment segment, out float3 intersectPoint)
        {

            intersectPoint = float3(0f,0f,0f);
            bool isIntersecting = false;

            //3d -> 2d
            float2
            SA = float2(segment.A.x, segment.A.y),
            SB = float2(segment.B.x, segment.B.y),

            AADir = normalize(float2(B.x - A.x, B.y - A.y)),
            BBDir = normalize(float2(SB.x - SA.x, SB.y - SA.y)),

            AAN = float2(-AADir.y, AADir.x),
            BBN = float2(-BBDir.y, BBDir.x);

            //Step 1: Rewrite the lines to a general form: Ax + By = k1 and Cx + Dy = k2
            //The normal vector is the A, B
            float _A = AAN.x, _B = AAN.y, _C = BBN.x, _D = BBN.y,

            //To get k we just use one point on the line
            k1 = (_A * A.x) + (_B * A.y),
            k2 = (_C * SA.x) + (_D * SA.y);

            //re the lines parallel? -> no solutions
            float2 an = normalize(AAN), bn = normalize(BBN);
            float temp = Mathf.Acos(Mathf.Clamp((an.x * bn.x + an.y * bn.y), -1f, 1f)) * 57.29578f;
            if (temp == 0f || temp == 180f) { return false; }

            // Are the lines the same line? -> infinite amount of solutions
            //Pick one point on each line and test if the vector between the points is orthogonal to one of the normals
            temp = (A.x - SA.x) * AAN.x + (A.y - SA.y) * AAN.y;
            if (temp < Maths.EPSILON && temp > Maths.NEPSILON) { return false; }
            
            intersectPoint = float3(
                 (_D * k1 - _B * k2) / (_A * _D - _B * _C),
                 (-_C * k1 + _A * k2) / (_A * _D - _B * _C), 
                 0f);

            // But we have line segments so we have to check if the intersection point is within the segment
            if (Maths.IsBetween(intersectPoint, A, B) && Maths.IsBetween(intersectPoint, float3(SA.x, SA.y,0f), float3(SB.x, SB.y,0f)))
            {
                isIntersecting = true;
            }

            return isIntersecting;
        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XZ plane
        /// Faster alternative, since the intersection point is not required.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public bool IsIntersectingXZ(Segment segment)
        {
            float2
            LA = float2(A.x, A.z),
            LB = float2(B.x, B.z),

            SA = float2(segment.A.x, segment.A.z),
            SB = float2(segment.B.x, segment.B.z);

            float denominator = (SB.y - SA.y) * (LB.x - LA.x) - (SB.x - SA.x) * (LB.y - LA.y);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((SB.x - SA.x) * (LA.y - SA.y) - (SB.y - SA.y) * (LA.x - SA.x)) / denominator;
                float u_b = ((LB.x - LA.x) * (LA.y - SA.y) - (LB.y - LA.y) * (LA.x - SA.x)) / denominator;

                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if this segment is intersecting with another given segment, on the XZ plane
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="intersectPoint"></param>
        /// <returns></returns>
        public bool IsIntersectingXZ(Segment segment, out float3 intersectPoint)
        {

            intersectPoint = float3(false);
            bool isIntersecting = false;

            //3d -> 2d
            float2
            LA = float2(A.x, A.z),
            LB = float2(B.x, B.z),

            SA = float2(segment.A.x, segment.A.z),
            SB = float2(segment.B.x, segment.B.z),

            AADir = normalize(float2(LB.x - LA.x, LB.y - LA.y)),
            BBDir = normalize(float2(SB.x - SA.x, SB.y - SA.y)),

            AAN = float2(-AADir.y, AADir.x),
            BBN = float2(-BBDir.y, BBDir.x);

            //Step 1: Rewrite the lines to a general form: Ax + By = k1 and Cx + Dy = k2
            //The normal vector is the A, B
            float _A = AAN.x, _B = AAN.y, _C = BBN.x, _D = BBN.y,

            //To get k we just use one point on the line
            k1 = (_A * LA.x) + (_B * LA.y),
            k2 = (_C * SA.x) + (_D * SA.y);

            //re the lines parallel? -> no solutions
            float2 an = normalize(AAN), bn = normalize(BBN);
            float temp = Mathf.Acos(Mathf.Clamp((an.x * bn.x + an.y * bn.y), -1f, 1f)) * 57.29578f;
            if (temp == 0f || temp == 180f) { return false; }

            // Are the lines the same line? -> infinite amount of solutions
            //Pick one point on each line and test if the vector between the points is orthogonal to one of the normals
            temp = (LA.x - SA.x) * AAN.x + (LA.y - SA.y) * AAN.y;
            if (temp < Maths.EPSILON && temp > Maths.NEPSILON) { return false; }
            
            intersectPoint = float3(
                 (_D * k1 - _B * k2) / (_A * _D - _B * _C),
                 0f,
                 (-_C * k1 + _A * k2) / (_A * _D - _B * _C));

            // But we have line segments so we have to check if the intersection point is within the segment
            if (Maths.IsBetween(intersectPoint, float3(LA.x, LA.y, 0f), float3(LB.x, LB.y, 0f)) && Maths.IsBetween(intersectPoint, float3(SA.x, SA.y, 0f), float3(SB.x, SB.y, 0f)))
            {
                isIntersecting = true;
            }

            return isIntersecting;
        }


    }

}
