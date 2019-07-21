using UnityEngine;
using Unity.Collections;

namespace Nebukam.Geom
{
    /// <summary>
    /// Job-friendly snapshot of a GPath instance.
    /// </summary>
    public struct PathJobData
    {

        [ReadOnly]
        NativeArray<PathPointJobData> pointList;
        
        /// <summary>
        /// Create a Job-friendly snapshot of a given managed path.
        /// Make sure to Dispose of the NativeArrays afterward.
        /// </summary>
        /// <param name="mPath"></param>
        /// <param name="jobData"></param>
        public static void CreateFrom( ManagedPath mPath, out PathJobData jobData )
        {

            jobData = new PathJobData();

            int count = mPath.Count;
            NativeArray<PathPointJobData> pointList = new NativeArray<PathPointJobData>(count, Allocator.Persistent);
            for(int i = 0; i < count; i++)
            {
                pointList[i] = new PathPointJobData(i, mPath[i]);
            }

            jobData.pointList = pointList;
            
        }

        /// <summary>
        /// Create a Job-friendly snapshot of a given path.
        /// Make sure to Dispose of the NativeArrays afterward.
        /// </summary>
        /// <param name="mPath"></param>
        /// <param name="jobData"></param>
        public static void CreateFrom(Path mPath, out PathJobData jobData)
        {

            jobData = new PathJobData();

            int count = mPath.Count;
            NativeArray<PathPointJobData> pointList = new NativeArray<PathPointJobData>(count, Allocator.Persistent);
            for (int i = 0; i < count; i++)
            {
                pointList[i] = new PathPointJobData(i, mPath[i]);
            }

            jobData.pointList = pointList;

        }

    }
}
