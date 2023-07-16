using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public class MyRvoAgentContainer
    {
        private Dictionary<MyRvoAgentInterface, MyRvoObstacleInterface> ObstacleCached { get; } = new();
        private List<MyRvoObstacleInterface> Values { get; } = new();

        public void Add(MyRvoAgentInterface agent)
        {
            if (agent.Alive == false)
                return;

            if (Values.Contains(agent) == false)
            {
                agent.Container = this;
                Values.Add(agent);
            }
        }

        public void AddRange(IEnumerable<MyRvoObstacleInterface> obstacles)
        {
            Values.AddRange(obstacles.Except(Values));
        }

        public void Clear()
        {
            ObstacleCached.Clear();
            Values.Clear();
        }

        public MyRvoObstacleInterface GetObstacle(Vector3 pos, MyRvoAgentInterface me)
        {
            if (ObstacleCached.TryGetValue(me, out var value))
            {
                if (value.Alive && value.IsCollidedInXZ(pos))
                    return value;
            }

            value = Values.FirstOrDefault(t => (t != me) && t.IsCollidedInXZ(pos));
            if (value != default)
                ObstacleCached[me] = value;

            return value;
        }

        public void Remove(MyRvoAgentInterface agent)
        {
            if (Values.Remove(agent))
            {
                ObstacleCached.Remove(agent);

                agent.Container = default;
            }
        }
    }
}
