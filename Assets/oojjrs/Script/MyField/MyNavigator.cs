using Assets.oojjrs.Script.MyRvo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.oojjrs.Script.MyField
{
    public class MyNavigator : MonoBehaviour
    {
        private Dictionary<Vector2Int, MyFlowField> Fields { get; } = new();
        public MyFlowField Latest { get; private set; }
        // TODO : 이 위치가 적합한가에 대해서는 생각의 여지가 있음.
        public MyRvoAgentContainer RvoAgentContainer { get; } = new();
        private Dictionary<Vector2Int, MyFlowField.TileIntermediate> Tiles { get; } = new();

        private event Func<Vector3, Vector2Int> PositionToCoordinate;
        public event Action<MyNavigator> OnUpdated;
        public event Action<MyNavigator> OnUsed;

        public void Calculate(Vector2Int to, Action<MyNavigator, MyFlowField> onFinish, Func<bool> keepGoingOn = default)
        {
            Debug.Assert(Tiles.Count > 0, "타일 정보가 없는데요? SetField부터 호출해주세요.");

            if (Fields.TryGetValue(to, out var field))
            {
                // TODO : 계산은 계산대로 하고, 리턴은 바로 할 수 있습니다..? 건설이 없으니까 가능하긴 한데 그게 좀 그렇지 않나.
                if (field.Calculating)
                {
                    _ = StartCoroutine(Func());
                }
                else
                {
                    Latest = field;
                    onFinish?.Invoke(this, field);
                }

                IEnumerator Func()
                {
                    if (keepGoingOn == default)
                        keepGoingOn = () => true;

                    yield return new WaitUntil(() => (field.Calculating == false) || (keepGoingOn() == false));

                    if (keepGoingOn())
                    {
                        Latest = field;
                        onFinish?.Invoke(this, field);
                    }
                }
            }
            else
            {
                field = new(Tiles.Values.ToArray(), to, PositionToCoordinate);
                Fields[to] = field;

                _ = StartCoroutine(field.CalculateAsync(() =>
                {
                    Latest = field;
                    onFinish?.Invoke(this, field);
                }, keepGoingOn));
            }
        }

        public void Search(Vector3 src, Vector3 dst, Vector2Int from, Vector2Int to, Action<MyPath> onFinish, Func<bool> keepGoingOn = default)
        {
            Debug.Assert(Tiles.Count > 0, "타일 정보가 없는데요? SetField부터 호출해주세요.");

            if (Tiles.TryGetValue(from, out var fromTile) && Tiles.TryGetValue(to, out var toTile))
            {
                var fromOk = fromTile.Tile.Walkable;
                var toOk = toTile.Tile.Walkable;

                if (fromOk)
                {
                    if (toOk)
                    {
                        // 둘 다 유효한 칸이지만 서로 간에 길이 끊겨있을 수는 있어.
                        Calculate(to, (nav, ff) =>
                        {
                            var path = ff.GetPath(from, src, dst);
                            onFinish?.Invoke(path);

                            OnUsed?.Invoke(this);
                        }, keepGoingOn);
                    }
                    // 도착지 근처를 찾아서 보내주는 것도 일인데...
                    else
                    {
                        _ = StartCoroutine(SearchAroundTiles(src, dst, fromTile, toTile, onFinish, keepGoingOn));
                    }
                }
                // 출발지가 망가진 경우인데, 이 경우는 뭐 다른 곳으로 갈 수가 없고 그냥 출발지에서 긴급 탈출이 필요한 케이스다.
                else
                {
                    MyFlowField.TileIntermediate GetClosestTile()
                    {
                        var useds = new HashSet<MyFlowField.TileIntermediate>();
                        useds.Add(fromTile);

                        var inspectingTiles = new List<MyFlowField.TileIntermediate>();
                        inspectingTiles.Add(fromTile);

                        while (inspectingTiles.Count > 0)
                        {
                            // 최적화 때문에 except 안 쓴 거니까 바꾸지 마라.
                            var neighborTiles = inspectingTiles.SelectMany(tile => tile.Neighbors.Select(coordinate => Tiles.TryGetValue(coordinate, out var tile) ? tile : default)).Distinct().Where(tile => useds.Contains(tile) == false).ToArray();
                            inspectingTiles.Clear();

                            var closestReachable = neighborTiles.Where(t => t.Tile.Walkable).OrderBy(tile => (tile.Position - src).sqrMagnitude).FirstOrDefault();
                            if (closestReachable != default)
                            {
                                return closestReachable;
                            }
                            else
                            {
                                inspectingTiles.AddRange(neighborTiles);
                                foreach (var tile in neighborTiles)
                                    useds.Add(tile);
                            }
                        }

                        return default;
                    }

                    var closestTile = GetClosestTile();
                    if (closestTile != default)
                    {
                        Calculate(closestTile.Coordinate, (nav, ff) =>
                        {
                            var path = ff.GetPath(closestTile.Coordinate, src, closestTile.Position);
                            onFinish?.Invoke(path);

                            OnUsed?.Invoke(this);
                        }, keepGoingOn);
                    }
                    else
                    {
                        // 맵에 뭐 갈 수 있는 곳이 하나도 없다는데...
                        onFinish?.Invoke(default);
                    }
                }
            }
            // 너 뭐야 어떻게 이런 요청을 할 수 있었나
            else
            {
                onFinish?.Invoke(default);
            }
        }

        private IEnumerator SearchAroundTiles(Vector3 src, Vector3 dst, MyFlowField.TileIntermediate fromTile, MyFlowField.TileIntermediate toTile, Action<MyPath> onFinish, Func<bool> keepGoingOn = default)
        {
            var groups = toTile.Tile.AroundCoordinates.Select(c => Tiles.TryGetValue(c, out var tile) ? tile : default).Where(t => (t != default) && t.Tile.Walkable).GroupBy(t => t.Tile.GetCost(toTile.Tile)).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                foreach (var aroundTile in group.OrderBy(t => (t.Position - dst).sqrMagnitude).ThenBy(t => (t.Position - src).sqrMagnitude))
                {
                    var b = false;
                    var ret = false;
                    Calculate(aroundTile.Coordinate, (nav, field) =>
                    {
                        var path = field.GetPath(fromTile.Coordinate, src, aroundTile.Position + (toTile.Position - aroundTile.Position).normalized * aroundTile.Tile.Length);
                        if (path != default)
                        {
                            b = true;

                            onFinish?.Invoke(path);

                            OnUsed?.Invoke(this);
                        }

                        ret = true;
                    }, keepGoingOn);

                    yield return new WaitUntil(() => ret);

                    if (b)
                        yield break;
                }
            }

            onFinish?.Invoke(default);

            OnUsed?.Invoke(this);
        }

        public void SetField(MyFlowField.TileInterface[] tiles, Func<Vector3, Vector2Int> positionToCoordinate)
        {
            Fields.Clear();
            Tiles.Clear();

            foreach (var tile in tiles)
                Tiles.Add(tile.Coordinate, new(tile));

            PositionToCoordinate = positionToCoordinate;
        }

        public void UpdateFields(Action<MyNavigator, MyFlowField> onFinish, Func<bool> keepGoingOn = default)
        {
            Debug.Assert(Tiles.Count > 0, "타일 정보가 없는데요? SetField부터 호출해주세요.");

            foreach (var field in Fields.Values)
            {
                _ = StartCoroutine(field.RecalculateAsync(() =>
                {
                    onFinish?.Invoke(this, field);

                    if ((Latest != default) && (Latest == field))
                        OnUpdated?.Invoke(this);
                }, keepGoingOn));
            }
        }
    }
}
