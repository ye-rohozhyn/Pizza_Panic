using System.Collections.Generic;
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

    [Header("Grabbing")]
    [SerializeField] private Transform leftCheckPoint;
    [SerializeField] private Transform rightCheckPoint;
    [SerializeField] private LayerMask grabbingLayer;
    [SerializeField] private float checkRadius = 0.1f;
    [SerializeField] private ConfigurableJoint leftUpperArm;
    [SerializeField] private ConfigurableJoint rightUpperArm;
    [SerializeField] private Quaternion leftTargetRotation;
    [SerializeField] private Quaternion rightTargetRotation;

    private HandState _handState;
    private int _isCarryingHash;
    private JointDrive _drive;
    private Rigidbody _handLeftRb, _handRightRb;
    private Transform _rightHand, _leftHand;
    private List<Pizza> _pizzas = new();
    private GameObject _leftGrabbingObj, _rightGrabbingObj;

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

    private void OnTriggerEnter(Collider other)
    {
        if (_handState == HandState.Grabbing) return;

        Pizza pizza = other.GetComponent<Pizza>();

        if (!pizza) return;

        if (_pizzas.IndexOf(pizza) == -1)
        {
            pizza.SetParent(pizzaHolder, new(0, 0.0015f * _pizzas.Count, 0.003f));
            _pizzas.Add(pizza);
            
            if (_pizzas.Count == 1) EnableCarrying();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_handState == HandState.Carrying)
            {
                DisableCarrying();
            }
            else
            {
                _handState = HandState.Grabbing;

                leftUpperArm.targetRotation = leftTargetRotation;
                rightUpperArm.targetRotation = rightTargetRotation;
            }
        }
        else if (Input.GetKey(KeyCode.E) & _handState == HandState.Grabbing)
        {
            if (!_leftGrabbingObj)
            {
                var colliders = Physics.OverlapSphere(leftCheckPoint.position, checkRadius, grabbingLayer);

                if (colliders.Length == 0) return;

                Rigidbody grabbingRb = colliders[0].GetComponent<Rigidbody>();

                if (grabbingRb)
                {
                    _leftGrabbingObj = grabbingRb.gameObject;

                    FixedJoint fj = _leftGrabbingObj.AddComponent<FixedJoint>();
                    fj.connectedBody = _handLeftRb;
                    fj.breakForce = 9000;
                }
            }

            if (!_rightGrabbingObj)
            {
                var colliders = Physics.OverlapSphere(rightCheckPoint.position, checkRadius, grabbingLayer);

                if (colliders.Length == 0) return;

                Rigidbody grabbingRb = colliders[0].GetComponent<Rigidbody>();

                if (grabbingRb)
                {
                    _rightGrabbingObj = grabbingRb.gameObject;

                    FixedJoint fj = _rightGrabbingObj.AddComponent<FixedJoint>();
                    fj.connectedBody = _handRightRb;
                    fj.breakForce = 9000;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            if (_pizzas.Count == 0)
            {
                if (_handState == HandState.Grabbing)
                {
                    if (_leftGrabbingObj == _rightGrabbingObj & _leftGrabbingObj != null & _rightGrabbingObj != null)
                    {
                        foreach(FixedJoint fj in _leftGrabbingObj.GetComponents<FixedJoint>())
                        {
                            Destroy(fj);
                        }

                        _leftGrabbingObj = null;
                        _rightGrabbingObj = null;

                        _handState = HandState.Normal;

                        leftUpperArm.targetRotation = Quaternion.identity;
                        rightUpperArm.targetRotation = Quaternion.identity;

                        return;
                    }

                    if (_leftGrabbingObj)
                    {
                        Destroy(_leftGrabbingObj.GetComponent<FixedJoint>());
                        _leftGrabbingObj = null;
                    }

                    if (_rightGrabbingObj)
                    {
                        Destroy(_rightGrabbingObj.GetComponent<FixedJoint>());
                        _rightGrabbingObj = null;
                    }
                }

                _handState = HandState.Normal;

                leftUpperArm.targetRotation = Quaternion.identity;
                rightUpperArm.targetRotation = Quaternion.identity;
            }
        }
    }

    private void EnableCarrying()
    {
        _handState = HandState.Carrying;

        playerAnimator.SetBool(_isCarryingHash, true);

        _drive.positionSpring = maxSpring;

        foreach (ConfigurableJoint joint in physicalHandParts) joint.slerpDrive = _drive;
        handLeft.slerpDrive = _drive;
        handRight.slerpDrive = _drive;

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

    private void DisableCarrying()
    {
        _handState = HandState.Normal;

        playerAnimator.SetBool(_isCarryingHash, false);

        _drive.positionSpring = defaultSpring;

        foreach (ConfigurableJoint joint in physicalHandParts) joint.slerpDrive = _drive;
        handLeft.slerpDrive = _drive;
        handRight.slerpDrive = _drive;

        foreach (Pizza pizza in _pizzas) pizza.ClearParent();
        _pizzas.Clear();

        for (int i = 0; i < physicalHandParts.Length; i++)
        {
            physicalHandParts[i].targetRotation = Quaternion.identity;
        }

        pizzaHolderConfJoint.connectedBody = null;
        leftFixedJoint.connectedBody = null;
        rightFixedJoint.connectedBody = null;
    }
}


public enum HandState
{
    Normal, Carrying, Grabbing
}