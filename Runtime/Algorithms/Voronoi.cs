using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Nebukam.Common;

namespace Nebukam.Geom.Algorithms
{
    public static class Voronoi
    {
        public static void Process( 
            List<Vertex> inputVertices,
            List<List<int>> outputSites)
        {

        }

        public static void Process(
            List<Vertex> inputVertices,
            List<Triad> inputTriangles,
            List<List<int>> outputSites)
        {

            List<int> siteNeighbors;
            int tCount = inputTriangles.Count;

            for (int i = 0, sCount = outputSites.Count; i < sCount; i++)
            {
                siteNeighbors = outputSites[i];
                if(siteNeighbors == null) { continue; }
                for (int j = 0, ssCount = siteNeighbors.Count; j < sCount; j++) { siteNeighbors.Clear(); }
            }

            outputSites.Clear();

            //For each triangle, build a list of neighbor
            for(int i = 0; i < tCount; i++)
            {
                siteNeighbors = new List<int>();

            }

        }

    }
}
