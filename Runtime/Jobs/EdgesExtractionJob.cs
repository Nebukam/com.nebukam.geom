// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

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

            int triCount = inputTriangles.Length, A, B, C;
            NativeHashMap<int, bool> m_hash = new NativeHashMap<int, bool>(triCount, Allocator.Temp);

            bool bAB = false, bBC = false, bCA = false, r;
            int hAB, hBC, hCA;
            UnsignedEdge AB, BC, CA;
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
