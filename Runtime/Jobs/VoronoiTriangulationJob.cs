// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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

using Nebukam.JobAssist;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Geom
{

    [BurstCompile]
    internal struct IndexedVertex
    {
        public int index;
        public float angle;
    }

    [BurstCompile]
    internal struct SortIndexedVertex : IComparer<IndexedVertex>
    {
        public int Compare(IndexedVertex a, IndexedVertex b)
        {
            return a.angle > b.angle ? -1 : a.angle == b.angle ? 0 : 1;
        }
    }

    [BurstCompile]
    public struct VoronoiTriangulationJob : IJob
    {

        [ReadOnly]
        public NativeList<float3> inputVertices;
        [ReadOnly]
        public NativeList<float3> inputSitesVertices;
        [ReadOnly]
        public NativeMultiHashMap<int, int> inputSites; //Key is input vertice, Values are connected Sites index
        [ReadOnly]
        public NativeHashMap<int, UIntPair> inputUnorderedHullEdges;
        public AxisPair plane;

        public bool computeTriadCentroid;

        public NativeList<float3> outputVertices;
        public NativeList<Triad> outputTriangles;
        public NativeList<int> outputHullVertices;
        public NativeHashMap<int, UIntPair> outputUnorderedHullEdges;


        public void Execute()
        {

            int vCount = inputVertices.Length, sCount = inputSitesVertices.Length, totalCount = vCount + sCount,
                A, B, C, tCount, nIndex;
            Triad triad;
            float3 center, pt;
            NativeList<int> neighbors = new NativeList<int>(10, Allocator.Temp);
            NativeArray<IndexedVertex> neighborsList = new NativeArray<IndexedVertex>(0, Allocator.Temp);
            UIntPair edge;
            bool check = false;

            outputVertices.Clear();
            outputVertices.Capacity = totalCount;

            outputTriangles.Clear();
            outputTriangles.Capacity = sCount * 3;

            //Merge base vertices and voronoi sites in a single list
            for (int i = 0; i < vCount; i++) { outputVertices.Add(inputVertices[i]); }
            for (int i = 0; i < sCount; i++) { outputVertices.Add(inputSitesVertices[i]); }

            for (int i = 0; i < vCount; i++)
            {

                //A being the base vertice
                A = i;

                check = inputUnorderedHullEdges.TryGetValue(A, out edge);
                if (check) { continue; }

                neighbors.Clear();
                tCount = inputSites.PushValues(ref i, ref neighbors);

                neighborsList.Release();
                neighborsList = new NativeArray<IndexedVertex>(tCount, Allocator.Temp);

                center = inputVertices[A];
                for (int v = 0; v < tCount; v++)
                {
                    nIndex = neighbors[v];
                    pt = inputSitesVertices[nIndex] - center;
                    IndexedVertex iv = new IndexedVertex()
                    {
                        index = vCount + nIndex,
                        angle = plane == AxisPair.XY ? atan2(pt.y, pt.x) : atan2(pt.z, pt.x)
                    };
                    neighborsList[v] = iv;
                }

                neighborsList.Sort(new SortIndexedVertex());

                for (int v = 0; v < tCount; v++)
                {
                    B = neighborsList[v].index;
                    C = v + 1;

                    if (C >= tCount)
                    {
                        C = neighborsList[0].index;
                    }
                    else
                    {
                        C = neighborsList[v + 1].index;
                    }

                    if (check)
                    {
                        if (inputUnorderedHullEdges.TryGetValue(B - vCount, out edge)
                        || inputUnorderedHullEdges.TryGetValue(C - vCount, out edge))
                        {
                            continue;
                        }
                    }


                    triad = new Triad(A, B, C, float3(0f), 0f);
                    outputTriangles.Add(triad);
                }

            }

        }

    }
}
