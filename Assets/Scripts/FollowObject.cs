using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject objectToFollow;

    void Start()
    {
        if (objectToFollow == null)
        {
            objectToFollow = GameObject.FindGameObjectWithTag("Player");
        }
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z );
    }
}
