using UnityEngine;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        private class FlatTopped : LogicInterface
        {
            Vector2 LogicInterface.FromWorld(Vector2 v)
            {
                var st = Mathf.Sqrt(3);
                var qf = 2 / 3f * v.x;
                var rf = -1 / 3f * v.x + st / 3 * v.y;
                return new(qf, rf);
            }

            float LogicInterface.GetCornerAngleAsDegree(int index) => 60 * index;
            float LogicInterface.GetHeight(float size) => size * Mathf.Sqrt(3);
            float LogicInterface.GetHorizontalDistance(float size) => ((LogicInterface)this).GetWidth(size) * 0.75f;
            float LogicInterface.GetVerticalDistance(float size) => ((LogicInterface)this).GetHeight(size);
            float LogicInterface.GetWidth(float size) => size * 2;

            Vector2 LogicInterface.ToWorld(int q, int r)
            {
                var st = Mathf.Sqrt(3);
                var x = 3 / 2f * q;
                var y = st / 2 * q + st * r;
                return new(x, y);
            }
        }
    }
}
