using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nebukam.Utils;

namespace Nebukam.Geom
{

    /// <summary>
    /// Non-managed version of the ManagedPath class, using Vector3 instead of ManagedPoints
    /// </summary>
    public class Path
    {

        protected List<Vector3> m_vertices = new List<Vector3>();
        
        public int Count { get { return m_vertices.Count; } }

        public bool loop { get; set; } = true;

        public Vector3 this[int index] { get { return m_vertices[index]; } }
        public int this[Vector3 v] { get { return m_vertices.IndexOf(v); } }
        
        /// <summary>
        /// Create a vertex in the path, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 Add(Vector3 v)
        {
            m_vertices.Add(v);
            return v;
        }

        /// <summary>
        /// Create a vertex in the path at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 Insert(int index, Vector3 v)
        {
            m_vertices.Insert(index, v);
            return v;
        }

        /// <summary>
        /// Removes the vertex at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 RemoveAt(int index)
        {
            Vector3 result = m_vertices[index];
            m_vertices.RemoveAt(index);
            return result;
        }

        #region Nearest point in path

        /// <summary>
        /// Return the point index in path of the nearest Vector3 to a given point
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetNearestPointIndex(Vector3 v)
        {
            int index = -1, count = m_vertices.Count;
            float dist, sDist = float.MaxValue;
            Vector3 B, C;
            for (int i = 0; i < count; i++)
            {
                B = m_vertices[i];

                C.x = v.x - B.x;
                C.y = v.y - B.y;
                C.z = v.z - B.z;

                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if (dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the the nearest Vector3 in path to a given Vector3 point
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 GetNearestPoint(Vector3 v)
        {
            int index = GetNearestPointIndex(v);
            if (index == -1) { return Vector3.zero; }
            return m_vertices[index];
        }

        #endregion

        #region Catmull-Rom Spline

        #region Interpolation

        /// <summary>
        ///Get a point on a Catmull-Rom spline.
        ///The percentage is in range 0 to 1, which starts at the second control point and ends at the second last control point. 
        ///The array cPoints should contain all control points. The minimum amount of control points should be 4. 
        ///Based on : https://forum.unity.com/threads/waypoints-and-constant-variable-speed-problems.32954/#post-213942
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 CRInterp(float t)
        {
            int numSections = m_vertices.Count - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt, uu = u * u, uuu = uu * u;

            Vector3 a = m_vertices[currPt],
                b = m_vertices[currPt + 1],
                c = m_vertices[currPt + 2],
                d = m_vertices[currPt + 3];

            float ax = a.x, ay = a.y, az = a.z, amx = -ax, amy = -ay, amz = -az, bx = b.x, by = b.y, bz = b.z, cx = c.x, cy = c.y, cz = c.z, dx = d.x, dy = d.y, dz = d.z;

            return new Vector3(
                .5f * ((amx + 3f * bx - 3f * cx + dx) * uuu + (2f * ax - 5f * bx + 4f * cx - dx) * uu + (amx + cx) * u + 2f * bx),
                .5f * ((amy + 3f * by - 3f * cy + dy) * uuu + (2f * ay - 5f * by + 4f * cy - dy) * uu + (amy + cy) * u + 2f * by),
                .5f * ((amz + 3f * bz - 3f * cz + dz) * uuu + (2f * az - 5f * bz + 4f * cz - dz) * uu + (amz + cz) * u + 2f * bz)
                );
        }

        /// <summary>
        ///Get a point on a Catmull-Rom spline, clamped between two nodes ( index range 1:n-1 ).
        ///The percentage is in range 0 to 1, which starts at the second control point and ends at the second last control point.
        /// </summary>
        /// <param name="from">index range 1:n-1</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 CRInterpClamped(int from, float t)
        {

            float u = t, uu = u * u, uuu = uu * u;

            Vector3 a = m_vertices[from - 1],
                b = m_vertices[from],
                c = m_vertices[from + 1],
                d = m_vertices[from + 2];

            float ax = a.x, ay = a.y, az = a.z, amx = -ax, amy = -ay, amz = -az, bx = b.x, by = b.y, bz = b.z, cx = c.x, cy = c.y, cz = c.z, dx = d.x, dy = d.y, dz = d.z;

            return new Vector3(
                .5f * ((amx + 3f * bx - 3f * cx + dx) * uuu + (2f * ax - 5f * bx + 4f * cx - dx) * uu + (amx + cx) * u + 2f * bx),
                .5f * ((amy + 3f * by - 3f * cy + dy) * uuu + (2f * ay - 5f * by + 4f * cy - dy) * uu + (amy + cy) * u + 2f * by),
                .5f * ((amz + 3f * bz - 3f * cz + dz) * uuu + (2f * az - 5f * bz + 4f * cz - dz) * uu + (amz + cz) * u + 2f * bz)
                );
        }

        #endregion

        #region Velocity

        /// <summary>
        /// Catmul-Rom Spline velocity solver
        /// Require at least 4 points
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 CRVelocity(float t)
        {
            int numSections = m_vertices.Count - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt, uu = u * u;

            Vector3 a = m_vertices[currPt],
                b = m_vertices[currPt + 1],
                c = m_vertices[currPt + 2],
                d = m_vertices[currPt + 3];

            float ax = a.x, ay = a.y, az = a.z, bx = b.x, by = b.y, bz = b.z, cx = c.x, cy = c.y, cz = c.z, dx = d.x, dy = d.y, dz = d.z;

            return new Vector3(
                1.5f * (-ax + 3f * bx - 3f * cx + dx) * uu + (2f * ax - 5f * bx + 4f * cx - d.x) * u + .5f * cx - .5f * ax,
                1.5f * (-ay + 3f * by - 3f * cy + dy) * uu + (2f * ay - 5f * by + 4f * cy - d.y) * u + .5f * cy - .5f * ay,
                1.5f * (-az + 3f * bz - 3f * cz + dz) * uu + (2f * az - 5f * bz + 4f * cz - d.z) * u + .5f * cz - .5f * az
                );

        }

        /// <summary>
        /// Catmul-Rom Spline velocity solver, clamped between two nodes ( index range 1:n-1 ).
        /// Require at least 4 points
        /// </summary>
        /// <param name="from">index range 1:n-1</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 CRVelocityClamped(int from, float t)
        {

            float u = t, uu = u * u;

            Vector3 a = m_vertices[from - 1],
                b = m_vertices[from],
                c = m_vertices[from + 1],
                d = m_vertices[from + 2];

            float ax = a.x, ay = a.y, az = a.z, bx = b.x, by = b.y, bz = b.z, cx = c.x, cy = c.y, cz = c.z, dx = d.x, dy = d.y, dz = d.z;

            return new Vector3(
                1.5f * (-ax + 3f * bx - 3f * cx + dx) * uu + (2f * ax - 5f * bx + 4f * cx - d.x) * u + .5f * cx - .5f * ax,
                1.5f * (-ay + 3f * by - 3f * cy + dy) * uu + (2f * ay - 5f * by + 4f * cy - d.y) * u + .5f * cy - .5f * ay,
                1.5f * (-az + 3f * bz - 3f * cz + dz) * uu + (2f * az - 5f * bz + 4f * cz - d.z) * u + .5f * cz - .5f * az
                );

        }

        #endregion

        #region Closest point on Catmull-Rom Spline

        #endregion

        #endregion

    }

}
