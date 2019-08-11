using Unity.Collections;
using Nebukam.JobAssist;

namespace Nebukam.Geom
{
    public interface ITriadProvider : IProcessor
    {
        /// <summary>
        /// The IVerticesProvider used during preparation.
        /// </summary>
        bool computeTriadCentroid { get; set; }
        IVerticesProvider verticesProvider { get; }
        NativeList<Triad> outputTriangles { get; }
        NativeList<int> outputHullVertices { get; }
        NativeHashMap<int, UnsignedEdge> outputUnorderedHull { get; }
    }
}
