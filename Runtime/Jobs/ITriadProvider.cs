using Unity.Collections;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{
    public interface ITriadProvider : IProcessor
    {
        /// <summary>
        /// The IVerticesProvider used during preparation.
        /// </summary>
        IVerticesProvider verticesProvider { get; }
        NativeList<Triad> outputTriangles { get; }
    }
}
