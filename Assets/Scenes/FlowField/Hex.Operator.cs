using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        public static explicit operator (int, int)(Hex hex) => new(hex.Q, hex.R);

        public static explicit operator Vector2(Hex hex) => new(hex.Q, hex.R);

        public static explicit operator Vector2Int(Hex hex) => new(hex.Q, hex.R);

        public static explicit operator Hex(Vector2Int v) => new(v.x, v.y);

        public static Hex operator +(Hex lhs, (int q, int r) rhs) => new(lhs.Q + rhs.q, lhs.R + rhs.r, lhs.Size, lhs.Form);

        public static Hex operator +(Hex lhs, Vector2Int rhs) => new(lhs.Q + rhs.x, lhs.R + rhs.y, lhs.Size, lhs.Form);

        public static Hex operator +(Hex lhs, Hex rhs) => new(lhs.Q + rhs.Q, lhs.R + rhs.R, lhs.Size, lhs.Form);

        public static Hex operator -(Hex lhs, (int q, int r) rhs) => new(lhs.Q - rhs.q, lhs.R - rhs.r, lhs.Size, lhs.Form);

        public static Hex operator -(Hex lhs, Vector2Int rhs) => new(lhs.Q - rhs.x, lhs.R - rhs.y, lhs.Size, lhs.Form);

        public static (int, int) operator -(Hex lhs, Hex rhs) => new(lhs.Q - rhs.Q, lhs.R - rhs.R);

        public static int GetDistance(Hex lhs, Hex rhs) => (Mathf.Abs(lhs.Q - rhs.Q) + Mathf.Abs(lhs.R - rhs.R) + Mathf.Abs(lhs.S - rhs.S)) / 2;

        public static IEnumerable<Hex> GetBetween(Hex lhs, Hex rhs)
        {
            var n = GetDistance(lhs, rhs);
            return Enumerable.Range(0, n + 1).Select(i =>
            {
                // Epsilon을 더해주어 경계선에 정확히 떨어지는 값들을 방지할 수 있다고 하는군.
                var v = Round(Vector2.Lerp((Vector2)lhs, (Vector2)rhs + new Vector2(float.Epsilon, float.Epsilon), 1f / n * i));
                return new Hex(v.x, v.y, lhs.Size, lhs.Form);
            });
        }

        public static IEnumerable<Hex> GetRange(Hex center, int maxDistance)
        {
            return GetRange(center, 0, maxDistance);
        }

        public static IEnumerable<Hex> GetRange(Hex center, int minDistance, int maxDistance)
        {
            if ((maxDistance >= 0) && (minDistance <= maxDistance))
            {
                for (int q = -maxDistance; q <= maxDistance; ++q)
                {
                    for (int r = Math.Max(-maxDistance, -q - maxDistance); r <= Math.Min(maxDistance, -q + maxDistance); ++r)
                    {
                        var v = new Vector2Int(q, r);
                        if (Vector2Int.Distance(Vector2Int.zero, v) >= minDistance)
                            yield return center + v;
                    }
                }
            }
        }

        // 통상적인 방향과 반대다. z좌표가 반대니까.
        public static Hex RotateLeft(Hex hex)
        {
            return new(-hex.R, -hex.S);
        }

        // 통상적인 방향과 반대다. z좌표가 반대니까.
        public static Hex RotateRight(Hex hex)
        {
            return new(-hex.S, -hex.Q);
        }

        private static Vector2Int Round(Vector2 v)
        {
            var xgrid = Mathf.RoundToInt(v.x);
            var ygrid = Mathf.RoundToInt(v.y);
            v.x -= xgrid;
            v.y -= ygrid;

            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
                return new(xgrid + Mathf.RoundToInt(v.x + v.y * 0.5f), ygrid);
            else
                return new(xgrid, ygrid + Mathf.RoundToInt(v.x * 0.5f + v.y));
        }
    }
}
