using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour {

    [SerializeField] private Collider[] _allColliders;

    private void Awake() {
        for (int a = 0; a < _allColliders.Length; a++) {
            for (int b = 0; b < _allColliders.Length; b++) {
                Physics.IgnoreCollision(_allColliders[a], _allColliders[b], true);
            }
        }
    }

}
