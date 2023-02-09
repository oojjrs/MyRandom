using UnityEngine;

namespace Assets.oojjrs.Script.MyRvo
{
    public class MyRvoAgent : MonoBehaviour, MyRvoAgentInterface
    {
        MyRvoAgentContainer MyRvoAgentInterface.Container { get; set; }
        Vector3 MyRvoObstacleInterface.Position => Position;
        float MyRvoObstacleInterface.Radius => Radius;

        public Vector3 Forward => Velocity.normalized;
        private Vector3 Position => transform.position;
        private Vector3 PreviousPosition { get; set; }
        public float Radius { get; set; }
        public Vector3 Velocity { get; set; }

        private void Start()
        {
            PreviousPosition = transform.position;
        }

        private void Update()
        {
            if (Position != PreviousPosition)
            {
                Velocity = Position - PreviousPosition;
                PreviousPosition = Position;
            }
            else
            {
                Velocity = Vector3.zero;
            }
        }

        bool MyRvoObstacleInterface.IsCollidedInXZ(Vector3 pos)
        {
            var a = new Vector2(transform.position.x, transform.position.z);
            var b = new Vector2(pos.x, pos.z);
            return Vector2.Distance(a, b) <= Radius;
        }
    }
}
