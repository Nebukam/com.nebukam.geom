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

namespace Nebukam.Geom
{

    /// <summary>
    /// Edge with unsigned HashCode.
    /// </summary>
    public struct UnsignedEdge : System.IEquatable<UnsignedEdge>
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

                h = 100000; //max edge count in a hashmap

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

        public UnsignedEdge ascending { get { return A > B ? new UnsignedEdge(B, A) : this; } }

        public UnsignedEdge descending { get { return A < B ? new UnsignedEdge(B, A) : this; } }

        public bool Contains(int i)
        {
            return (A == i || B == i);
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