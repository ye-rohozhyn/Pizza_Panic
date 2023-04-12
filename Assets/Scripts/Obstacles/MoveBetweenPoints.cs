using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public Transform startPoint; // ��������� �����
    public Transform endPoint; // �������� �����
    public float speed = 1.0f; // �������� ��������

    private Rigidbody rb;
    private Vector3 targetPoint; // ������� ����� ��������

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPoint = endPoint.position;
    }

    private void FixedUpdate()
    {
        // ��������� ������ �������� �� ������� ������� �� ������� �����
        Vector3 moveDirection = (targetPoint - transform.position).normalized;

        // ��������� ���� � Rigidbody ��� ��������
        rb.velocity = moveDirection * speed;

        // ���� ������ ����������� � ������� �����, ������ ������� ����� �� ���������������
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            if (targetPoint == endPoint.position)
            {
                targetPoint = startPoint.position;
            }
            else
            {
                targetPoint = endPoint.position;
            }
        }
    }
}
