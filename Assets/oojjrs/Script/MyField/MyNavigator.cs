using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.oojjrs.Script.MyField
{
    public class MyNavigator : MonoBehaviour
    {
        public static MyNavigator Instance { get; private set; }

        private Dictionary<Vector2Int, MyFlowField> Fields { get; } = new();
        private MyFlowField.TileInterface[] Tiles { get; set; }

        private void Awake()
        {
            if (Instance == default)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = default;
        }

        public void Calculate(Vector2Int to, Action<MyNavigator, MyFlowField> onFinish, Func<bool> keepGoingOn = default)
        {
            if (Fields.TryGetValue(to, out var field))
            {
                onFinish?.Invoke(this, field);
            }
            else
            {
                field = new(Tiles, to);
                Fields[to] = field;

                StartCoroutine(field.CalculateAsync(() => onFinish?.Invoke(this, field), keepGoingOn));
            }
        }

        public void Search(Vector2Int from, Action<MyPath> onFinish)
        {
        }

        public void SetField(MyFlowField.TileInterface[] tiles)
        {
            Tiles = tiles;
        }
    }
}
