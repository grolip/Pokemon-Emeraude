using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gateway
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private UI.Fader fader;
        
        void Awake()
        {
            if (FindObjectsByType<SceneTransition>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (fader != null)
            {
                StopAllCoroutines();
                StartCoroutine(fader.FadeIn(0.3f));
            }
        }
        
        public IEnumerator ExitScene(string sceneName)
        {
            StopAllCoroutines();
            yield return StartCoroutine(fader.FadeOut(0.3f));
            
            SceneManager.LoadScene(sceneName);
        }
    }
}
