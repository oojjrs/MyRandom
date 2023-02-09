using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public class MyRvoAgentContainer
    {
        private List<MyRvoObstacleInterface> Values { get; } = new();

        public void Add(MyRvoAgentInterface agent)
        {
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
            Values.Clear();
        }

        public MyRvoObstacleInterface GetObstacle(Vector3 pos, MyRvoAgentInterface me)
        {
            return Values.FirstOrDefault(t => (t != me) && t.IsCollidedInXZ(pos));
        }

        public void Remove(MyRvoAgentInterface agent)
        {
            if (Values.Remove(agent))
                agent.Container = default;
        }
    }
}
