using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{
    public struct Circle
    {
        
        public float3 center;
        public float3 normal;
        public float3 dir;
        public float radius;

        public Circle(float r = 1f)
            : this(float3(0f,0f,0f), r, float3(0f,1f,0f), float3(0f,0f,1f))
        {

        }

        public Circle(float3 c, float r)
            : this(c, r, float3(0f,1f,0f), float3(0f,0f,1f))
        {

        }

        public Circle(float3 c, float r, float3 n, float3 d)
        {
            center = c;
            radius = r;
            normal = n;
            dir = d;
        }

        public float3 AtRadians(float radAngle)
        {
            //float3 n = (Quaternion.AngleAxis(radAngle / 0.0174532924f, normal) * normalize(cross(normal, dir))) * radius;
            float3 n = mul(Unity.Mathematics.quaternion.AxisAngle(normal, radAngle), normalize(cross(normal, dir))) * radius;
            return float3(center.x + n.x, center.y + n.y, center.z + n.z);
        }

        public float3 AtAngle(float angle)
        {
            //float3 n = (Quaternion.AngleAxis(angle, normal) * normalize(cross(normal, dir))) * radius;
            float3 n = mul(Unity.Mathematics.quaternion.AxisAngle(normal, angle / 0.0174532924f), normalize(cross(normal, dir))) * radius;
            return float3(center.x + n.x, center.y + n.y, center.z + n.z);
        }

        public bool Intersects(Circle circle)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.z - center.z),
                d = Mathf.Sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= Mathf.Abs(otherRadius - radius))
                return true;
            else
                return false;

        }

        public bool TryGetIntersection(Circle circle, out Segment line, float height = 0f)
        {

            float3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.z - center.z),
                d = sqrt(dx * dx + dy * dy),
                otherRadius = circle.radius;

            if (d <= (radius + otherRadius) && d >= Mathf.Abs(otherRadius - radius))
            {

                float rr = radius * radius,
                    ex = (otherCenter.x - center.x) / d,
                    ey = (otherCenter.z - center.z) / d,
                    x = (rr - otherRadius * otherRadius + d * d) / (2 * d),
                    y = Mathf.Sqrt(rr - x * x),
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

        public static implicit operator float3(Circle c) { return c.center; }
    }

}
