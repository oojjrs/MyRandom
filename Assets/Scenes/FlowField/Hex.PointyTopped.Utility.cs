using System.Collections.Generic;
using UnityEngine;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        public class PointyToppedUtility
        {
            private Hex Hex { get; }

            internal PointyToppedUtility(Hex hex)
            {
                Hex = hex;
            }

            public IEnumerable<Hex> TrapezoidBottom
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(-1, 0);
                    yield return Hex + new Vector2Int(-1, 1);
                    yield return Hex + new Vector2Int(0, 1);
                    yield return Hex + new Vector2Int(1, 0);
                }
            }
            public IEnumerable<Hex> TrapezoidTop
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(-1, 0);
                    yield return Hex + new Vector2Int(0, -1);
                    yield return Hex + new Vector2Int(1, -1);
                    yield return Hex + new Vector2Int(1, 0);
                }
            }
            public IEnumerable<Hex> TrianglesBottom
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(0, -1);
                    yield return Hex + new Vector2Int(1, -1);
                }
            }
            public IEnumerable<Hex> TrianglesTop
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(-1, 1);
                    yield return Hex + new Vector2Int(0, 1);
                }
            }
        }

        public PointyToppedUtility AsPointyTopped => new(this);
    }
}
