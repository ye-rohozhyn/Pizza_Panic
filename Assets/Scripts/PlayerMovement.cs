using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveSpeedInAir = 1f;
    [SerializeField] private float jumpForce = 100f;

    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;

    [Header("Check ground")]
    [SerializeField] private float checkSphereRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    public bool _isRunning;
    private int _isRunningHash;
    public bool _isGrounded;
    public bool _jumpStarted;

    private Transform _playerTransform;
    private Vector3 _movement;

    private void Start()
    {
        _playerTransform = playerRigidbody.transform;
        _isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Update()
    {
        _movement.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _isRunning = _movement != Vector3.zero;
        _isGrounded = Physics.CheckSphere(_playerTransform.position, checkSphereRadius, groundMask);
        if (playerAnimator) playerAnimator.SetBool(_isRunningHash, _isRunning);
        if (Input.GetButtonDown("Jump") & _isGrounded) _jumpStarted = true; 
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        float speed = _isGrounded ? moveSpeed : moveSpeedInAir;

        _movement.Normalize();
        if (_isRunning)
        {
            playerRigidbody.velocity = speed * _movement;
            _playerTransform.forward = _movement;
        }

        if (_jumpStarted)
        {
            _jumpStarted = false;
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (playerRigidbody.velocity.magnitude >= speed)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * speed;
        }

        
    }
}
