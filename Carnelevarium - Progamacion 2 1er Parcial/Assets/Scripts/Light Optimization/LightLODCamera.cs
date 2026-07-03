using UnityEngine;

[RequireComponent (typeof(Camera))]
public class LightLODCamera : MonoBehaviour
{
    public static LightLODCamera instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
    }

    public void Activate()
    {
        instance = this;
    }
}
