using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.JobAssist;

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
        public NativeHashMap<int, UnsignedEdge> inputUnorderedHull;

        public NativeList<UnsignedEdge> outputEdges;
        public NativeMultiHashMap<int, int> outputConnections;

        public void Execute()
        {

            int A, B, C, vCount = inputVertices.Length, triCount = inputTriangles.Length;
            float3 vA, vB, vC;
            float AB, BC, CA;
            UnsignedEdge edge;
            Triad triad;
            bool tooLong;
            NativeHashMap<UnsignedEdge, bool> longuestEdges = new NativeHashMap<UnsignedEdge, bool>(triCount * 3, Allocator.Temp);
            NativeHashMap<UnsignedEdge, bool> uniqueEdges = new NativeHashMap<UnsignedEdge, bool>(triCount * 3, Allocator.Temp);

            for (int i = 0; i < triCount; i++)
            {
                triad = inputTriangles[i];
                A = triad.A; B = triad.B; C = triad.C;
                vA = inputVertices[A]; vB = inputVertices[B]; vC = inputVertices[C];
                AB = distancesq(vA, vB);
                BC = distancesq(vB, vC);
                CA = distancesq(vC, vA);

                if(AB > BC && AB > CA)
                {
                    edge = new UnsignedEdge(A, B);
                }
                else if (BC > AB && BC > CA)
                {
                    edge = new UnsignedEdge(B, C);
                }
                else// if (CA > AB && CA > BC)
                {
                    edge = new UnsignedEdge(C, A);
                }

                longuestEdges.TryAdd(edge, true);

            }

            for (int i = 0; i < triCount; i++)
            {
                triad = inputTriangles[i];
                A = triad.A; B = triad.B; C = triad.C;

                edge = new UnsignedEdge(A, B);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                    && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.B, edge.A);
                    outputConnections.Add(edge.A, edge.B);
                    outputEdges.Add(edge);
                }

                edge = new UnsignedEdge(B, C);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                    && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.B, edge.A);
                    outputConnections.Add(edge.A, edge.B);
                    outputEdges.Add(edge);
                }

                edge = new UnsignedEdge(C, A);
                if (!longuestEdges.TryGetValue(edge, out tooLong)
                && !uniqueEdges.TryGetValue(edge, out tooLong))
                {
                    outputConnections.Add(edge.B, edge.A);
                    outputConnections.Add(edge.A, edge.B);
                    outputEdges.Add(edge);
                }
            }


        }

    }
}
