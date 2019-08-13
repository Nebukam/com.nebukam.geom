﻿using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.JobAssist;
using Nebukam.Common;

namespace Nebukam.Geom
{

    internal struct IndexedVertex
    {
        public int index;
        public float angle;
    }


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
        public NativeHashMap<int, UnsignedEdge> inputUnorderedHullEdges;
        public AxisPair plane;

        public bool computeTriadCentroid;

        public NativeList<float3> outputVertices;
        public NativeList<Triad> outputTriangles;
        public NativeList<int> outputHullVertices;
        public NativeHashMap<int, UnsignedEdge> outputUnorderedHullEdges;


        public void Execute()
        {

            int vCount = inputVertices.Length, sCount = inputSitesVertices.Length, totalCount = vCount + sCount,
                A, B, C, tCount, nIndex;
            Triad triad;
            float3 center, pt;
            NativeList<int> neighbors = new NativeList<int>(10, Allocator.Temp);
            NativeArray<IndexedVertex> neighborsList = new NativeArray<IndexedVertex>(0, Allocator.Temp);
            UnsignedEdge edge;
            bool check = false;

            outputVertices.Clear();
            outputVertices.Capacity = totalCount;

            outputTriangles.Clear();
            outputTriangles.Capacity = sCount * 3;

            //Merge base vertices and voronoi sites in a single list
            for(int i = 0; i < vCount; i++) { outputVertices.Add(inputVertices[i]); }
            for(int i = 0; i < sCount; i++) { outputVertices.Add(inputSitesVertices[i]); }

            for(int i = 0; i < vCount; i++)
            {

                //A being the base vertice
                A = i;

                check = inputUnorderedHullEdges.TryGetValue(A, out edge);
                if (check) { continue; }

                neighbors.Clear();
                tCount = inputSites.PushValues(ref i, ref neighbors);

                neighborsList.Dispose();
                neighborsList = new NativeArray<IndexedVertex>(tCount, Allocator.Temp);

                center = inputVertices[A];
                for (int v = 0; v < tCount; v++)
                {
                    nIndex = neighbors[v];
                    pt = inputSitesVertices[nIndex] - center;
                    IndexedVertex iv = new IndexedVertex(){
                        index = vCount + nIndex,
                        angle = plane == AxisPair.XY ? atan2(pt.y, pt.x) : atan2(pt.z, pt.x)
                    };
                    neighborsList[v] = iv;
                }

                neighborsList.Sort(new SortIndexedVertex());

                for (int v = 0; v < tCount; v++)
                {
                    B = neighborsList[v].index;
                    C = v+1;

                    if(C >= tCount)
                    {
                        C = neighborsList[0].index;
                    }
                    else
                    {
                        C = neighborsList[v+1].index;
                    }

                    if (check)
                    {
                        if (inputUnorderedHullEdges.TryGetValue(B-vCount, out edge)
                        || inputUnorderedHullEdges.TryGetValue(C-vCount, out edge))
                        {
                            continue;
                        }
                    }
                    

                    triad = new Triad(A, B, C, float3(false), 0f);
                    outputTriangles.Add(triad);
                }
                
            }

        }

    }
}
