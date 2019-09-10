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

using Nebukam.JobAssist;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Geom
{

    [BurstCompile]
    public struct VoronoiJob : IJob
    {

        [ReadOnly]
        public NativeList<float3> inputVertices;
        [ReadOnly]
        public NativeList<Triad> inputTriangles;
        [ReadOnly]
        public NativeList<int> inputHullVertices;
        [ReadOnly]
        public NativeHashMap<int, UIntPair> inputUnorderedHull;

        public float centroidWeight;
        public NativeList<float3> outputVertices;
        public NativeMultiHashMap<int, int> outputSites;
        public NativeList<UIntPair> outputEdges;

        public void Execute()
        {

            int A, B, C, siteCount = inputTriangles.Length, edgeIndex = -1, Ti;
            float3 centroid, circumcenter;
            UIntPair edge;
            Triad triad;
            NativeHashMap<UIntPair, int> uniqueEdges = new NativeHashMap<UIntPair, int>(siteCount * 3, Allocator.Temp);
            NativeMultiHashMap<UIntPair, int> connectedTriangles = new NativeMultiHashMap<UIntPair, int>(siteCount * 3, Allocator.Temp);
            NativeList<int> neighbors = new NativeList<int>(0, Allocator.Temp);
            float weight = clamp(centroidWeight, 0f, 1f);

            if (weight == 0f)
            {
                for (int i = 0; i < siteCount; i++)
                {
                    triad = inputTriangles[i];
                    A = triad.A; B = triad.B; C = triad.C;
                    outputVertices.Add(Circumcenter(ref triad, ref inputVertices));

                    connectedTriangles.Add(new UIntPair(A, B), i);
                    connectedTriangles.Add(new UIntPair(B, C), i);
                    connectedTriangles.Add(new UIntPair(C, A), i);
                }
            }
            else if (weight == 1f)
            {
                for (int i = 0; i < siteCount; i++)
                {
                    triad = inputTriangles[i];
                    A = triad.A; B = triad.B; C = triad.C;
                    outputVertices.Add((inputVertices[A] + inputVertices[B] + inputVertices[C]) / 3f);

                    connectedTriangles.Add(new UIntPair(A, B), i);
                    connectedTriangles.Add(new UIntPair(B, C), i);
                    connectedTriangles.Add(new UIntPair(C, A), i);
                }
            }
            else
            {
                for (int i = 0; i < siteCount; i++)
                {
                    triad = inputTriangles[i];
                    A = triad.A; B = triad.B; C = triad.C;

                    centroid = (inputVertices[A] + inputVertices[B] + inputVertices[C]) / 3f;
                    circumcenter = Circumcenter(ref triad, ref inputVertices);

                    outputVertices.Add(lerp(circumcenter, centroid, weight));

                    connectedTriangles.Add(new UIntPair(A, B), i);
                    connectedTriangles.Add(new UIntPair(B, C), i);
                    connectedTriangles.Add(new UIntPair(C, A), i);
                }
            }

            for (int i = 0; i < siteCount; i++)
            {
                triad = inputTriangles[i];
                A = triad.A; B = triad.B; C = triad.C;

                outputSites.Add(A, i);
                outputSites.Add(B, i);
                outputSites.Add(C, i);

                neighbors.Clear();

                edge = new UIntPair(A, B);
                connectedTriangles.PushValues(ref edge, ref neighbors);
                edge = new UIntPair(B, C);
                connectedTriangles.PushValues(ref edge, ref neighbors);
                edge = new UIntPair(C, A);
                connectedTriangles.PushValues(ref edge, ref neighbors);

                for (int t = 0; t < neighbors.Length; t++)
                {
                    Ti = neighbors[t];
                    if (Ti == i) { continue; }

                    edge = new UIntPair(Ti, i);

                    if (!uniqueEdges.TryGetValue(edge, out edgeIndex))
                    {
                        uniqueEdges.TryAdd(edge, outputEdges.Length);
                        outputEdges.Add(edge);
                    }

                }

            }

        }

        private static float3 Circumcenter(ref Triad triad, ref NativeList<float3> vertices)
        {

            float3 vA = vertices[triad.A], vB = vertices[triad.B], vC = vertices[triad.C];

            float dA = vA.x * vA.x + vA.y * vA.y;
            float dB = vB.x * vB.x + vB.y * vB.y;
            float dC = vC.x * vC.x + vC.y * vC.y;

            float aux1 = (dA * (vC.y - vB.y) + dB * (vA.y - vC.y) + dC * (vB.y - vA.y));
            float aux2 = -(dA * (vC.x - vB.x) + dB * (vA.x - vC.x) + dC * (vB.x - vA.x));
            float div = (2 * (vA.x * (vC.y - vB.y) + vB.x * (vA.y - vC.y) + vC.x * (vB.y - vA.y)));

            return float3(aux1 / div, aux2 / div, 0f);

        }

    }
}
