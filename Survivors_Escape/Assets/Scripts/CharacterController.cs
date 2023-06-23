using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurvivorsEscape
{
    public enum CharacterStance { STANDING, CROUCHING, PRONING }
    public class CharacterController : MonoBehaviour
    {
        private InputManager _inputs;
        private CameraController _cameraController;
        private Animator _animator;
        private CapsuleCollider _capsuleCollider;
        private CharacterStance _stance;

        [Header("Speed [Normal Sprint]")]
        [SerializeField] private Vector2 _standingSpeed = new Vector2(0, 0);
        [SerializeField] private Vector2 _crouchingSpeed = new Vector2(0, 0);
        [SerializeField] private Vector2 _proningSpeed = new Vector2(0, 0);

        [Header("Capsule [Radius Height YOffset]")]
        [SerializeField] private Vector3 _standingCapsule = Vector3.zero;
        [SerializeField] private Vector3 _crouchingCapsule = Vector3.zero;
        [SerializeField] private Vector3 _proningCapsule = Vector3.zero;

        [Header("Sharpness")]
        [SerializeField] private float _moveSharpness = 10f;
        [SerializeField] private float _standingRotationSharpness = 10f;
        [SerializeField] private float _crouchingRotationSharpness = 10f;
        [SerializeField] private float _proningRotationSharpness = 10f;

        #region ANIMATOR_STATE_NAMES
        private const string _standToCrouch = "Base Layer.Base Crouching";
        private const string _standToProne = "Base Layer.Stand to Prone";
        private const string _crouchToStand = "Base Layer.Base Standing";
        private const string _crouchToProne = "Base Layer.Crouch to Prone";
        private const string _proneToStand = "Base Layer.Prone to Stand";
        private const string _proneToCrouch = "Base Layer.Prone to Crouch";
        #endregion

        private LayerMask _layerMask;
        private Collider[] _obstructions = new Collider[8];

        private float _runSpeed;
        private float _sprintSpeed;
        private float _rotationSharpness;

        private float _targetSpeed;
        private Quaternion _targetRotation;

        private float _newSpeed;
        private Vector3 _newVelocity;
        private Quaternion _newRotation;

        private bool _proning;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _cameraController = GetComponent<CameraController>();
            _inputs = GetComponent<InputManager>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _runSpeed = _standingSpeed.x;
            _sprintSpeed = _standingSpeed.y;
            _rotationSharpness = _standingRotationSharpness;

            _stance = CharacterStance.STANDING;
            SetCapsuleDimensions(_standingCapsule);

            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!(Physics.GetIgnoreLayerCollision(gameObject.layer, i)))
                {
                    mask |= 1 << i;
                }
            }
            _layerMask = mask;

            _animator.applyRootMotion = false;
        }

        private void Update()
        {
            if (_proning) return;

            Vector3 moveInputVector = new Vector3(_inputs.MoveAxisRight, 0, _inputs.MoveAxisForward).normalized;
            Vector3 cameraPlanarDirection = _cameraController._cameraPlanarDirection;
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection);
            moveInputVector = cameraPlanarRotation * moveInputVector;

            if (_inputs.Sprint.Pressed())
            {
                _targetSpeed = moveInputVector != Vector3.zero ? _sprintSpeed : 0;
            }
            else
            {
                _targetSpeed = moveInputVector != Vector3.zero ? _runSpeed : 0;
            }

            _newSpeed = Mathf.Lerp(_newSpeed, _targetSpeed, Time.deltaTime * _moveSharpness);

            _newVelocity = moveInputVector * _targetSpeed;
            transform.Translate(_newVelocity * Time.deltaTime, Space.World);

            if (_targetSpeed != 0)
            {
                _targetRotation = Quaternion.LookRotation(moveInputVector);
                _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
                transform.rotation = _newRotation;
            }

            _animator.SetFloat("Forward", _newSpeed);
        }

        private void LateUpdate()
        {
            if (_proning) return;

            switch (_stance)
            {
                case CharacterStance.STANDING:
                    if (_inputs.Crouch.PressedDown()) { RequestStanceChange(CharacterStance.CROUCHING); }
                    else if (_inputs.Prone.PressedDown()) { RequestStanceChange(CharacterStance.PRONING); }
                    break;

                case CharacterStance.CROUCHING:
                    if (_inputs.Crouch.PressedDown()) { RequestStanceChange(CharacterStance.STANDING); }
                    else if (_inputs.Prone.PressedDown()) { RequestStanceChange(CharacterStance.PRONING); }
                    break;

                case CharacterStance.PRONING:
                    if (_inputs.Crouch.PressedDown()) { RequestStanceChange(CharacterStance.CROUCHING); }
                    else if (_inputs.Prone.PressedDown()) { RequestStanceChange(CharacterStance.STANDING); }
                    break;
            }
        }

        public bool RequestStanceChange(CharacterStance newStance)
        {
            if (_stance == newStance) return true;

            switch (_stance)
            {
                case CharacterStance.STANDING:
                    if (newStance == CharacterStance.CROUCHING)
                    {
                        if (!CharacterOverlap(_crouchingCapsule))
                        {
                            _runSpeed = _crouchingSpeed.x;
                            _sprintSpeed = _crouchingSpeed.y;
                            _rotationSharpness = _crouchingRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_standToCrouch, 0.5f);
                            SetCapsuleDimensions(_crouchingCapsule);

                            return true;
                        }
                    }
                    else if (newStance == CharacterStance.PRONING)
                    {
                        if (!CharacterOverlap(_proningCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat("Forward", 0);

                            _runSpeed = _proningSpeed.x;
                            _sprintSpeed = _proningSpeed.y;
                            _rotationSharpness = _proningRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_standToProne, 0.5f);
                            SetCapsuleDimensions(_proningCapsule);

                            return true;
                        }
                    }
                    break;

                case CharacterStance.CROUCHING:
                    if (newStance == CharacterStance.STANDING)
                    {
                        if (!CharacterOverlap(_standingCapsule))
                        {
                            _runSpeed = _standingSpeed.x;
                            _sprintSpeed = _standingSpeed.y;
                            _rotationSharpness = _standingRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_crouchToStand, 0.5f);
                            SetCapsuleDimensions(_standingCapsule);

                            return true;
                        }
                    }
                    else if (newStance == CharacterStance.PRONING)
                    {
                        if (!CharacterOverlap(_proningCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat("Forward", 0);

                            _runSpeed = _proningSpeed.x;
                            _sprintSpeed = _proningSpeed.y;
                            _rotationSharpness = _proningRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_crouchToProne, 0.5f);
                            SetCapsuleDimensions(_proningCapsule);

                            return true;
                        }
                    }
                    break;

                case CharacterStance.PRONING:
                    if (newStance == CharacterStance.STANDING)
                    {
                        if (!CharacterOverlap(_standingCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat("Forward", 0);

                            _runSpeed = _standingSpeed.x;
                            _sprintSpeed = _standingSpeed.y;
                            _rotationSharpness = _standingRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_proneToStand, 0.5f);
                            SetCapsuleDimensions(_standingCapsule);

                            return true;
                        }
                    }
                    else if (newStance == CharacterStance.CROUCHING)
                    {
                        if (!CharacterOverlap(_crouchingCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat("Forward", 0);

                            _runSpeed = _crouchingSpeed.x;
                            _sprintSpeed = _crouchingSpeed.y;
                            _rotationSharpness = _crouchingRotationSharpness;
                            _stance = newStance;

                            _animator.CrossFadeInFixedTime(_proneToCrouch, 0.5f);
                            SetCapsuleDimensions(_crouchingCapsule);

                            return true;
                        }
                    }
                    break;
            }

            return false;

        }

        private bool CharacterOverlap(Vector3 capsuleDimensions)
        {
            float radius = capsuleDimensions.x;
            float height = capsuleDimensions.y;
            Vector3 center = new Vector3(_capsuleCollider.center.x, capsuleDimensions.z, _capsuleCollider.center.z);

            Vector3 point0;
            Vector3 point1;

            if (height < radius * 2)
            {
                point0 = transform.position + center;
                point1 = transform.position + center;
            }
            else
            {
                point0 = transform.position + center + (transform.up * (height * 0.5f - radius));
                point1 = transform.position + center - (transform.up * (height * 0.5f - radius));
            }
            radius = radius - 0.1f;

            int numOverlaps = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, _obstructions, _layerMask);
            for (int i = 0; i < numOverlaps; i++)
            {
                if (_obstructions[i] == _capsuleCollider)
                    numOverlaps--;
            }

            return numOverlaps > 0;
        }

        private void SetCapsuleDimensions(Vector3 capsuleDimensions)
        {
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, capsuleDimensions.z, _capsuleCollider.center.z);
            _capsuleCollider.radius = capsuleDimensions.x;
            _capsuleCollider.height = capsuleDimensions.y;
        }

        public void OnEvent(string eventName)
        {
            switch(eventName)
            {
                case "ProneEnd":
                    _proning = false;
                    break;

                default:
                    break;
            }
        }

        public Vector2 GetStandingSpeed() { return _standingSpeed; }
        public float GetMoveSharpness() { return _moveSharpness; }
        public float GetRotationSharpness() { return _rotationSharpness; }

        public Animator GetAnimator() { return _animator; }
        public CameraController GetCameraController() { return _cameraController; }
        public InputManager GetInputManager() { return _inputs; }
        public Transform GetPlayerTransform() { return transform; }
    }
}
