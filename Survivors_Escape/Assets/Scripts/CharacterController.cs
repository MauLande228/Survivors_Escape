using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
    private InputManager _inputs;
    private CameraController _cameraController;
    private Animator _animator;
    private CapsuleCollider _capsuleCollider;

    [Header("Speed [Normal Sprint]")]
    [SerializeField] private Vector2 _standingSpeed = new Vector2(0, 0);

    [Header("Sharpness")]
    [SerializeField] private float _moveSharpness = 10f;
    [SerializeField] private float _rotationSharpness = 10f;

    private LayerMask _layerMask;
    private Collider[] _obstructions = new Collider[8];

    private float _runSpeed;
    private float _sprintSpeed;

    private float _targetSpeed;
    private Quaternion _targetRotation;

    private float _newSpeed;
    private Vector3 _newVelocity;
    private Quaternion _newRotation;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _cameraController = GetComponent<CameraController>();
        _inputs = GetComponent<InputManager>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _runSpeed = _standingSpeed.x;
        _sprintSpeed = _standingSpeed.y;

        _animator.applyRootMotion = false;
    }

    private void Update()
    {
        Vector3 moveInputVector = new Vector3(_inputs.MoveAxisRight, 0, _inputs.MoveAxisForward).normalized;
        Debug.Log("MoveAxisRight: " + moveInputVector.x + ", MoveAxisForward" + moveInputVector.z);
        Vector3 cameraPlanarDirection = _cameraController._cameraPlanarDirection;
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection);
        moveInputVector = cameraPlanarRotation * moveInputVector;

        if(_inputs.Sprint.Pressed())
        {
            _targetSpeed = moveInputVector != Vector3.zero ? _sprintSpeed : 0;
        }
        else
        {
            _targetSpeed = moveInputVector != Vector3.zero ? _runSpeed : 0;
        }
        _newSpeed = Mathf.Lerp(_newSpeed, _targetSpeed, Time.deltaTime * _moveSharpness);

        _newVelocity = moveInputVector * _newSpeed;
        transform.Translate(_newVelocity * Time.deltaTime, Space.World);

        if(_targetSpeed != 0)
        {
            _targetRotation = Quaternion.LookRotation(moveInputVector);
            _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
            //transform.rotation = _newRotation;
            transform.rotation = _newRotation;
        }

        _animator.SetFloat("Forward", _newSpeed);
    }
}
