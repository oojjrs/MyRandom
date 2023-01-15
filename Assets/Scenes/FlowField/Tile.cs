using Assets.oojjrs.Script.MyField;
using Assets.Sources.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FlowField
{
    public class Tile : MonoBehaviour, MyFlowField.TileInterface
    {
        [SerializeField]
        private GameObject _arrow;
        [SerializeField]
        private GameObject _obstacle;

        Vector2Int MyFlowField.TileInterface.Coordinate => new(Hex.Q, Hex.R);
        IEnumerable<Vector2Int> MyFlowField.TileInterface.Neighbors
        {
            get
            {
                foreach (var hex in Hex.Neighbors)
                    yield return (Vector2Int)hex;
            }
        }
        Vector2 MyFlowField.TileInterface.Position => new(transform.position.x, transform.position.z);
        bool MyFlowField.TileInterface.Walkable => Walkable;

        public Hex Hex { get; set; }
        public bool Walkable { get; set; }

        private void Start()
        {
            _arrow.SetActive(Walkable);
            _obstacle.SetActive(Walkable == false);
        }

        float MyFlowField.TileInterface.GetCost(MyFlowField.TileInterface toTile)
        {
            return (((MyFlowField.TileInterface)this).Position - toTile.Position).magnitude;
        }

        public void UpdateDirection(Vector2? dir)
        {
            if (Walkable)
            {
                if (dir.HasValue)
                    _arrow.transform.forward = new(dir.Value.x, 0, dir.Value.y);
                else
                    _arrow.SetActive(false);
            }
            else
            {
                _arrow.SetActive(false);
            }
        }
    }
}
