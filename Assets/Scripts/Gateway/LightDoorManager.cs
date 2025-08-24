using System.Collections;
using Character;
using UnityEngine;

namespace Gateway
{
    public class LightDoorManager : MonoBehaviour
    {
        public string nextSpawnID;
        
        private UI.Fader _fader;

        private void Start()
        {
            _fader = FindFirstObjectByType<UI.Fader>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            StopAllCoroutines();
            StartCoroutine(EnterZone(other.gameObject));
        }
        
        private IEnumerator EnterZone(GameObject player)
        {
            var playerController = player.GetComponent<PlayerController>();
            
            playerController.nextSpawnID = nextSpawnID;
            playerController.Disappear();
            yield return StartCoroutine(_fader.FadeOut(0.3f));
            
            playerController.FindSpawnPoint();
            playerController.Appear();
            yield return StartCoroutine(_fader.FadeIn(0.3f,true));
        }
    }
}
