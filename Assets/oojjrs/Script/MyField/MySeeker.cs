using Assets.oojjrs.Script.MyRvo;
using System;
using UnityEngine;

namespace Assets.oojjrs.Script.MyField
{
    public class MySeeker : MonoBehaviour
    {
        private MyFlowField.NodeInterface CurrentNode { get; set; }
        private float CurrentNodeLastTimeOffset { get; set; }
        public float CurrentSpeed { get; private set; }
        public MyPath Path { get; private set; }
        public bool Processing { get; private set; }
        public bool ReachedEndOfPath => CurrentNode == default;
        // 몇 번까지 와리가리 헤맬 수 있는가.
        private int RvoCount { get; set; }
        private MyRvoDirectorInterface RvoDirector { get; set; }

        private event Func<float> GetTime;
        public event Action<string> OnDebug;

        private void Start()
        {
            RvoDirector = GetComponent<MyRvoDirectorInterface>();
        }

        private void Update()
        {
            if (Processing)
            {
                if ((Path != default) && (CurrentNode != default))
                {
                    var pos = transform.position;
                    if ((pos - Path.Destination).magnitude > float.Epsilon)
                    {
                        var isIn = CurrentNode.TileIntermediate.Tile.IsIn(pos);
                        if (isIn == false)
                        {
                            if (CurrentNode.NextNode != default)
                            {
                                isIn = CurrentNode.NextNode.TileIntermediate.Tile.IsIn(pos);
                                // 경로대로 잘 가고 있구만
                                if (isIn)
                                    CurrentNode = CurrentNode.NextNode;
                            }
                        }

                        if (isIn == false)
                        {
                            CurrentNode = Path.Field.GetNodeByPosition(pos);
                            isIn = CurrentNode != default;

                            Debug.Assert(isIn, "얘는 대체 어디에 서 있는 거야?");
                        }

                        if (isIn)
                        {
                            var distance = UpdateCurrentNodeLastTimeOffset(GetTime()) * CurrentSpeed;
                            var v = Vector3.ClampMagnitude((CurrentNode.TileIntermediate.Position - pos) + CurrentNode.Power ?? Path.Destination - pos, distance);
                            var vd = v;

                            if ((RvoDirector != default) && RvoDirector.Working)
                            {
                                // 긴급 피난일 때에는 목적지로 바로 가야지 쓸데 없는 짓 하면 안 된다.
                                if (CurrentNode.Reachable)
                                {
                                    if (CurrentNode.Target || (CurrentNode.NextNode?.Target == true))
                                        --RvoCount;

                                    v = RvoDirector.Modify(v, CurrentNodeLastTimeOffset);
                                }
                            }

#if UNITY_EDITOR
                            if ((RvoDirector != default) && RvoDirector.Working)
                            {
                                // 긴급 피난일 때에는 목적지로 바로 가야지 쓸데 없는 짓 하면 안 된다.
                                if (CurrentNode.Reachable)
                                {
                                    if (v != vd)
                                        OnDebug?.Invoke("RVO WORKING");
                                    else
                                        OnDebug?.Invoke("NO CORRECTION");
                                }
                                else
                                {
                                    OnDebug?.Invoke("EMERGENCY MOVING");
                                }
                            }
                            else
                            {
                                OnDebug?.Invoke("NO RVO");
                            }
#endif

                            if (RvoCount > 0)
                            {
                                transform.forward = v.normalized;
                                transform.position = pos + v;
                            }
                            else
                            {
                                StopMove();
                            }
                        }
                        else
                        {
                            StopMove();
                        }
                    }
                    else
                    {
                        StopMove();
                    }
                }
                else
                {
                    Debug.LogWarning($"{name}> 이게 어찌된 거지?");
                    StopMove();
                }
            }
        }

        public void Pause()
        {
            Processing = false;
        }

        public void Resume()
        {
            Processing = Path != default;
        }

        public void StartMove(MyPath path, float speed, bool startImmediately, Func<float> getTime)
        {
            if (getTime == default)
                getTime = () => Time.time;

            CurrentNode = path.FromNode;
            CurrentNodeLastTimeOffset = getTime();
            CurrentSpeed = speed;
            Path = path;
            Processing = startImmediately;
            RvoCount = 80;

            GetTime = getTime;
        }

        public void StopMove()
        {
            CurrentNode = default;
            CurrentNodeLastTimeOffset = 0;
            CurrentSpeed = 0;
            Path = default;
            Processing = false;
            RvoCount = 0;
        }

        private float UpdateCurrentNodeLastTimeOffset(float now)
        {
            var t = now - CurrentNodeLastTimeOffset;
            CurrentNodeLastTimeOffset = now;
            return t;
        }
    }
}
