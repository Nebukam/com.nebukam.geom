using Unity.Mathematics;

namespace Nebukam.Geom
{
    public interface IWorldVertexInfos : IVertexInfos
    {
        /// <summary>
        /// local, anchored position
        /// </summary>
        float3 local { get; set; }

        /// <summary>
        /// transformed anchor position
        /// Usually updated by a parent AnchorGroup
        /// </summary>
        float3 world { get; set; }
    }

    public partial struct WorldVertexInfos : IWorldVertexInfos
    {

        #region IVertexInfos

        public float3 m_pos;

        public float3 pos
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        #endregion

        #region IWorldVertexInfos

        public float3 m_local;
        public float3 m_world;

        public float3 local
        {
            get { return m_local; }
            set { m_local = value; }
        }

        public float3 world
        {
            get { return m_world; }
            set { m_world = value; }
        }

        #endregion
        
    }
}
