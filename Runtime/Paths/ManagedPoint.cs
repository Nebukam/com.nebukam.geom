using UnityEngine;

namespace Nebukam.Geom
{
    
    public interface IManagedPoint
    {
        
        float x { get; set; }
        float y { get; set; }
        float z { get; set; }

        Vector3 v { get; set; }

    }

    public interface IManagedPointProxy : IManagedPoint
    {

        IManagedPoint source { get; set; }

    }
    
    public class ManagedPoint : IManagedPoint
    {

        protected Vector3 _v = Vector3.zero;
        public virtual Vector3 v { get { return _v; } set { _v = value; } }

        public virtual float x { get { return _v.x; } set { _v.x = value; } }
        public virtual float y { get { return _v.y; } set { _v.y = value; } }
        public virtual float z { get { return _v.z; } set { _v.z = value; } }
        
        public static implicit operator Vector3(ManagedPoint p) { return p.v; }
        public static implicit operator ManagedPoint(Vector3 p)
        {
            ManagedPoint newPoint = new ManagedPoint();
            newPoint.v = p;

            return newPoint;
        }

    }

    public class ManagedPointProxy : ManagedPoint, IManagedPointProxy
    {

        protected IManagedPoint _source = null;
        public IManagedPoint source { get { return _source; } set { _source = value; } }
        
        public override Vector3 v { get { return _source.v; } set {} }

        public override float x { get { return _source.x; } set {} }
        public override float y { get { return _source.y; } set {} }
        public override float z { get { return _source.z; } set {} }
        
        public static implicit operator Vector3(ManagedPointProxy p) { return p.v; }

    }


}
