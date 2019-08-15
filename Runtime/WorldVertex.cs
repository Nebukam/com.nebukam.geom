using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{
    public class WorldVertex : Vertex
    {

        /// <summary>
        /// local, anchored position
        /// </summary>
        public float3 local = float3(false);

        /// <summary>
        /// transformed anchor position
        /// Usually updated by a parent AnchorGroup
        /// </summary>
        public float3 world = float3(false);

    }
}
