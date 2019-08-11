using UnityEngine;
using Unity.Mathematics;

namespace Nebukam.Geom
{

    public struct Triangle 
    {

        public static Triangle zero = new Triangle(float3.zero, float3.zero, float3.zero);

        public float3 A, B, C;

        public Segment AB { get { return new Segment(A, B); } }
        public Segment AC { get { return new Segment(A, C); } }
        public Segment BC { get { return new Segment(B, C); } }

        public float3 normal {
            get
            {
                float lhsx = B.x - A.x, lhsy = B.y - A.y, lhsz = B.z - A.z;
                float rhsx = C.x - A.x, rhsy = C.y - A.y, rhsz = C.z - A.z;

                return new float3(
                    lhsy * rhsz - lhsz * rhsy, 
                    lhsz * rhsx - lhsx * rhsz, 
                    lhsx * rhsy - lhsy * rhsx);
            }
        }
        
        public Triangle(float3 _A, float3 _B, float3 _C)
        {
            A = _A;
            B = _B;
            C = _C;
        }

        void ChangeOrientation()
        {
            float3 tv = A;
            A = B;
            B = tv;
        }

        public bool ContainsXY(float3 pt)
        {
            float N = 1 / 2 * (-B.y * C.x + A.y * (-B.x + C.x) + A.x * (B.y - C.y) + B.x * C.y);
            float sign = N < 0 ? -1 : 1;
            float s = (A.y * C.x - A.x * C.y + (C.y - A.y) * pt.x + (A.x - C.x) * pt.y) * sign;
            float t = (A.x * B.y - A.y * B.x + (A.y - B.y) * pt.x + (B.x - A.x) * pt.y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * N * sign;
        }

        public bool ContainsXZ(float3 pt)
        {
            float N = 1 / 2 * (-B.z * C.x + A.z * (-B.x + C.x) + A.x * (B.z - C.z) + B.x * C.z);
            float sign = N < 0 ? -1 : 1;
            float s = (A.z * C.x - A.x * C.z + (C.z - A.z) * pt.x + (A.x - C.x) * pt.z) * sign;
            float t = (A.x * B.z - A.z * B.x + (A.z - B.z) * pt.x + (B.x - A.x) * pt.z) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * N * sign;
        }

        //TODO : 3D https://stackoverflow.com/questions/1988100/how-to-determine-ordering-of-3d-vertices
        public bool IsCounterClockwiseXY()
        {
            return ((B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y)) > 0;
        }

        public bool IsCounterClockwiseXZ()
        {
            return ((B.x - A.x) * (C.z - A.z) - (C.x - A.x) * (B.z - A.z)) > 0;
        }

    }

}
