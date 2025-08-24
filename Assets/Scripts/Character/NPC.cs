using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class NPC : Human
    {
        public GameObject way;
        public bool isStatic;
        protected override float MoveSpeed => 2f;
        
        private enum State
        {
            Walking,
            Talking,
            Waiting
        }
        
        private const float MinDistanceFromTarget = 0.05f;
        private const float MinStopDuration = 1f;
        private const float MaxStopDuration = 4f;
        
        private Rigidbody2D _rb;
        private List<Vector2> _wayPoints;
        private int _currentIndex;
        private Vector2 _target;
        private bool _playerInteraction;
        private State _currentState;
        private int _initialDirection;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            _wayPoints = new List<Vector2>();
            _initialDirection = animator.GetInteger(AnimatorDirection);
            
            if (!isStatic)
            {
                foreach (Transform child in way.transform)
                    _wayPoints.Add(child.position);

                ChooseRandomStartPoint();
                _currentState = State.Walking;
            }
            else
            {
                _currentState = State.Waiting;
            }
        }

        private void FixedUpdate()
        {
            if (_currentState != State.Walking) return;
            
            Move();

            var distanceFromTarget = Vector2.Distance(transform.position, _target);

            if (!(distanceFromTarget < MinDistanceFromTarget)) return;
            
            StartCoroutine(Wait());
            NextWaypoint();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
        
            Talk();
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
        
            LookAt(other.gameObject);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
        
            StopAllCoroutines();

            if (isStatic) StartCoroutine(ReturnToInitialDirection());
            else StartCoroutine(Wait(MinStopDuration));
        }

        protected override void Move()
        {
            var movement = Vector2.MoveTowards(_rb.position, _target, MoveSpeed * Time.deltaTime);
            _rb.MovePosition(movement);
        }
        
        private void Stop()
        {
            _currentState = State.Waiting;
            animator.SetFloat(AnimatorSpeed, 0f);
        }

        private IEnumerator Wait(float delay = 0)
        {
            var t = delay <= 0 ? Random.Range(MinStopDuration, MaxStopDuration + 1) : delay;
        
            Stop();
            yield return new WaitForSeconds(t);

            if (_currentState != State.Talking)
                Walk();
        }

        private void Talk()
        {
            _currentState = State.Talking;
            animator.SetFloat(AnimatorSpeed, 0f);
        }

        private void Walk()
        {
            _currentState = State.Walking;
            animator.SetFloat(AnimatorSpeed, 1f);
            
            var newDirection = GetDirection(_target);
            animator.SetInteger(AnimatorDirection, newDirection);
        }

        private void NextWaypoint()
        {
            if (_currentIndex == _wayPoints.Count - 1)
                _currentIndex = 0;
            else
                _currentIndex++;
            _target = _wayPoints[_currentIndex];
        }
        
        private void ChooseRandomStartPoint()
        {
            _currentIndex = Random.Range(0, _wayPoints.Count);
            _target = _wayPoints[_currentIndex];
            transform.position = _target;
        }
        
        private IEnumerator ReturnToInitialDirection()
        {
            yield return new WaitForSeconds(MinStopDuration);
            
            _currentState = State.Waiting;
            animator.SetInteger(AnimatorDirection, _initialDirection);
        }
        
        private void LookAt(GameObject target)
        {
            var nextDirection = GetDirection(target.transform.position);
            
            animator.SetInteger(AnimatorDirection, nextDirection);
        }
    }
}