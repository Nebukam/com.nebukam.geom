using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

namespace Nebukam.Geom
{

    public struct EdgesExtractionJob : IJob
    {
        
        [ReadOnly]
        public NativeList<Triad> inputTriangles;
        
        public NativeList<UnsignedEdge> outputEdges;

        
        public void Execute()
        {

            outputEdges.Clear();

            int triCount = inputTriangles.Length, eCount = 0, A, B, C, iA, iB;
            bool bAB = false, bBC = false, bCA = false;
            UnsignedEdge AB, BC, CA, EE;
            Triad triad;
            
            for (int i = 0; i < triCount; i++)
            {

                triad = inputTriangles[i];

                //This is a bad triangle.
                //Add edges forming the triangle to the list of hole boundaries
                A = triad.A; B = triad.B; C = triad.C;
                AB = new UnsignedEdge(A, B);
                BC = new UnsignedEdge(B, C);
                CA = new UnsignedEdge(C, A);

                #region Loop

                bAB = false; bBC = false; bCA = false;
                eCount = outputEdges.Length;
                for (int ie = 0; ie < eCount; ie++)
                {
                    EE = outputEdges[ie];
                    
                    iA = EE.A; iB = EE.B;

                    if (!bAB && ((iA == A && iB == B) || (iA == B && iB == A))) { bAB = true; }
                    else if (!bBC && ((iA == B && iB == C) || (iA == C && iB == B))) { bBC = true; }
                    else if (!bCA && ((iA == C && iB == A) || (iA == A && iB == C))) { bCA = true; }

                }

                if (!bAB) { outputEdges.Add(AB); }
                if (!bBC) { outputEdges.Add(BC); }
                if (!bCA) { outputEdges.Add(CA); }

                #endregion
            }



        }
        
    }
}
