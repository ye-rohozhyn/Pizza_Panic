using UnityEngine;

public class BodyPartHealth : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int health = 10;
    [SerializeField] private float minSpeedForDamage = 5;
    [SerializeField] private LayerMask ignoreLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (!playerMovement.Enable) return;

        if (((1 << collision.gameObject.layer) & ignoreLayer.value) == 0)
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (rb)
            {
                Vector3 displacement = collision.transform.position - transform.position;
                float speed = displacement.magnitude / Time.fixedDeltaTime / 10;

                if (speed >= minSpeedForDamage)
                {
                    if (speed * 2 >= health)
                    {
                        StartCoroutine(playerMovement.KnockOut());
                    }
                }
            }
        }
    }
}
