using UnityEngine;

namespace Nebukam.Geom
{
    /// <summary>
    /// Job-friendly snapshot of a GPoint instance.
    /// </summary>
    public struct PathPointJobData
    {
        public int index;
        public Vector3 v;

        public PathPointJobData(int i, Vector3 vector)
        {
            index = i;
            v = vector;
        }

    }
}
