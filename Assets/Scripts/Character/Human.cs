using UnityEngine;

namespace Character
{
    // ABSTRACTION - Humain Joueur comme PNJ
    public abstract class Human : MonoBehaviour
    {
        public enum State
        {
            Walking,
            Busy,
            Waiting
        }
        
        private const int Down = 0;
        private const int Up = 1;
        private const int Left = 2;
        private const int Right = 3;
        private const int None = -1;
        
        protected static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        protected static readonly int AnimatorDirection = Animator.StringToHash("direction");
        protected static readonly int IsJumping = Animator.StringToHash("isJumping");
        
        protected virtual float MoveSpeed => 3.5f;
        protected int currentDirection;
        protected Vector2 movement;
        protected Animator animator;
        protected State currentState;

        protected void Start()
        {
            currentDirection = Down;
            animator = GetComponent<Animator>();
            movement = Vector2.zero;
            currentState = State.Waiting;
        }
        
        protected int GetDirection(string direction)
        {
            return direction.ToLower() switch
            {
                "up" => Up,
                "left" => Left,
                "right" => Right,
                _ => Down
            };
        }
        
        protected int GetDirection(KeyCode key)
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
        
        protected int GetDirection(Vector3 target)
        {
            var delta = target - transform.position;
            int direction;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                direction = delta.x > 0 ? Right : Left;
            else
                direction = delta.y > 0 ? Up : Down;

            return direction;
        }
        
        protected Vector2 ConvertDirectionToVector(int dir)
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
        
        // POLYMORPHISM - Méthode de gestion du mouvement physique
        protected void Move()
        {
            transform.Translate(movement * (MoveSpeed * Time.fixedDeltaTime));
        }

        protected void Walk()
        {
            animator.SetInteger(AnimatorDirection, currentDirection);
            animator.SetFloat(AnimatorSpeed, 1f);
            
            currentState = State.Walking;
            movement = ConvertDirectionToVector(currentDirection);
        }
        
        protected void Stop()
        {
            animator.SetFloat(AnimatorSpeed, 0f);
            
            currentState = State.Waiting;
            movement = Vector2.zero;
        }

        protected void Spawn(Vector2 position)
        {
            animator.SetInteger(AnimatorDirection, currentDirection);
            transform.position = position;
        }
    }
}
