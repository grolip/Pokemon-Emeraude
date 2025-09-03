using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character
{
    // INHERITANCE - Humain
    public class PlayerController : Human
    {
        // ENCAPSULATION - Instance du joueur
        public static PlayerController Instance { get; private set; }
        
        public string nextSpawnID;
        public GameObject shadow;
        
        private SpriteRenderer _shadowSprite;
        private SpriteRenderer _sprite;
        private Collider2D _collider;
        private readonly List<KeyCode> _activeKeys = new ();
        private readonly List<KeyCode> _allKeys = new ();
        
        private void Awake()
        {
            if (FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private new void Start()
        {
            base.Start();
            
            _shadowSprite = shadow.GetComponent<SpriteRenderer>();
            _sprite = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _allKeys.AddRange(new [] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow });
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void Update()
        {
            CheckKeyPress();
            
            // Si au moins une touche enfoncée
            if (_activeKeys.Count > 0)
            {
                currentDirection = GetDirection(_activeKeys[^1]);
                Walk();
            }
            else
            {
                Stop();
            }
        }

        private void FixedUpdate()
        {
            if (currentState == State.Busy) return;
            Move();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindSpawnPoint();
            Appear();
        }
        
        private void CheckKeyPress()
        {
            foreach (var key in _allKeys)
            {
                // Gérer x touches directionnelles enfoncée en même temps.
                if (Input.GetKeyDown(key) && !_activeKeys.Contains(key))
                    _activeKeys.Add(key);

                // Supprimer de la pile les touches relachées.
                if (Input.GetKeyUp(key) && _activeKeys.Contains(key))
                    _activeKeys.Remove(key);
            }
        }
        
        public void FindSpawnPoint()
        {
            var spawnPoints = FindObjectsByType<SpawnPointData>(FindObjectsSortMode.None);
        
            if (spawnPoints.Length == 0) return;
        
            var playerSpawnPoint = spawnPoints[0];
        
            if (nextSpawnID.Length != 0)
            {
                foreach (var sp in spawnPoints)
                {
                    if (sp.spawnID == nextSpawnID)
                    {
                        playerSpawnPoint = sp;
                        break;
                    }
                }
            }
            
            var direction = GetDirection(playerSpawnPoint.playerOrientation);
            animator.SetInteger(AnimatorDirection, direction);
            transform.position = playerSpawnPoint.transform.position;
        }
    
        public void Appear()
        {
            _sprite.enabled = true;
            currentState = State.Waiting;
        }
    
        public void Disappear()
        {
            _sprite.enabled = false;
            currentState = State.Busy;
        }
        
        public IEnumerator JumpOver(float distance, float duration)
        {
            var direction = ConvertDirectionToVector(animator.GetInteger(AnimatorDirection));
            var baseScale = shadow.transform.localScale;
            var minScale = Vector3.one * 0.2f;
            var start = (Vector2)transform.position;
            var end = start + direction * distance;
            var endShadow = start + direction * (distance + 0.6f);
            var elapsed = 0f;
            
            _collider.enabled = false;
            _shadowSprite.enabled = true;
            animator.SetBool(IsJumping, true);
            currentState = State.Busy;
            
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
            
            _collider.enabled = true;
            _shadowSprite.enabled = false;
            animator.SetBool(IsJumping, false);
            currentState = State.Waiting;
        }

        public bool HaveSameDirection(string direction)
        {
            return currentDirection == GetDirection(direction);
        }
    }
}