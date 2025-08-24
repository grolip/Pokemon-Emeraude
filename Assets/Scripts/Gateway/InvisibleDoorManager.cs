using Character;
using Gateway;
using UI;
using UnityEngine;

public class InvisibleDoorManager : MonoBehaviour
{
    public GameObject exitIndicator;
    public string sceneName;
    public string nextSpawnID;
    
    private SceneTransition _sceneTransition;
    private Bouncing _bouncing;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneTransition = FindFirstObjectByType<SceneTransition>();
        _bouncing = exitIndicator.GetComponent<Bouncing>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        _bouncing.ShowArrow();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        _bouncing.HideArrow();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        var playerController = other.gameObject.GetComponent<PlayerController>();
        
        playerController.nextSpawnID = nextSpawnID;
        playerController.Disappear();
        
        StartCoroutine(_sceneTransition.ExitScene(sceneName));
    }
    
}
