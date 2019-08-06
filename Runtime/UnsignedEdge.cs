using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nebukam.Geom
{
    
    /// <summary>
    /// Edge with unsigned HashCode.
    /// </summary>
    public struct UnsignedEdge : IEquatable<UnsignedEdge>
    {

        public const int Hh = int.MaxValue / 2;
        public int A, B, d, h;//, d;
        
        public UnsignedEdge(int a, int b)
        {
            d = 0;
            A = a;
            B = b;

            unchecked // Overflow is fine, just wrap
            {

                h = 1000000; //max edge count in a hashmap

                if (A > B)
                {
                    h *= A;
                    h += B;
                }
                else
                {
                    h *= B;
                    h += A;
                }

            }

        }

        public static bool operator !=(UnsignedEdge e1, UnsignedEdge e2)
        {
            return !((e1.A == e2.A && e1.B == e2.B) || (e1.A == e2.B && e1.B == e2.A));
        }

        public static bool operator ==(UnsignedEdge e1, UnsignedEdge e2)
        {
            return (e1.A == e2.A && e1.B == e2.B) || (e1.A == e2.B && e1.B == e2.A);
        }

        public bool Equals( UnsignedEdge e)
        {
            return (e.A == A && e.B == B) || (e.A == B && e.B == A);
        }

        public override bool Equals(object obj)
        {
            UnsignedEdge e = (UnsignedEdge)obj;
            return (e.A == A && e.B == B) || (e.A == B && e.B == A);
        }

        public override int GetHashCode()
        {
            return h;
            /*
            unchecked // Overflow is fine, just wrap
            {

                int h = 100000; //max edge count in a hashmap

                if (A > B)
                {
                    h *= A;
                    h += B;
                }
                else
                {
                    h *= B;
                    h += A;
                }

                return h;

            }
            */
        }

    }

}