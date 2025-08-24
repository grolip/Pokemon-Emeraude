using System.Collections;
using UnityEngine;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        private const float FadeDuration = 0.6f;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public IEnumerator FadeIn(float fadeDuration = FadeDuration, bool noDelay = false)
        {
            canvasGroup.alpha = 1;
            
            if (!noDelay)
                yield return new WaitForSeconds(0.5f);
        
            var t = FadeDuration;
        
            while (t > 0)
            {
                t -= Time.deltaTime;
                canvasGroup.alpha = t / fadeDuration;
                yield return null;
            }
            canvasGroup.alpha = 0;
        }

        public IEnumerator FadeOut(float fadeDuration = FadeDuration)
        {
            var t = 0f;
        
            // Gestion du Fade out
            while (t < FadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = t / fadeDuration;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }
    }
}
