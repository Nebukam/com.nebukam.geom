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
using Unity.Collections;

namespace Nebukam.Geom
{

    /// <summary>
    /// An edge extraction pass, to be use in a ProcessingChain.
    /// Requires an ITriadProvider.
    /// </summary>
    public class EdgesExtractionPass : Processor<EdgesExtractionJob>, IEdgesProvider
    {

        protected ITriadProvider m_triadProvider = null;
        protected NativeList<Triad> m_inputTriangles;
        protected NativeList<UIntPair> m_outputEdges = new NativeList<UIntPair>(0, Allocator.Persistent);

        /// <summary>
        /// The ITriadProvider used during preparation.
        /// </summary>
        public ITriadProvider triadProvider { get { return m_triadProvider; } }

        public NativeList<UIntPair> outputEdges { get { return m_outputEdges; } }

        protected override void Prepare(ref EdgesExtractionJob job, float delta)
        {

            if (!TryGetFirstInCompound(out m_triadProvider))
            {
                throw new System.Exception("No ITriadProvider in chain !");
            }

            m_outputEdges.Clear();

            job.inputTriangles = m_triadProvider.outputTriangles;
            job.outputEdges = m_outputEdges;
        }

        protected override void Apply(ref EdgesExtractionJob job)
        {

        }

        protected override void InternalLock()
        {

        }

        protected override void InternalUnlock()
        {

        }

        protected override void InternalDispose()
        {
            m_outputEdges.Dispose();
        }

    }
}
