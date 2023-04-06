using UnityEngine;

public class HandsController : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float defaultSpring = 1000;
    [SerializeField] private float maxSpring = 9999999;

    [Header("Hands")]
    [SerializeField] private ConfigurableJoint[] physicalHandParts;
    [SerializeField] private Quaternion[] targetRotation;
    [SerializeField] private ConfigurableJoint handLeft;
    [SerializeField] private ConfigurableJoint handRight;

    private Quaternion[] _startHandPartsRotation;

    [Header("Pizza Holder")]
    [SerializeField] private Transform pizzaHolder;
    [SerializeField] private Rigidbody middleSpine;
    [SerializeField] private FixedJoint leftFixedJoint, rightFixedJoint;
    [SerializeField] private ConfigurableJoint pizzaHolderConfJoint;

    private bool _isCarrying;
    private int _isCarryingHash;
    private JointDrive _drive;
    private Rigidbody _handLeftRb, _handRightRb;
    private Transform _rightHand, _leftHand;

    private void Start()
    {
        _isCarryingHash = Animator.StringToHash("isCarrying");

        _drive = handLeft.slerpDrive;
        _rightHand = handRight.transform;
        _leftHand = handLeft.transform;
        _handLeftRb = handLeft.GetComponent<Rigidbody>();
        _handRightRb = handRight.GetComponent<Rigidbody>();

        _startHandPartsRotation = new Quaternion[physicalHandParts.Length];

        for (int i = 0; i < physicalHandParts.Length; i++)
        {
            _startHandPartsRotation[i] = physicalHandParts[i].transform.localRotation;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickCarryingBtn();
        }
    }

    public void OnClickCarryingBtn()
    {
        _isCarrying = !_isCarrying;

        playerAnimator.SetBool(_isCarryingHash, _isCarrying);

        _drive.positionSpring = _isCarrying ? maxSpring : defaultSpring;

        foreach (ConfigurableJoint joint in physicalHandParts) joint.slerpDrive = _drive;
        handLeft.slerpDrive = _drive;
        handRight.slerpDrive = _drive;

        if (_isCarrying)
        {
            for (int i = 0; i < physicalHandParts.Length; i++)
            {
                physicalHandParts[i].targetRotation = Quaternion.Inverse(targetRotation[i]) * _startHandPartsRotation[i];
                physicalHandParts[i].transform.localRotation = targetRotation[i];
            }

            pizzaHolder.position = Vector3.Lerp(_rightHand.position, _leftHand.position, 0.5f);
            pizzaHolder.localRotation = Quaternion.Euler(8, 0, 0);

            pizzaHolderConfJoint.connectedBody = middleSpine;
            leftFixedJoint.connectedBody = _handLeftRb;
            rightFixedJoint.connectedBody = _handRightRb;
        }
        else
        {
            for (int i = 0; i < physicalHandParts.Length; i++)
            {
                physicalHandParts[i].targetRotation = Quaternion.identity;
            }

            pizzaHolderConfJoint.connectedBody = null;
            leftFixedJoint.connectedBody = null;
            rightFixedJoint.connectedBody = null;
        }
    }
}
