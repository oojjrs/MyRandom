using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public interface MyRvoObstacleInterface
    {
        Vector3 Position { get; }
        float Radius { get; }

        bool IsCollidedInXZ(Vector3 pos);
    }
}
