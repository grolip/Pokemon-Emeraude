using System.Collections.Generic;
using JetBrains.Annotations;
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
        
        private SpriteRenderer _sprite;
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
            
            _sprite = GetComponent<SpriteRenderer>();
            _allKeys.AddRange(new [] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow });
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void Update()
        {
            CheckKeyPress();
            
            if (currentState == State.Busy) return;
            
            // Si au moins une touche enfoncée
            if (_activeKeys.Count > 0)
            {
                var newDirection = GetDirection(_activeKeys[^1]);
                
                if (currentDirection != newDirection) UpdateDirection(newDirection);
                if (currentState != State.Walking) Walk();
            }
            else if (currentState != State.Waiting)
                Stop();
        }

        private void FixedUpdate()
        {
            if (currentState != State.Walking) return;
            Move();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Spawn();
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
        
        [CanBeNull]
        private SpawnPointData GetSpawnPoint()
        {
            var spawnPoints = FindObjectsByType<SpawnPointData>(FindObjectsSortMode.None);
        
            if (spawnPoints.Length == 0) return null;
        
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
            
            return playerSpawnPoint;
        }
        
        public void Spawn()
        {
            var spawnPoint = GetSpawnPoint();
            
            if (spawnPoint)
            {
                UpdateDirection(GetDirection(spawnPoint.playerOrientation));
                base.Spawn(spawnPoint.transform.position);
            }
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
        
        public bool HaveSameDirection(string direction)
        {
            return currentDirection == GetDirection(direction);
        }
    }
}