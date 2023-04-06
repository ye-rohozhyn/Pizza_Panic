using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class PhysicalBodyPart : MonoBehaviour {

    [SerializeField] private Transform target;
    private ConfigurableJoint _joint;
    private Quaternion _startRotation;

    private void Start() 
    {
        _joint = GetComponent<ConfigurableJoint>();
        _startRotation = transform.localRotation;
    }

    private void FixedUpdate() 
    {
        _joint.targetRotation = Quaternion.Inverse(target.localRotation) * _startRotation;
    }
}
