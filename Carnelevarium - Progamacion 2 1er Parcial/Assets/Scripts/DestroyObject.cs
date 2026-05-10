using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float time;
    void Start()
    {
        Destroy(gameObject, time);  
    }
}
