using Character;
using UnityEngine;

namespace Gateway
{
    public class DoorManager : MonoBehaviour
    {
        public string sceneName;
        public string nextSpawnID;
    
        private static readonly int IsOpen = Animator.StringToHash("isOpen");
    
        private SceneTransition _sceneTransition;
        private Animator _animator;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _animator = GetComponent<Animator>();
            _sceneTransition = FindFirstObjectByType<SceneTransition>();
        }
    
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
        
            OpenTheDoor();
        }
    
        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
        
            CloseTheDoor();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            var playerController = other.gameObject.GetComponent<PlayerController>();
        
            playerController.nextSpawnID = nextSpawnID;
            playerController.Disappear();
        
            CloseTheDoor();
            StartCoroutine(_sceneTransition.ExitScene(sceneName));
        }

        private void OpenTheDoor()
        {
            _animator.SetBool(IsOpen, true);
        }

        private void CloseTheDoor()
        {
            _animator.SetBool(IsOpen, false);
        }
    }
}
