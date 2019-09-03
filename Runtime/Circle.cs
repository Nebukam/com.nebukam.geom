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
    public struct Circle
    {

        public float radius;
        public float3 center;
        public float3 normal;
        public float3 dir;

        public Circle(float r = 1f)
            : this(float3(0f, 0f, 0f), r, float3(0f, 1f, 0f), float3(0f, 0f, 1f))
        {

        }

        public Circle(float3 c, float r)
            : this(c, r, float3(0f, 1f, 0f), float3(0f, 0f, 1f))
        {

        }

        public Circle(float3 c, float r, float3 n, float3 d)
        {
            center = c;
            radius = r;
            normal = n;
            dir = d;
        }

        #region AtRadians

        public float3 AtRadians(float radAngle, AxisPair axis)
        {
            //float3 n = (Quaternion.AngleAxis(radAngle / 0.0174532924f, normal) * normalize(cross(normal, dir))) * radius;
            float3 n = mul(Unity.Mathematics.quaternion.AxisAngle(normal, radAngle), normalize(cross(normal, dir))) * radius;
            return float3(center.x + n.x, center.y + n.y, center.z + n.z);
        }

        #endregion

        #region AtAngle

        public float3 AtAngle(float angle, AxisPair axis)
        {
            //float3 n = (Quaternion.AngleAxis(angle, normal) * normalize(cross(normal, dir))) * radius;
            float3 n = mul(Unity.Mathematics.quaternion.AxisAngle(normal, angle / 0.0174532924f), normalize(cross(normal, dir))) * radius;
            return float3(center.x + n.x, center.y + n.y, center.z + n.z);
        }


        #endregion

        #region Intersects

        public bool IntersectsXY(Circle circle)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.y - center.y),
                d = sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= abs(otherRadius - radius))
                return true;
            else
                return false;

        }

        public bool IntersectsXZ(Circle circle)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.z - center.z),
                d = sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= abs(otherRadius - radius))
                return true;
            else
                return false;

        }

        public bool Intersects(Circle circle, AxisPair axis)
        {
            return axis == AxisPair.XY ? IntersectsXY(circle) : IntersectsXZ(circle);
        }

        #endregion

        #region Circle-circle intersection

        /// <summary>
        /// Find the intersection line between this circle and another, if it exists
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="line"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool TryGetIntersectionXY(Circle circle, out Segment line, float height = 0f)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.y - center.y),
                d = sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= abs(otherRadius - radius))
            {

                float rr = radius * radius,
                    ex = (otherCenter.x - center.x) / d,
                    ey = (otherCenter.y - center.y) / d,
                    x = (rr - otherRadius * otherRadius + d * d) / (2 * d),
                    y = sqrt(rr - x * x),
                    xex = center.x + x * ex, xey = center.y + x * ey,
                    yex = y * ex, yey = y * ey;

                line = new Segment(
                    new float3(xex - yey, xey + yex, height),
                    new float3(xex + yey, xey - yex, height)
                    );

                return true;

            }
            else
            {
                // No Intersection, far outside or one circle within the other
                line = Segment.zero;
                return false;
            }
        }

        /// <summary>
        /// Find the intersection line between this circle and another, if it exists
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="line"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool TryGetIntersectionXZ(Circle circle, out Segment line, float height = 0f)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.z - center.z),
                d = sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= abs(otherRadius - radius))
            {

                float rr = radius * radius,
                    ex = (otherCenter.x - center.x) / d,
                    ey = (otherCenter.z - center.z) / d,
                    x = (rr - otherRadius * otherRadius + d * d) / (2 * d),
                    y = sqrt(rr - x * x),
                    xex = center.x + x * ex, xey = center.z + x * ey,
                    yex = y * ex, yey = y * ey;

                line = new Segment(
                    new float3(xex - yey, height, xey + yex),
                    new float3(xex + yey, height, xey - yex)
                    );

                return true;

            }
            else
            {
                // No Intersection, far outside or one circle within the other
                line = Segment.zero;
                return false;
            }
        }

        /// <summary>
        /// Find the intersection line between this circle and another, if it exists
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="line"></param>
        /// <param name="axis"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool TryGetIntersection(Circle circle, out Segment line, AxisPair axis, float height = 0f)
        {
            return axis == AxisPair.XY ? 
                TryGetIntersectionXY(circle, out line, height) :
                TryGetIntersectionXZ(circle, out line, height);
        }

        #endregion

        #region Circle-line intersection

        /// <summary>
        /// Find the intersection point between that circle and the A,B line 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersectionXY(float3 A, float3 B,
            out float3 intersection1, out float3 intersection2, float height = 0f)
        {

            float
                cx = center.x,
                cy = center.y,
                Ax = A.x, Ay = A.y, Bx = B.x, By = B.y,
                dx = Bx - Ax,
                dy = By - Ay,
                magA = dx * dx + dy * dy,
                magB = 2f * (dx * (Ax - cx) + dy * (Ay - cy)),
                magC = (Ax - cx) * (Ax - cx) + (Ay - cy) * (Ay - cy) - radius * radius,
                det = magB * magB - 4f * magA * magC,
                sqDet, t;

            if ((magA <= float.Epsilon) || (det < 0f))
            {
                // No real solutions.
                intersection1 = float3(float.NaN, float.NaN, height);
                intersection2 = float3(float.NaN, float.NaN, height);

                return 0;
            }

            if (det == 0)
            {
                // One solution.
                t = -magB / (2f * magA);

                intersection1 = float3(Ax + t * dx, Ay + t * dy, height);
                intersection2 = float3(float.NaN, float.NaN, height);

                return 1;
            }
            else
            {
                // Two solutions.
                sqDet = sqrt(det);

                t = ((-magB + sqDet) / (2f * magA));
                intersection1 = float3(Ax + t * dx, Ay + t * dy, height);

                t = ((-magB - sqDet) / (2f * magA));
                intersection2 = float3(Ax + t * dx, Ay + t * dy, height);

                return 2;
            }
        }

        /// <summary>
        /// Find the intersection point between that circle and the A,B line 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersectionXZ(float3 A, float3 B,
            out float3 intersection1, out float3 intersection2, float height = 0f)
        {

            float
                cx = center.x,
                cy = center.z,
                Ax = A.x, Ay = A.z, Bx = B.x, By = B.z,
                dx = Bx - Ax,
                dy = By - Ay,
                magA = dx * dx + dy * dy,
                magB = 2f * (dx * (Ax - cx) + dy * (Ay - cy)),
                magC = (Ax - cx) * (Ax - cx) + (Ay - cy) * (Ay - cy) - radius * radius,
                det = magB * magB - 4f * magA * magC,
                sqDet, t;

            if ((magA <= float.Epsilon) || (det < 0f))
            {
                // No real solutions.
                intersection1 = float3(float.NaN, height, float.NaN);
                intersection2 = float3(float.NaN, height, float.NaN);

                return 0;
            }

            if (det == 0)
            {
                // One solution.
                t = -magB / (2f * magA);

                intersection1 = float3(Ax + t * dx, height, Ay + t * dy);
                intersection2 = float3(float.NaN, height, float.NaN);

                return 1;
            }
            else
            {
                // Two solutions.
                sqDet = sqrt(det);

                t = ((-magB + sqDet) / (2f * magA));
                intersection1 = float3(Ax + t * dx, height, Ay + t * dy);

                t = ((-magB - sqDet) / (2f * magA));
                intersection2 = float3(Ax + t * dx, height, Ay + t * dy);

                return 2;
            }
        }

        /// <summary>
        /// Find the intersection point between that circle and the A,B line 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersection(float3 A, float3 B,
            out float3 intersection1, out float3 intersection2, AxisPair axis, float height = 0f)
        {
            return axis == AxisPair.XY ? 
                TryGetIntersectionXY(A, B, out intersection1, out intersection2, height) :
                TryGetIntersectionXZ(A, B, out intersection1, out intersection2, height);
        }

        #endregion

        #region Circle-segment intersection

        /// <summary>
        /// Find the intersection point between that circle and a segment
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersectionXY(Segment segment,
            out float3 intersection1, out float3 intersection2, float height = 0f)
        {

            float3
                A = segment.A,
                B = segment.B;

            float
                cx = center.x,
                cy = center.y,
                Ax = A.x, Ay = A.y, Bx = B.x, By = B.y,
                dx = Bx - Ax,
                dy = By - Ay,
                magA = dx * dx + dy * dy,
                magB = 2f * (dx * (Ax - cx) + dy * (Ay - cy)),
                magC = (Ax - cx) * (Ax - cx) + (Ay - cy) * (Ay - cy) - radius * radius,
                det = magB * magB - 4f * magA * magC,
                sqDet, t;

            if ((magA <= float.Epsilon) || (det < 0f))
            {
                // No real solutions.
                intersection1 = float3(float.NaN, float.NaN, height);
                intersection2 = float3(float.NaN, float.NaN, height);

                return 0;
            }

            if (det == 0)
            {
                // One solution.
                t = -magB / (2f * magA);

                intersection1 = float3(Ax + t * dx, Ay + t * dy, height);
                intersection2 = float3(float.NaN, float.NaN, height);

                if (!segment.IsBetweenXZ(intersection1))
                    return 0;

                return 1;
            }
            else
            {
                // Two solutions.
                sqDet = sqrt(det);

                t = ((-magB + sqDet) / (2f * magA));
                intersection1 = float3(Ax + t * dx, Ay + t * dy, height);

                t = ((-magB - sqDet) / (2f * magA));
                intersection2 = float3(Ax + t * dx, Ay + t * dy, height);

                if (!segment.IsBetweenXZ(intersection1))
                {
                    if (!segment.IsBetweenXZ(intersection2))
                        return 0;

                    intersection1 = intersection2;
                    return 1;
                }
                else if (!segment.IsBetweenXZ(intersection2))
                {
                    return 1;
                }

                return 2;
            }
        }


        /// <summary>
        /// Find the intersection point between that circle and a segment
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersectionXZ(Segment segment,
            out float3 intersection1, out float3 intersection2, float height = 0f)
        {

            float3
                A = segment.A,
                B = segment.B;

            float
                cx = center.x,
                cy = center.z,
                Ax = A.x, Ay = A.z, Bx = B.x, By = B.z,
                dx = Bx - Ax,
                dy = By - Ay,
                magA = dx * dx + dy * dy,
                magB = 2f * (dx * (Ax - cx) + dy * (Ay - cy)),
                magC = (Ax - cx) * (Ax - cx) + (Ay - cy) * (Ay - cy) - radius * radius,
                det = magB * magB - 4f * magA * magC,
                sqDet, t;

            if ((magA <= float.Epsilon) || (det < 0f))
            {
                // No real solutions.
                intersection1 = float3(float.NaN, height, float.NaN);
                intersection2 = float3(float.NaN, height, float.NaN);

                return 0;
            }

            if (det == 0)
            {
                // One solution.
                t = -magB / (2f * magA);

                intersection1 = float3(Ax + t * dx, height, Ay + t * dy);
                intersection2 = float3(float.NaN, height, float.NaN);

                if (!segment.IsBetweenXZ(intersection1))
                    return 0;

                return 1;
            }
            else
            {
                // Two solutions.
                sqDet = sqrt(det);

                t = ((-magB + sqDet) / (2f * magA));
                intersection1 = float3(Ax + t * dx, height, Ay + t * dy);

                t = ((-magB - sqDet) / (2f * magA));
                intersection2 = float3(Ax + t * dx, height, Ay + t * dy);

                if (!segment.IsBetweenXZ(intersection1))
                {
                    if (!segment.IsBetweenXZ(intersection2))
                        return 0;

                    intersection1 = intersection2;
                    return 1;
                }
                else if (!segment.IsBetweenXZ(intersection2))
                {
                    return 1;
                }

                return 2;
            }
        }

        /// <summary>
        /// Find the intersection point between that circle and a segment
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <param name="axis"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int TryGetIntersection(Segment segment,
            out float3 intersection1, out float3 intersection2, AxisPair axis, float height = 0f)
        {
            return axis == AxisPair.XY ?
                TryGetIntersectionXY(segment, out intersection1, out intersection2, height) :
                TryGetIntersectionXZ(segment, out intersection1, out intersection2, height);
        }

        #endregion

        public static implicit operator float3(Circle c) { return c.center; }
    }

}
