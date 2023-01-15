using UnityEngine;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        private class PointyTopped : LogicInterface
        {
            Vector2 LogicInterface.FromWorld(Vector2 v)
            {
                var st = Mathf.Sqrt(3);
                var qf = st / 3 * v.x - 1 / 3f * v.y;
                var rf = 2 / 3f * v.y;
                return new(qf, rf);
            }

            // index = 0일 때 위치가 -330도 맞는 거다.
            float LogicInterface.GetCornerAngleAsDegree(int index) => 60 * index - 30;
            float LogicInterface.GetHeight(float size) => size * 2;
            float LogicInterface.GetHorizontalDistance(float size) => ((LogicInterface)this).GetWidth(size);
            float LogicInterface.GetVerticalDistance(float size) => ((LogicInterface)this).GetHeight(size) * 0.75f;
            float LogicInterface.GetWidth(float size) => size * Mathf.Sqrt(3);

            Vector2 LogicInterface.ToWorld(int q, int r)
            {
                var st = Mathf.Sqrt(3);
                var x = st * q + st / 2 * r;
                var y = 3 / 2f * r;
                return new(x, y);
            }
        }
    }
}
