﻿// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{

    [BurstCompile]
    public struct UrquhartJob : IJob
    {

        [ReadOnly]
        public NativeList<float3> inputVertices;
        [ReadOnly]
        public NativeList<Triad> inputTriangles;
        [ReadOnly]
        public NativeList<int> inputHullVertices;
        [ReadOnly]
        public NativeParallelHashMap<int, UIntPair> inputUnorderedHull;

        public NativeList<UIntPair> outputEdges;
        public NativeParallelMultiHashMap<int, int> outputConnections;

        public void Execute()
        {

            int A, B, C, vCount = inputVertices.Length, triCount = inputTriangles.Length;
            float3 vA, vB, vC;
            float AB, BC, CA;
            UIntPair edge;
            Triad triad;
            bool tooLong;
            NativeParallelHashMap<UIntPair, bool> longuestEdges = new NativeParallelHashMap<UIntPair, bool>(triCount * 3, Allocator.Temp);
            NativeParallelHashMap<UIntPair, bool> uniqueEdges = new NativeParallelHashMap<UIntPair, bool>(triCount * 3, Allocator.Temp);

            for (int i = 0; i < triCount; i++)
            {
                triad = inputTriangles[i];
                A = triad.A; B = triad.B; C = triad.C;
                vA = inputVertices[A]; vB = inputVertices[B]; vC = inputVertices[C];
                AB = distancesq(vA, vB);
                BC = distancesq(vB, vC);
                CA = distancesq(vC, vA);

                if (AB > BC && AB > CA)
                {
                    edge = new UIntPair(A, B);
                }
                else if (BC > AB && BC > CA)
                {
                    edge = new UIntPair(B, C);
                }
                else// if (CA > AB && CA > BC)
                {
                    edge = new UIntPair(C, A);
                }

                longuestEdges.TryAdd(edge, true);

            }

            for (int i = 0; i < triCount; i++)
            {
                triad = inputTriangles[i];
                A = triad.A; B = triad.B; C = triad.C;

                edge = new UIntPair(A, B);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                    && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.y, edge.x);
                    outputConnections.Add(edge.x, edge.y);
                    outputEdges.Add(edge);
                }

                edge = new UIntPair(B, C);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                    && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.y, edge.x);
                    outputConnections.Add(edge.x, edge.y);
                    outputEdges.Add(edge);
                }

                edge = new UIntPair(C, A);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.y, edge.x);
                    outputConnections.Add(edge.x, edge.y);
                    outputEdges.Add(edge);
                }
            }


        }

    }
}
