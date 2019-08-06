using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

namespace Nebukam.Geom
{

    [BurstCompile]
    public struct EdgesExtractionJob : IJob
    {
        
        [ReadOnly]
        public NativeList<Triad> inputTriangles;
        
        public NativeList<UnsignedEdge> outputEdges;

        
        public void Execute()
        {

            outputEdges.Clear();

            int triCount = inputTriangles.Length, eCount = 0, A, B, C, iA, iB;
            NativeHashMap<int, bool> m_hash = new NativeHashMap<int, bool>(triCount, Allocator.Temp);

            bool bAB = false, bBC = false, bCA = false, r;
            int hAB, hBC, hCA;
            UnsignedEdge AB, BC, CA, EE;
            Triad triad;
            
            for (int i = 0; i < triCount; i++)
            {

                triad = inputTriangles[i];

                A = triad.A; B = triad.B; C = triad.C;
                AB = new UnsignedEdge(A, B);
                BC = new UnsignedEdge(B, C);
                CA = new UnsignedEdge(C, A);

                hAB = AB.GetHashCode();
                hBC = BC.GetHashCode();
                hCA = CA.GetHashCode();

                #region Loop

                //Fast but consistency drop over ~200k edges.
                bAB = m_hash.TryGetValue(hAB, out r);
                bBC = m_hash.TryGetValue(hBC, out r);
                bCA = m_hash.TryGetValue(hCA, out r);

                /*
                eCount = outputEdges.Length;
                for (int ie = 0; ie < eCount; ie++)
                {
                    EE = outputEdges[ie];
                    
                    iA = EE.A; iB = EE.B;

                    if (!bAB && ((iA == A && iB == B) || (iA == B && iB == A))) { bAB = true; }
                    else if (!bBC && ((iA == B && iB == C) || (iA == C && iB == B))) { bBC = true; }
                    else if (!bCA && ((iA == C && iB == A) || (iA == A && iB == C))) { bCA = true; }

                }
                */
                if (!bAB) { outputEdges.Add(AB); m_hash.TryAdd(hAB, true); }
                if (!bBC) { outputEdges.Add(BC); m_hash.TryAdd(hBC, true); }
                if (!bCA) { outputEdges.Add(CA); m_hash.TryAdd(hCA, true); }

                #endregion
            }



        }
        
    }
}
