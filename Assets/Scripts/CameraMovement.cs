using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothStep = 0.125f;
    [SerializeField] private Vector3 offset;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset; 
        Vector3 smoothedPosition = Vector3.Lerp(_transform.position, desiredPosition, smoothStep);

        _transform.position = smoothedPosition;
    }
}
