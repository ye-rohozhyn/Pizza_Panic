using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public Transform startPoint; // Начальная точка
    public Transform endPoint; // Конечная точка
    public float speed = 1.0f; // Скорость движения

    private Rigidbody rb;
    private Vector3 targetPoint; // Целевая точка движения

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPoint = endPoint.position;
    }

    private void FixedUpdate()
    {
        // Вычисляем вектор движения от текущей позиции до целевой точки
        Vector3 moveDirection = (targetPoint - transform.position).normalized;

        // Применяем силу к Rigidbody для движения
        rb.velocity = moveDirection * speed;

        // Если объект приблизился к целевой точке, меняем целевую точку на противоположную
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
