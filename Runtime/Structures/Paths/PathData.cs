using UnityEngine;
using Unity.Collections;

namespace Nebukam.Geom
{
    /// <summary>
    /// Job-friendly snapshot of a GPath instance.
    /// </summary>
    public struct PathData
    {

        [ReadOnly]
        NativeArray<VertexData> pointList;
        
        /// <summary>
        /// Create a Job-friendly snapshot of a given managed path.
        /// Make sure to Dispose of the NativeArrays afterward.
        /// </summary>
        /// <param name="mPath"></param>
        /// <param name="jobData"></param>
        public static void CreateFrom( ManagedPath mPath, out PathData jobData )
        {

            jobData = new PathData();

            int count = mPath.Count;
            NativeArray<VertexData> pointList = new NativeArray<VertexData>(count, Allocator.Persistent);
            for(int i = 0; i < count; i++)
            {
                pointList[i] = new VertexData(i, mPath[i]);
            }

            jobData.pointList = pointList;
            
        }

        /// <summary>
        /// Create a Job-friendly snapshot of a given path.
        /// Make sure to Dispose of the NativeArrays afterward.
        /// </summary>
        /// <param name="mPath"></param>
        /// <param name="jobData"></param>
        public static void CreateFrom(Path mPath, out PathData jobData)
        {

            jobData = new PathData();

            int count = mPath.Count;
            NativeArray<VertexData> pointList = new NativeArray<VertexData>(count, Allocator.Persistent);
            for (int i = 0; i < count; i++)
            {
                pointList[i] = new VertexData(i, mPath[i]);
            }

            jobData.pointList = pointList;

        }

    }
}
