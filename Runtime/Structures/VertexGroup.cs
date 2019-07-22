using System.Collections.Generic;
using UnityEngine;
using Nebukam.Utils;

namespace Nebukam.Geom
{
    public abstract class VertexGroup
    {


        protected List<Vertex> m_vertices = new List<Vertex>();

        public int Count { get { return m_vertices.Count; } }

        public IList<Vertex> vertices { get { return m_vertices; } }

        public bool loop { get; set; } = true;

        public Vertex this[int index] { get { return m_vertices[index]; } }
        public int this[Vertex v] { get { return m_vertices.IndexOf(v); } }

        /// <summary>
        /// Adds a vertex in the group.
        /// </summary>
        /// <param name="v">The vertex to be added.</param>
        /// <param name="ownVertex">Whether or not this group gets ownership over the vertex.</param>
        /// <returns></returns>
        public Vertex Add(Vertex v, bool ownVertex = true)
        {
            if (m_vertices.Contains(v)) { return null; }

            m_vertices.Add(v);

            if (ownVertex)
            {
                v.group = this;
            }

            return v;
        }

        /// <summary>
        /// Create a vertex in the group, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vertex Add(Vector3 v)
        {
            Vertex vert = new Vertex();
            vert.v = v;
            return Add(vert, true);
        }

        /// <summary>
        /// Create a vertex in the group, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public T Add<T>(Vector3 v)
            where T : Vertex, new()
        {
            Vertex vert = new T();
            vert.v = v;
            return Add(vert, true) as T;
        }

        /// <summary>
        /// Inserts a vertex at a given index in the group.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <param name="ownVertex"></param>
        /// <param name="allowProxy"></param>
        /// <returns></returns>
        public Vertex Insert(int index, Vertex v, bool ownVertex = true)
        {
            Vertex vertex = Add(v, ownVertex);

            if (vertex == null)
                return null;

            m_vertices.Insert(index, vertex);
            m_vertices.Pop();
            return vertex;
        }

        /// <summary>
        /// Create a vertex in the group at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vertex Insert(int index, Vector3 v)
        {
            Vertex vertex = Add(v);

            if (vertex == null)
                return null;

            m_vertices.Insert(index, vertex);
            m_vertices.Pop();
            return vertex;
        }

        /// <summary>
        /// Removes a given vertex from the group.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public Vertex Remove(Vertex v)
        {
            int index = m_vertices.IndexOf(v);
            return RemoveAt(index);
        }

        /// <summary>
        /// Removes the vertex at the given index from the group .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public Vertex RemoveAt(int index)
        {
            Vertex result = m_vertices[index];
            m_vertices.RemoveAt(index);
            return result;
        }

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetNearestVertexIndex(Vertex v)
        {
            int index = -1, count = m_vertices.Count;
            float dist, sDist = float.MaxValue;
            Vector3 A = v.v, B, C;
            Vertex oV;
            for (int i = 0; i < count; i++)
            {
                oV = m_vertices[i];

                if (oV == v) { continue; }

                B = oV.v;

                C.x = A.x - B.x;
                C.y = A.y - B.y;
                C.z = A.z - B.z;

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
        /// Return the the nearest IVertex in group to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vertex GetNearestVertex(Vertex v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_vertices[index];
        }

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetNearestVertexIndex(Vector3 v)
        {
            int index = -1, count = m_vertices.Count;
            float dist, sDist = float.MaxValue;
            Vector3 B, C;
            for (int i = 0; i < count; i++)
            {
                B = m_vertices[i].v;

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
        /// Return the nearest IVertex in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vertex GetNearestVertex(Vector3 v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_vertices[index];
        }



        #endregion
        
    }

}
