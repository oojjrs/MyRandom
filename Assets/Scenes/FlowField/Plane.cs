using Assets.oojjrs.Script.MyField;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FlowField
{
    public class Plane : MonoBehaviour
    {
        [SerializeField]
        private int _height = 10;
        [SerializeField]
        private Tile _tilePrefab;
        [SerializeField]
        private int _width = 10;

        private void Start()
        {
            var ret = new List<Tile>();
            for (int x = -_width / 2; x < _width / 2; ++x)
            {
                for (int y = -_height / 2; y < _height / 2; ++y)
                {
                    var tile = Instantiate(_tilePrefab);
                    tile.name = $"({x + _width / 2}, {y + _height / 2})";
                    tile.transform.SetParent(transform);
                    tile.transform.position = new(x, 0, y);

                    ret.Add(tile);
                }
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
