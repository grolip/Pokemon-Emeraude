using UnityEngine;

namespace Character
{
    public class JumpZone : MonoBehaviour
    {
        public string directionAllowed;
        
        private const float JumpDistance = 2f;
        private const float JumpDuration = 0.5f;
        
        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (PlayerController.Instance.HaveSameDirection(directionAllowed))
            {
                StartCoroutine(PlayerController.Instance.JumpOver(JumpDistance, JumpDuration));
            }
        }
    }
}
