using System.Diagnostics;
using UnityEngine;

namespace Assets.Sources.Scripts
{
    // 정확하게 표준(?)과 일치하지 않는다. 유니티와 축을 맞추기 위해 r축이 반전되었으므로 Q의 증가 방향이 30도 돌았다.
    [DebuggerDisplay("({Q}, {R})")]
    public partial struct Hex
    {
        public enum FormEnum
        {
            FlatTopped,
            PointyTopped,
        }

        public interface CoordinateInterface
        {
            Hex Hex { get; }
            int X { get; }
            int Y { get; }
        }

        private interface LogicInterface
        {
            Vector2 FromWorld(Vector2 v);
            float GetCornerAngleAsDegree(int index);
            float GetHeight(float size);
            float GetHorizontalDistance(float size);
            float GetVerticalDistance(float size);
            float GetWidth(float size);
            Vector2 ToWorld(int q, int r);
        }

        public CoordinateInterface AsDoubleOffsetCoordinate => Form switch
        {
            FormEnum.FlatTopped => new FlatToppedDoubleOffsetCoordinate(this),
            FormEnum.PointyTopped => new PointyToppedDoubleOffsetCoordinate(this),
            _ => throw new System.NotImplementedException(),
        };
        public CoordinateInterface AsEvenOffsetCoordinate => Form switch
        {
            FormEnum.FlatTopped => new FlatToppedEvenOffsetCoordinate(this),
            FormEnum.PointyTopped => new PointyToppedEvenOffsetCoordinate(this),
            _ => throw new System.NotImplementedException(),
        };
        public CoordinateInterface AsOddOffsetCoordinate => Form switch
        {
            FormEnum.FlatTopped => new FlatToppedOddOffsetCoordinate(this),
            FormEnum.PointyTopped => new PointyToppedOddOffsetCoordinate(this),
            _ => throw new System.NotImplementedException(),
        };
        public FormEnum Form { get; }
        public float Height => Logic.GetHeight(Size);
        public float HorizontalDistance => Logic.GetHorizontalDistance(Size);
        private LogicInterface Logic { get; }
        public int Q { get; }
        public int R { get; }
        public int S => 0 - Q - R;
        public float Size { get; }
        public float VerticalDistance => Logic.GetVerticalDistance(Size);
        public float Width => Logic.GetWidth(Size);

        public Hex(int q, int r)
            : this(q, r, 1)
        {
        }

        public Hex(int q, int r, float size)
            : this(q, r, size, FormEnum.PointyTopped)
        {
        }

        public Hex(int q, int r, float size, FormEnum form)
        {
            Form = form;
            Logic = form switch
            {
                FormEnum.FlatTopped => new FlatTopped(),
                FormEnum.PointyTopped => new PointyTopped(),
                _ => throw new System.NotImplementedException(),
            };
            Q = q;
            R = r;
            Size = size;
        }

        public Hex(Vector2 point, float size, FormEnum form)
        {
            Form = form;
            Logic = form switch
            {
                FormEnum.FlatTopped => new FlatTopped(),
                FormEnum.PointyTopped => new PointyTopped(),
                _ => throw new System.NotImplementedException(),
            };
            Size = size;

            var v = Round(Logic.FromWorld(point) / size);
            Q = v.x;
            R = v.y;
        }

        public Hex(Vector3 position, float size, FormEnum form)
            : this(new Vector2(position.x, position.z), size, form)
        {
        }

        // 유니티에서 y축이 위쪽이므로 반시계 방향으로 리턴됨
        public Vector2 GetCorner(int index)
        {
            var deg = Logic.GetCornerAngleAsDegree(index);
            var rad = deg * Mathf.Deg2Rad;
            return new Vector2(Q + Size * Mathf.Cos(rad), R + Size * Mathf.Sin(rad));
        }

        public bool IsIn(Hex hex, int distance)
        {
            var v = this - hex;
            return Mathf.Abs(v.Item1 + v.Item2) <= distance;
        }

        public Vector2 ToWorld2D()
        {
            return Logic.ToWorld(Q, R) * Size;
        }

        public Vector3 ToWorld3D(float y = 0)
        {
            var v = ToWorld2D();
            return new Vector3(v.x, y, v.y);
        }
    }
}
