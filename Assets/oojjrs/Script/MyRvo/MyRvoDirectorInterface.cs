using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public interface MyRvoDirectorInterface
    {
        bool Working { get; }

        Vector3 Modify(Vector3 v, float time);
    }
}
