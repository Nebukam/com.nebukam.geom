using UnityEngine;

namespace Nebukam.Geom
{

    public struct Triangle
    {

        public static Triangle zero = new Triangle(Vector3.zero, Vector3.zero, Vector3.zero);

        public Vector3 A, B, C;
        public int iA, iB, iC;

        public Segment AB { get { return new Segment(A, B); } }
        public Segment AC { get { return new Segment(A, C); } }
        public Segment BC { get { return new Segment(B, C); } }

        public Triangle(int _A, int _B, int _C)
        {
            A = B = C = Vector3.zero;
            iA = _A;
            iB = _B;
            iC = _C;
        }

        public Triangle(Vector3 _A, Vector3 _B, Vector3 _C)
        {
            iA = iB = iC = -1;
            A = _A;
            B = _B;
            C = _C;
        }

        void ChangeOrientation()
        {
            int t = iA;
            Vector3 tv = A;

            iA = iB;
            iB = t;

            A = B;
            B = tv;
        }

        public bool InTriangleXY(Vector3 pt)
        {
            float N = 1 / 2 * (-B.y * C.x + A.y * (-B.x + C.x) + A.x * (B.y - C.y) + B.x * C.y);
            float sign = N < 0 ? -1 : 1;
            float s = (A.y * C.x - A.x * C.y + (C.y - A.y) * pt.x + (A.x - C.x) * pt.y) * sign;
            float t = (A.x * B.y - A.y * B.x + (A.y - B.y) * pt.x + (B.x - A.x) * pt.y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * N * sign;
        }

        public bool InTriangleXZ(Vector3 pt)
        {
            float N = 1 / 2 * (-B.z * C.x + A.z * (-B.x + C.x) + A.x * (B.z - C.z) + B.x * C.z);
            float sign = N < 0 ? -1 : 1;
            float s = (A.z * C.x - A.x * C.z + (C.z - A.z) * pt.x + (A.x - C.x) * pt.z) * sign;
            float t = (A.x * B.z - A.z * B.x + (A.z - B.z) * pt.x + (B.x - A.x) * pt.z) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * N * sign;
        }

    }

}
