﻿using Assets.oojjrs.Script.MyField;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FlowField
{
    public class Tile : MonoBehaviour, MyFlowField.TileInterface
    {
        [SerializeField]
        private GameObject _arrow;

        Vector2Int MyFlowField.TileInterface.Coordinate => new((int)transform.position.x, (int)transform.position.z);
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
        Vector2 MyFlowField.TileInterface.Position => new(transform.position.x, transform.position.z);
        bool MyFlowField.TileInterface.Walkable => true;

        public void UpdateDirection(Vector2? dir)
        {
            if (dir.HasValue)
                _arrow.transform.forward = new(dir.Value.x, 0, dir.Value.y);
            else
                _arrow.SetActive(false);
        }
    }
}
