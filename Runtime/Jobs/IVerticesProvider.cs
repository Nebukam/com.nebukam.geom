using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{

    public interface IVerticesProvider : IProcessor
    {
        NativeArray<float3> outputVertices { get; }
    }

}
