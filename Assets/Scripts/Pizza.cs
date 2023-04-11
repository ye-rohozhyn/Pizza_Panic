using UnityEngine;

public class Pizza : MonoBehaviour
{
    [SerializeField] private Rigidbody pizzaRigidbody;
    [SerializeField] private BoxCollider physicsCollider;

    private void Start()
    {
        ClearParent(false);
    }

    public void SetParent(Transform parent, Vector3 offset)
    {
        pizzaRigidbody.isKinematic = true;
        physicsCollider.enabled = false;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero + offset;
        transform.localRotation = Quaternion.identity;
    }

    public void ClearParent(bool front)
    {
        pizzaRigidbody.isKinematic = false;
        physicsCollider.enabled = true;
        transform.SetParent(null);
        if (front) transform.position += transform.forward;
    }
}
