using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nebukam.Utils;

namespace Nebukam.Geom
{

    public interface IGenericManagerPath<TPoint>
        where TPoint : class, IManagedPoint, new()
    {
        
        IList<TPoint> points { get; }
        TPoint this[int index] { get; }
        int this[TPoint pt] { get; }


        /// <summary>
        /// Adds a point in the Path.
        /// </summary>
        /// <param name="pt">The point to be added.</param>
        /// <param name="pathOwnPoint">Whether or not this path gets ownership over the point.</param>
        /// <param name="allowProxy">Whether or not to allow duplicate of the given point.</param>
        /// <returns></returns>
        TPoint Add(TPoint pt, bool pathOwnPoint = false, bool allowProxy = false);

        /// <summary>
        /// Create a point in the path, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        TPoint Add(Vector3 v);

        /// <summary>
        /// Inserts a point at a given index in the path.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pt"></param>
        /// <param name="pathOwnPoint"></param>
        /// <param name="allowProxy"></param>
        /// <returns></returns>
        TPoint Insert(int index, TPoint pt, bool pathOwnPoint = false, bool allowProxy = false);

        /// <summary>
        /// Create a point in the path at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        TPoint Insert(int index, Vector3 v);

        /// <summary>
        /// Removes a given point from the path.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        TPoint Remove(TPoint pt, bool keepProxies = true);

        /// <summary>
        /// Removes the point at the given index from the path .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        TPoint RemoveAt(int index, bool keepProxies = true);

    }


    public interface IManagedPath
    {
        /// <summary>
        /// Number of managed points in the path
        /// </summary>
        int Count { get; }

        bool loop { get; set; }

        /// <summary>
        /// Return the point index in path of the nearest IGPoint to a given IGPoint point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        int GetNearestPointIndex(IManagedPoint pt);

        /// <summary>
        /// Return the the nearest IGPoint in path to a given IGPoint point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        IManagedPoint GetNearestPoint(IManagedPoint pt);

        /// <summary>
        /// Return the point index in path of the nearest IGPoint to a given point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        int GetNearestPointIndex(Vector3 pt);

        /// <summary>
        /// Return the nearest IGPoint in path of the nearest IGPoint to a given point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        IManagedPoint GetNearestPoint(Vector3 pt);

        /// <summary>
        ///Get a point on a Catmull-Rom spline.
        ///The percentage is in range 0 to 1, which starts at the second control point and ends at the second last control point. 
        ///The array cPoints should contain all control points. The minimum amount of control points should be 4. 
        ///Based on : https://forum.unity.com/threads/waypoints-and-constant-variable-speed-problems.32954/#post-213942
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Vector3 CRInterp(float t);

        /// <summary>
        ///Get a point on a Catmull-Rom spline, clamped between two nodes ( index range 1:n-1 ).
        ///The percentage is in range 0 to 1, which starts at the second control point and ends at the second last control point.
        /// </summary>
        /// <param name="from">index range 1:n-1</param>
        /// <param name="t"></param>
        /// <returns></returns>
        Vector3 CRInterpClamped(int from, float t);

        /// <summary>
        /// Catmul-Rom Spline velocity solver
        /// Require at least 4 points
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Vector3 CRVelocity(float t);

        /// <summary>
        /// Catmul-Rom Spline velocity solver, clamped between two nodes ( index range 1:n-1 ).
        /// Require at least 4 points
        /// </summary>
        /// <param name="from">index range 1:n-1</param>
        /// <param name="t"></param>
        /// <returns></returns>
        Vector3 CRVelocityClamped(int from, float t);
        void Clear();
    }

    /// <summary>
    /// Managed path allow tight point management as well as custom Point types, and as such may be used for tool authoring
    /// while still retaining path-related functionalities.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    /// <typeparam name="TProxyPoint"></typeparam>
    public abstract class AbstractManagedPath<TPoint, TProxyPoint> : IGenericManagerPath<TPoint>, IManagedPath
        where TPoint : class, IManagedPoint, new()
        where TProxyPoint : class, TPoint, IManagedPointProxy, new()
    {

        protected List<TPoint> m_points = new List<TPoint>();
        protected HashSet<TPoint> m_pointHash = new HashSet<TPoint>();

        public int Count { get { return m_points.Count; } }

        public IList<TPoint> points { get { return m_points; } }

        public bool loop { get; set; } = true;

        public TPoint this[int index] { get { return m_points[index]; } }
        public int this[TPoint pt] { get { return m_points.IndexOf(pt); } }
        
        /// <summary>
        /// Adds a point in the Path.
        /// </summary>
        /// <param name="pt">The point to be added.</param>
        /// <param name="pathOwnPoint">Whether or not this path gets ownership over the point.</param>
        /// <param name="allowProxy">Whether or not to allow duplicate of the given point.</param>
        /// <returns></returns>
        public TPoint Add(TPoint pt, bool pathOwnPoint = false, bool allowProxy = false)
        {            
            if(m_points.Contains(pt))
            {
                if (!allowProxy)
                    return null;

                TProxyPoint proxy = new TProxyPoint();
                proxy.source = pt;
                pt = proxy;
                pathOwnPoint = true;
            }

            m_points.Add(pt);

            if (pathOwnPoint)
                m_pointHash.Add(pt);

            return pt;
        }

        /// <summary>
        /// Create a point in the path, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public TPoint Add(Vector3 v)
        {
            TPoint pt = new TPoint();
            pt.v = v;
            m_points.Add(pt);
            m_pointHash.Add(pt);
            return pt;
        }

        /// <summary>
        /// Inserts a point at a given index in the path.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pt"></param>
        /// <param name="pathOwnPoint"></param>
        /// <param name="allowProxy"></param>
        /// <returns></returns>
        public TPoint Insert(int index, TPoint pt, bool pathOwnPoint = false, bool allowProxy = false)
        {
            TPoint point = Add(pt, pathOwnPoint, allowProxy);

            if (point == null)
                return null;

            m_points.Insert(index, point);
            m_points.Pop();
            return point;
        }

        /// <summary>
        /// Create a point in the path at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public TPoint Insert(int index, Vector3 v)
        {
            TPoint point = Add(v);

            if (point == null)
                return null;

            m_points.Insert(index, point);
            m_points.Pop();
            return point;
        }

        /// <summary>
        /// Removes a given point from the path.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public TPoint Remove(TPoint pt, bool keepProxies = true)
        {
            int index = m_points.IndexOf(pt);
            return RemoveAt(index, keepProxies);
        }

        /// <summary>
        /// Removes the point at the given index from the path .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public TPoint RemoveAt(int index, bool keepProxies = true)
        {
            TPoint result = m_points[index];
            //TODO : Remove given point.
            //If not proxy, check if there are existing proxies for the removed point
            return result;
        }

        #region Nearest point in path

        /// <summary>
        /// Return the point index in path of the nearest IGPoint to a given IGPoint point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int GetNearestPointIndex(IManagedPoint pt)
        {
            int index = -1, count = m_points.Count;
            float dist, sDist = float.MaxValue;
            Vector3 A = pt.v, B, C;
            IManagedPoint otherPt;
            for(int i = 0; i < count; i++)
            {
                otherPt = m_points[i];

                if (otherPt == pt) { continue; }

                B = otherPt.v;
                
                C.x = A.x - B.x;
                C.y = A.y - B.y;
                C.z = A.z - B.z;

                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if(dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the the nearest IGPoint in path to a given IGPoint point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public IManagedPoint GetNearestPoint(IManagedPoint pt)
        {
            int index = GetNearestPointIndex(pt);
            if(index == -1) { return null; }
            return m_points[index];
        }

        /// <summary>
        /// Return the point index in path of the nearest IGPoint to a given point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int GetNearestPointIndex(Vector3 pt)
        {
            int index = -1, count = m_points.Count;
            float dist, sDist = float.MaxValue;
            Vector3 B, C;
            for (int i = 0; i < count; i++)
            {
                B = m_points[i].v;

                C.x = pt.x - B.x;
                C.y = pt.y - B.y;
                C.z = pt.z - B.z;

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
        /// Return the nearest IGPoint in path of the nearest IGPoint to a given point
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public IManagedPoint GetNearestPoint(Vector3 pt)
        {
            int index = GetNearestPointIndex(pt);
            if(index == -1) { return null; }
            return m_points[index];
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
            int numSections = m_points.Count - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt, uu = u*u, uuu = uu * u;

            Vector3 a = m_points[currPt].v, 
                b = m_points[currPt + 1].v,
                c = m_points[currPt + 2].v,
                d = m_points[currPt + 3].v;

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

            Vector3 a = m_points[from-1].v,
                b = m_points[from].v,
                c = m_points[from + 1].v,
                d = m_points[from + 2].v;

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
            int numSections = m_points.Count - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt, uu = u * u;

            Vector3 a = m_points[currPt].v,
                b = m_points[currPt + 1].v,
                c = m_points[currPt + 2].v,
                d = m_points[currPt + 3].v;

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

            Vector3 a = m_points[from - 1].v,
                b = m_points[from].v,
                c = m_points[from + 1].v,
                d = m_points[from + 2].v;

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

        public void Clear()
        {
            m_points.Clear();
            m_pointHash.Clear();
        }

    }

    public class ManagedPath : AbstractManagedPath<ManagedPoint, ManagedPointProxy>
    {

    }

}
