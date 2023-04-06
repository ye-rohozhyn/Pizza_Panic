using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRogidbody;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;

    private bool _isRunning;
    private int _isRunningHash;

    private Transform _playerTransform;
    private Vector3 _movement;

    private void Start()
    {
        _playerTransform = playerRogidbody.transform;
        _isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Update()
    {
        _movement.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _isRunning = _movement != Vector3.zero;
        if (playerAnimator) playerAnimator.SetBool(_isRunningHash, _isRunning);
    }


    private void FixedUpdate()
    {
        MoveCharacter();
    }


    private void MoveCharacter()
    {
        if (!_isRunning & playerRogidbody) return;

        _movement.Normalize();
        playerRogidbody.velocity = moveSpeed * _movement;

        _playerTransform.forward = _movement;
    }
}
