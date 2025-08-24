using UnityEngine;

namespace Character
{
    public class Human : MonoBehaviour
    {
        private const int Down = 0;
        private const int Up = 1;
        private const int Left = 2;
        private const int Right = 3;
        private const int None = -1;
        
        protected static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        protected static readonly int AnimatorDirection = Animator.StringToHash("direction");
        protected static readonly int IsJumping = Animator.StringToHash("isJumping");
        
        protected virtual float MoveSpeed { get; set; } = 3.5f;
        protected Animator animator;
        
        
        public int GetDirection(string direction)
        {
            return direction.ToLower() switch
            {
                "up" => Up,
                "left" => Left,
                "right" => Right,
                _ => Down
            };
        }
        
        public int GetDirection(KeyCode key)
        {
            return key switch
            {
                KeyCode.DownArrow => Down,
                KeyCode.UpArrow => Up,
                KeyCode.LeftArrow => Left,
                KeyCode.RightArrow => Right,
                _ => None
            };
        }
        
        public int GetDirection(Vector3 target)
        {
            var delta = target - transform.position;
            int direction;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                direction = delta.x > 0 ? Right : Left;
            else
                direction = delta.y > 0 ? Up : Down;

            return direction;
        }
        
        public Vector2 ConvertDirectionToVector(int dir)
        {
            return dir switch
            {
                Down => Vector2.down,
                Up => Vector2.up,
                Left => Vector2.left,
                Right => Vector2.right,
                _ => Vector2.zero
            };
        }
    }
}
