using Assets.oojjrs.Script;
using Assets.oojjrs.Script.MyField;
using Assets.Sources.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FlowField
{
    public class Plane : MonoBehaviour
    {
        [SerializeField]
        private Tile _tilePrefab;

        private void Start()
        {
            var ret = new List<Tile>();
            foreach (var hex in Hex.GetRange(new(0, 0, 1.5f, Hex.FormEnum.PointyTopped), 10))
            {
                var tile = Instantiate(_tilePrefab);
                tile.name = $"({hex.Q}, {hex.R})";
                tile.transform.SetParent(transform);
                tile.transform.position = hex.ToWorld3D();
                tile.Hex = hex;
                tile.Walkable = MyRandom.Range(0f, 1f) > 0.5f;
                if (hex.Q == 0 && hex.R == 0)
                    tile.Walkable = true;

                ret.Add(tile);
            }

            MyNavigator.Instance.SetField(ret.ToArray());

            MyNavigator.Instance.Calculate(Vector2Int.zero, (nav, field) =>
            {
                foreach (var tile in ret)
                    tile.UpdateDirection(field.GetNode(((MyFlowField.TileInterface)tile).Coordinate).Direction);
            });
        }
    }
}
