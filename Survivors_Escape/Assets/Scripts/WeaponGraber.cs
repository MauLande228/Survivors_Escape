using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponGraber : MonoBehaviour
{
    private Transform _prevParent;
    private GameObject _handVessel;

    void Start()
    {
        _prevParent = transform.parent;
    }

    private void OnTransformParentChanged()
    {
        if (transform.parent != _prevParent)
        {
            Debug.Log("Object has been reparented.");

            var nw = GetComponent<NetworkObject>();
            if (nw != null)
                nw.enabled = false;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
                Destroy(rb);

            _handVessel = GameObject.Find("mixamorig1:RightHand");
            if (_handVessel != null)
                Debug.Log("HAND VESSEL FOUND IN WEAPON GRABER");

            _prevParent = transform.parent;
        }
    }

    void Update()
    {
        if (_handVessel != null)
        {
            transform.position = _handVessel.transform.position;
            transform.rotation = _handVessel.transform.rotation;
        }
    }
}
