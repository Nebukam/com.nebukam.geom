using Unity.Collections;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{

    public interface IEdgesProvider : IProcessor
    {
        /// <summary>
        /// The ITriadProvider used during preparation.
        /// </summary>
        ITriadProvider triadProvider { get; }
        NativeList<UnsignedEdge> outputEdges { get; }
    }

}
