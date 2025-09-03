using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class NPC : Human
    {
        public GameObject way;
        protected override float MoveSpeed => 2f;
        
        private const float MinDistanceFromTarget = 0.05f;
        private const float MinStopDuration = 1f;
        private const float MaxStopDuration = 4f;
        
        private Rigidbody2D _rb;
        private List<Vector2> _wayPoints;
        private int _currentIndex;
        private Vector2 _target;
        private bool _playerInteraction;
        private int _initialDirection;

        private bool IsStatic => way == null;
        
        private new void Start()
        {
            base.Start();
            
            _rb = GetComponent<Rigidbody2D>();
            _wayPoints = new List<Vector2>();
            _initialDirection = animator.GetInteger(AnimatorDirection);
            
            if (!IsStatic)
            {
                // Récupération de la position des points de passage.
                foreach (Transform child in way.transform)
                    _wayPoints.Add(child.position);
                
                // Choix du point d'entrée 
                ChooseRandomStartPoint();
                NextWaypoint();
                Walk();
            }
        }

        private void FixedUpdate()
        {
            if (currentState != State.Walking) return;
            
            Move();

            var distanceFromTarget = Vector2.Distance(transform.position, _target);

            if (distanceFromTarget < MinDistanceFromTarget)
            {
                StartCoroutine(Wait());
                NextWaypoint();
            }
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

            if (IsStatic) StartCoroutine(ReturnToInitialDirection());
            else StartCoroutine(Wait(MinStopDuration));
        }
        
        private IEnumerator Wait(float delay = 0)
        {
            var t = delay <= 0 ? Random.Range(MinStopDuration, MaxStopDuration + 1) : delay;
        
            Stop();
            yield return new WaitForSeconds(t);

            if (currentState != State.Busy)
                Walk();
        }

        private void Talk()
        {
            currentState = State.Busy;
            animator.SetFloat(AnimatorSpeed, 0f);
        }
        
        private void NextWaypoint()
        {
            if (_currentIndex == _wayPoints.Count - 1) _currentIndex = 0;
            else _currentIndex++;
            
            _target = _wayPoints[_currentIndex];
            currentDirection = GetDirection(_target);
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
            
            currentState = State.Waiting;
            animator.SetInteger(AnimatorDirection, _initialDirection);
        }
        
        private void LookAt(GameObject target)
        {
            var nextDirection = GetDirection(target.transform.position);
            
            animator.SetInteger(AnimatorDirection, nextDirection);
        }
    }
}