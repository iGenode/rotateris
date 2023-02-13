using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform Anchor;

    void Update()
    {
        transform.LookAt(Anchor);
    }
}
