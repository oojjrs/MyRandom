using Assets.oojjrs.Script.MyField;
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

        IEnumerable<Vector2Int> MyFlowField.TileInterface.AroundCoordinates => throw new System.NotImplementedException();
        Vector2Int MyFlowField.TileInterface.Coordinate => new((int)transform.position.x, (int)transform.position.z);
        float MyFlowField.TileInterface.Length => throw new System.NotImplementedException();
        IEnumerable<Vector2Int> MyFlowField.TileInterface.Neighbors
        {
            get
            {
                var c = ((MyFlowField.TileInterface)this).Coordinate;
                yield return new(c.x - 1, c.y - 1);
                yield return new(c.x - 1, c.y);
                yield return new(c.x - 1, c.y + 1);
                yield return new(c.x, c.y - 1);
                yield return new(c.x, c.y + 1);
                yield return new(c.x + 1, c.y - 1);
                yield return new(c.x + 1, c.y);
                yield return new(c.x + 1, c.y + 1);
            }
        }
        Vector3 MyFlowField.TileInterface.Position => transform.position;
        bool MyFlowField.TileInterface.Walkable => Walkable;

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

        bool MyFlowField.TileInterface.IsIn(Vector3 pos)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateDirection(Vector3? power)
        {
            if (Walkable)
            {
                if (power.HasValue)
                    _arrow.transform.forward = power.Value.normalized;
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
