using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public interface MyRvoObstacleInterface
    {
        bool Alive { get; }
        Vector3 Position { get; }
        float Radius { get; }

        bool IsCollidedInXZ(Vector3 pos);
    }
}
