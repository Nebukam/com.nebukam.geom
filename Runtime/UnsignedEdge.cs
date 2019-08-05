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
        public int A, B, d;// h, d;
        
        public UnsignedEdge(int a, int b)
        {
            d = 0;
            A = a;
            B = b;
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

            unchecked // Overflow is fine, just wrap
            {

                int h = 0;

                if (A > B)
                {
                    h += Hh + A.GetHashCode();
                    h += B.GetHashCode();
                }
                else
                {
                    h += Hh + B.GetHashCode();
                    h += A.GetHashCode();
                }

                return h;

            }

        }

    }

}