using System.Collections.Generic;
using UnityEngine;

namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        public class FlatToppedUtility
        {
            private Hex Hex { get; }

            public IEnumerable<Hex> TrapezoidLeft
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(0, -1);
                    yield return Hex + new Vector2Int(0, 1);
                    yield return Hex + new Vector2Int(1, 0);
                    yield return Hex + new Vector2Int(1, -1);
                }
            }
            public IEnumerable<Hex> TrapezoidRight
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(-1, 0);
                    yield return Hex + new Vector2Int(-1, 1);
                    yield return Hex + new Vector2Int(0, -1);
                    yield return Hex + new Vector2Int(0, 1);
                }
            }
            public IEnumerable<Hex> TrianglesLeft
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(-1, 0);
                    yield return Hex + new Vector2Int(-1, 1);
                }
            }
            public IEnumerable<Hex> TrianglesRight
            {
                get
                {
                    yield return Hex;
                    yield return Hex + new Vector2Int(1, 0);
                    yield return Hex + new Vector2Int(1, -1);
                }
            }

            internal FlatToppedUtility(Hex hex)
            {
                Hex = hex;
            }
        }

        public FlatToppedUtility AsFlatTopped => new(this);
    }
}
