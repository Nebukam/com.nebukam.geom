using UnityEngine;

namespace Nebukam.Geom
{
    public struct Circle
    {

        public Vector3 center;
        public Vector3 normal;
        public Vector3 dir;
        public float radius;

        public Circle(float r = 1f)
            : this(Vector3.zero, r, Vector3.up, Vector3.forward)
        {

        }

        public Circle(Vector3 c, float r)
            : this(c, r, Vector3.up, Vector3.forward)
        {

        }

        public Circle(Vector3 c, float r, Vector3 n, Vector3 d)
        {
            center = c;
            radius = r;
            normal = n;
            dir = d;
        }

        public Vector3 AtRadians(float radAngle)
        {
            Vector3 n = (Quaternion.AngleAxis(radAngle / 0.0174532924f, normal) * Vector3.Cross(normal, dir).normalized) * radius;
            n.x = center.x + n.x;
            n.y = center.y + n.y;
            n.z = center.z + n.z;
            return n;
        }

        public Vector3 AtAngle(float angle)
        {
            Vector3 n = (Quaternion.AngleAxis(angle, normal) * Vector3.Cross(normal, dir).normalized) * radius;
            n.x = center.x + n.x;
            n.y = center.y + n.y;
            n.z = center.z + n.z;
            return n;
        }

        public bool Intersects(Circle circle)
        {

            Vector3 otherCenter = circle.center;

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

            Vector3 otherCenter = circle.center;

            float dx = (otherCenter.x - center.x),
                dy = (otherCenter.z - center.z),
                d = Mathf.Sqrt(dx * dx + dy * dy),
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
                    new Vector3(xex - yey, height, xey + yex),
                    new Vector3(xex + yey, height, xey - yex)
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

        public static implicit operator Vector3(Circle c) { return c.center; }
    }

}
