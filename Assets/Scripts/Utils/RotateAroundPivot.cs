using UnityEngine;

public class RotateAroundPivot : MonoBehaviour
{
    [SerializeField]
    private Vector3 _pivot;
    [SerializeField]
    private float _speed;

    private void Update()
    {
        transform.RotateAround(_pivot, Vector3.up, _speed * Time.deltaTime);
    }
}
