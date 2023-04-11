using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private ConfigurableJoint[] bones;
    [SerializeField] private float unconsciousSpring = 500;
    private JointDrive[] _driveBones;
    private Coroutine[] _coroutines;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveSpeedInAir = 1f;
    [SerializeField] private float jumpForce = 100f;

    [Header("Gravity")]
    [SerializeField] private Vector3 gravity = Physics.gravity;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallingGravityScale = 1f;

    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ConfigurableJoint pelvis;
    [SerializeField] private float duration = 1.0f;

    [Header("Check ground")]
    [SerializeField] private float checkSphereRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    private bool _isRunning;
    private int _isRunningHash;
    private bool _isGrounded;
    private bool _jumpStarted;
    private bool _enable = true;
    public bool Enable { get { return _enable; } }

    private Transform _playerTransform;
    private Vector3 _movement;
    private JointDrive _pelvisDrive;

    private float timeElapsed;

    private void Start()
    {
        _playerTransform = playerRigidbody.transform;
        _isRunningHash = Animator.StringToHash("isRunning");
        _pelvisDrive = pelvis.slerpDrive;
        _driveBones = new JointDrive[bones.Length];
        _coroutines = new Coroutine[bones.Length + 1];
        for (int i = 0; i < bones.Length; i++)
            _driveBones[i] = bones[i].slerpDrive;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _enable = !_enable;

            if (_enable) EnableActiveRagdoll();
            else DisableActiveRagdoll();
        }

        if (!_enable) return;

        _movement.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _isRunning = _movement != Vector3.zero;
        
        if (playerAnimator) playerAnimator.SetBool(_isRunningHash, _isRunning);
        if (Input.GetButtonDown("Jump") & _isGrounded) _jumpStarted = true; 
    }

    private void FixedUpdate()
    {
        if (!_enable) return;

        _isGrounded = Physics.CheckSphere(_playerTransform.position, checkSphereRadius, groundMask);

        ControlGravityScale();
        MoveCharacter();
    }

    private void DisableActiveRagdoll()
    {
        StopSlerpDriveReturn();

        _isRunning = false;
        playerAnimator.SetBool(_isRunningHash, _isRunning);

        pelvis.angularXMotion = ConfigurableJointMotion.Free;
        pelvis.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive drive = _pelvisDrive;
        drive.positionSpring = 0;
        drive.positionDamper = 0;
        pelvis.slerpDrive = drive;

        for (int i = 0; i < _driveBones.Length; i++)
        {
            drive = _driveBones[i];
            drive.positionSpring = unconsciousSpring;
            bones[i].slerpDrive = drive;
        }
    }

    private void EnableActiveRagdoll()
    {
        _coroutines[0] = StartCoroutine(SlerpDriveReturn(pelvis, _pelvisDrive.positionSpring, true));
        
        for (int i = 0; i < _driveBones.Length; i++)
        {
            _coroutines[i+1] = StartCoroutine(SlerpDriveReturn(bones[i], _driveBones[i].positionSpring, false));
        }
    }

    private IEnumerator SlerpDriveReturn(ConfigurableJoint joint, float targetValue, bool lockXZ)
    {
        float startValue = joint.slerpDrive.positionSpring;
        float timeStarted = Time.time;

        while (Time.time - timeStarted < duration)
        {
            float currentValue = Mathf.Lerp(startValue, targetValue, (Time.time - timeStarted) / duration);

            JointDrive newDrive = joint.slerpDrive;
            newDrive.positionSpring = currentValue;

            joint.slerpDrive = newDrive;

            yield return null;
        }

        JointDrive finalDrive = joint.slerpDrive;
        finalDrive.positionSpring = targetValue;

        joint.slerpDrive = finalDrive;

        if (lockXZ)
        {
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
    }

    public void StopSlerpDriveReturn()
    {
        for (int i = 0; i < _coroutines.Length; i++)
        {
            if (_coroutines[i] != null)
            {
                StopCoroutine(_coroutines[i]);
                _coroutines[i] = null;
            }
        }
    }

    private void ControlGravityScale()
    {
        if (playerRigidbody.velocity.y >= 0)
        {
            playerRigidbody.AddForce((gravityScale - 1) * playerRigidbody.mass * gravity);
        }
        else
        {
            playerRigidbody.AddForce((fallingGravityScale - 1) * playerRigidbody.mass * gravity);
        }
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
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpStarted = false;
        }

        if (playerRigidbody.velocity.magnitude >= speed)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * speed;
        }
    }
}
