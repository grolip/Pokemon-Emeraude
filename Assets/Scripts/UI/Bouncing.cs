using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Bouncing : MonoBehaviour
    {
        public float bounceHeight = 1f;      // hauteur du rebond
        public float bounceSpeed = 2f;        // vitesse du rebond

        private Vector2 _startPosition;
        private RectTransform _arrowUI;
        private Image _arrowImage;
        private bool _isBouncing;
        private float _bounceTimer;

        private void Awake()
        {
            _arrowUI = GetComponent<RectTransform>();
            _arrowImage = GetComponent<Image>();
            _startPosition = _arrowUI.anchoredPosition;
        
            _arrowImage.enabled = false;
        }
    
        void Update()
        {
            if (_isBouncing)
            {
                _bounceTimer += Time.deltaTime * bounceSpeed;
                var offsetY = Mathf.Sin(_bounceTimer) * bounceHeight;
                _arrowUI.anchoredPosition = _startPosition + new Vector2(0, offsetY);
            }
        }

        public void ShowArrow()
        {
            _arrowUI.anchoredPosition = _startPosition;
            _bounceTimer = 0f;
            _arrowImage.enabled = true;
            StartCoroutine(StartBounceSmooth());
        }

        public void HideArrow()
        {
            _arrowImage.enabled = false;
            _isBouncing = false;
        }

        private IEnumerator StartBounceSmooth()
        {
            yield return null; // attend une frame pour éviter le glitch
            _isBouncing = true;
        }
    }
}






