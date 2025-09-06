using System.Collections;
using UnityEngine;

namespace Character
{
    // ABSTRACTION - Humain Joueur comme PNJ
    public abstract class Human : MonoBehaviour
    {
        public GameObject shadow;
        
        private const int Down = 0;
        private const int Up = 1;
        private const int Left = 2;
        private const int Right = 3;
        private const int None = -1;
        private static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorDirection = Animator.StringToHash("direction");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private Animator _animator;
        private SpriteRenderer _shadowSprite;
        private Collider2D _collider;
        
        protected enum State
        {
            Walking,
            Busy,
            Waiting
        }
        protected virtual float MoveSpeed => 3.5f;
        protected int currentDirection;
        protected State currentState;
        
        protected void Start()
        {
            _animator = GetComponent<Animator>();
            currentDirection = _animator.GetInteger(AnimatorDirection);
            currentState = State.Waiting;
            
            _shadowSprite = shadow.GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
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

        private Vector2 ConvertDirectionToVector(int dir)
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
            var movement = ConvertDirectionToVector(currentDirection);
            transform.Translate(movement * (MoveSpeed * Time.fixedDeltaTime));
        }

        protected void Walk()
        {
            _animator.SetFloat(AnimatorSpeed, 1f);
            currentState = State.Walking;
        }
        
        public void Talk()
        {
            _animator.SetFloat(AnimatorSpeed, 0f);
            currentState = State.Busy;
        }
        
        public void Stop()
        {
            _animator.SetFloat(AnimatorSpeed, 0f);
            currentState = State.Waiting;
        }

        protected void Spawn(Vector2 position)
        {
            _animator.SetInteger(AnimatorDirection, currentDirection);
            transform.position = position;
        }

        private void Jump()
        {
            _collider.enabled = false;
            _shadowSprite.enabled = true;
            _animator.SetBool(IsJumping, true);
            currentState = State.Busy;
        }

        private void Land()
        {
            _collider.enabled = true;
            _shadowSprite.enabled = false;
            _animator.SetBool(IsJumping, false);
            
            Stop();
        }
        
        protected void UpdateDirection(int direction)
        {
            currentDirection = direction;
            _animator.SetInteger(AnimatorDirection, currentDirection);
        }
        
        public IEnumerator JumpOver(float distance, float duration)
        {
            var direction = ConvertDirectionToVector(_animator.GetInteger(AnimatorDirection));
            var baseScale = shadow.transform.localScale;
            var minScale = Vector3.one * 0.2f;
            var start = (Vector2)transform.position;
            var end = start + direction * distance;
            var endShadow = start + direction * (distance + 0.6f);
            var elapsed = 0f;
            
            Jump();
            
            while (elapsed < duration)
            {
                var timeSpent = elapsed / duration;
                
                shadow.transform.localScale = Vector3.Lerp(minScale, baseScale, timeSpent);
                transform.position = Vector2.Lerp(start, end, timeSpent);
                shadow.transform.position = endShadow;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = end;
            shadow.transform.localScale = baseScale;
            
            Land();
        }
    }
}
