using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

namespace Nebukam.Geom
{
    
    public struct DelaunayJob : IJob
    {

        [ReadOnly]
        public NativeArray<float3> inputVertices;
        public NativeList<Triad> outputTriangles;
        
        public bool computeTriadCentroid;

        public void Execute()
        {
            outputTriangles.Clear();
            NativeList<UnsignedEdge> hole = new NativeList<UnsignedEdge>(20, Allocator.Temp);

            float m = 100f;
            float3 V, vA, vB, vC, bA, bB, bC, bD, centroid;
            int vCount = inputVertices.Length, A = vCount, B = vCount + 1, C = vCount + 2, D = vCount + 3, iA, iB, extraVCount = 3;
            Triad triad;
            NativeArray<float3> vertices = new NativeArray<float3>(vCount + extraVCount, Allocator.Temp);

            #region Create enclosing quad

            float maxX = float.MinValue, maxY = float.MinValue, minX = float.MaxValue, minY = float.MaxValue;

            //Find the min-max positions
            for (int index = 0; index < vCount; index++)
            {
                V = inputVertices[index];
                vertices[index] = V;
                if (V.x > maxX) { maxX = V.x; } else if (V.x < minX) { minX = V.x; }
                if (V.y > maxY) { maxY = V.y; } else if (V.y < minY) { minY = V.y; }
            }

            //Offset min/max to ensure proper enclosure
            minX *= m; minY *= m; maxX *= m; maxY *= m;
            /*
            bA = new float3(minX, maxY); bB = new float3(maxX, maxY);
            bC = new float3(maxX, minY); bD = new float3(minX, minY);

            vertices[A] = bA; vertices[B] = bB;
            vertices[C] = bC; vertices[D] = bD;

            Triad(out triad, A, B, C, ref vertices);
            outputTriangles.Add(triad);

            Triad(out triad, A, C, D, ref vertices);
            outputTriangles.Add(triad);
            */

            #region single enclosing triangle

            float width = maxX - minX, height = maxY - minY;
            centroid = new float3(minX + width * 0.5f, minY + 0.5f, 0f);

            bA = new float3(centroid.x - (width * 2f), minY - 0.5f, 0f); bB = new float3(centroid.x + (width * 2f), minY - 0.5f, 0f);
            bC = new float3(centroid.x, centroid.y - height * 0.5f + height * 2f, 0f);

            vertices[A] = bA; vertices[B] = bB;
            vertices[C] = bC;

            Triad(out triad, A, B, C, ref vertices);
            outputTriangles.Add(triad);

            #endregion

            #endregion

            int triCount, eCount = 0, t = 0;
            bool bAB = false, bBC = false, bCA = false, inc = false;
            UnsignedEdge edge, AB, BC, CA, EE;

            for (int index = 0; index < vCount; index++)
            {

                V = vertices[index];

                #region bad triangles

                //Find & remove bad triangles in current triangulation, given the current vertex
                triCount = outputTriangles.Length;
                t = 0;

                hole.Clear();

                for (int i = 0; i < triCount; i++)
                {
                    triad = outputTriangles[i];

                    //CircumcircleContainsXY
                    if (((V.x - triad.circumcenter.x) * (V.x - triad.circumcenter.x) + (V.y - triad.circumcenter.y) * (V.y - triad.circumcenter.y)) < triad.sqRadius)
                    {

                        //This is a bad triangle.
                        //Add edges forming the triangle to the list of hole boundaries
                        A = triad.A; B = triad.B; C = triad.C;
                        AB = new UnsignedEdge(A, B);
                        BC = new UnsignedEdge(B, C);
                        CA = new UnsignedEdge(C, A);

                        #region Loop

                        bAB = false; bBC = false; bCA = false; inc = false;
                        eCount = hole.Length;
                        for (int ie = 0; ie < eCount; ie++)
                        {
                            EE = hole[ie];

                            //if (EE.d != 0) { continue; }

                            inc = false;
                            iA = EE.A; iB = EE.B;

                            if (!bAB && ((iA == A && iB == B) || (iA == B && iB == A))) { inc = bAB = true; }
                            else if (!bBC && ((iA == B && iB == C) || (iA == C && iB == B))) { inc = bBC = true; }
                            else if (!bCA && ((iA == C && iB == A) || (iA == A && iB == C))) { inc = bCA = true; }

                            if (inc)
                            {
                                EE.d ++;
                                hole[ie] = EE;
                            }
                        }

                        if (!bAB) { hole.Add(AB); }
                        if (!bBC) { hole.Add(BC); }
                        if (!bCA) { hole.Add(CA); }

                        #endregion

                        //If constructing Voronoi, remove triangle from each point 'linked' triangles
                        //(do this here, as triad gets removed.)
                    }
                    else
                    {
                        //Move good triangle to the next good index
                        outputTriangles[t] = triad;
                        t++;
                    }
                }

                //Truncate triangulation
                outputTriangles.ResizeUninitialized(t);
                triCount = t;

                #endregion

                #region new triangles

                eCount = hole.Length;
                
                for (int e = 0; e < eCount; e++)
                {
                    edge = hole[e];
                    if (edge.d != 0) { continue; }
                    A = edge.A; B = edge.B;
                    vA = vertices[A]; vB = vertices[B];

                    Triad(out triad, index, A, B, ref vertices);

                    outputTriangles.Add(triad);
                }

                #endregion

                hole.Clear();

            }

            #region wrap up
            
            //Remove all triangles with a vertex superior to initial vertice count 
            //as they are linked to initial 4 boundaries vertices
            triCount = outputTriangles.Length;
            t = 0;

            if (computeTriadCentroid)
            {
                for (int i = 0; i < triCount; i++)
                {
                    triad = outputTriangles[i];
                    A = triad.A; B = triad.B; C = triad.C;

                    if (A >= vCount || B >= vCount || C >= vCount)
                        continue;

                    vA = vertices[triad.A];
                    vB = vertices[triad.B];
                    vC = vertices[triad.C];
                    triad.centroid = (vA + vB + vC) / 3f;

                    outputTriangles[t] = triad;
                    t++;
                }
            }
            else
            {
                for (int i = 0; i < triCount; i++)
                {
                    triad = outputTriangles[i];
                    A = triad.A; B = triad.B; C = triad.C;

                    if (A >= vCount || B >= vCount || C >= vCount)
                        continue;

                    outputTriangles[t] = triad;
                    t++;
                }
            }
            
            outputTriangles.ResizeUninitialized(t);

            #endregion

            hole.Dispose();

        }

        private static void Triad(out Triad triad, int A, int B, int C, ref NativeArray<float3> vertices)
        {

            float3 vA = vertices[A], vB = vertices[B], vC = vertices[C], center;

            float dA = vA.x * vA.x + vA.y * vA.y;
            float dB = vB.x * vB.x + vB.y * vB.y;
            float dC = vC.x * vC.x + vC.y * vC.y;

            float aux1 = (dA * (vC.y - vB.y) + dB * (vA.y - vC.y) + dC * (vB.y - vA.y));
            float aux2 = -(dA * (vC.x - vB.x) + dB * (vA.x - vC.x) + dC * (vB.x - vA.x));
            float div = (2 * (vA.x * (vC.y - vB.y) + vB.x * (vA.y - vC.y) + vC.x * (vB.y - vA.y)));

            if (div == 0)
            {
                //throw new System.Exception("div by zero.");
            }

            center = new float3(aux1 / div, aux2 / div, 0f);
            triad = new Triad(A, B, C, center, (center.x - vA.x) * (center.x - vA.x) + (center.y - vA.y) * (center.y - vA.y));

        }

    }
}
