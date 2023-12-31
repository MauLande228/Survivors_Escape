using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurvivorsEscape
{
    public enum CharacterStance { STANDING, CROUCHING}
    public class CharacterController : MonoBehaviour, IHitResponder
    {
        private InputManager _inputs;
        private CameraController _cameraController;
        private Animator _animator;
        private EventHandler _eventHandler;
        private CapsuleCollider _capsuleCollider;
        private CharacterStance _stance;

        [Header("Speed [Normal Sprint]")]
        [SerializeField] private Vector3 _standingSpeed = new Vector3(6, 8, 2);
        [SerializeField] private Vector2 _crouchingSpeed = new Vector2(0, 0);
        [SerializeField] private Vector2 _proningSpeed = new Vector2(0, 0);

        [Header("Capsule [Radius Height YOffset]")]
        [SerializeField] private Vector3 _standingCapsule = Vector3.zero;
        [SerializeField] private Vector3 _crouchingCapsule = Vector3.zero;
        [SerializeField] private Vector3 _proningCapsule = Vector3.zero;

        [Header("Attacking")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private SEHitBox _hitBox;

        [Header("Sharpness")]
        [SerializeField] private float _moveSharpness = 10f;
        [SerializeField] private float _standingRotationSharpness = 10f;
        [SerializeField] private float _crouchingRotationSharpness = 10f;
        [SerializeField] private float _proningRotationSharpness = 10f;

        #region ANIMATOR_STATE_NAMES
        private const string _standToCrouch = "Base Layer.Base Crouching";
        private const string _crouchToStand = "Base Layer.Base Standing";
        private const string _meleeAtack    = "Base Layer.Melee attack horizontal";
        #endregion

        private bool _inAnimation;
        private Vector3 _animatorVelocity;
        private Quaternion _animatorDeltaRotation;
        private bool _hitting;
        private List<GameObject> _objectsHit = new List<GameObject>();

        private bool _strafing;
        private bool _sprinting;
        private float _strafeParameter;
        private Vector3 _strafeParameterXZ;

        private LayerMask _layerMask;
        private Collider[] _obstructions = new Collider[8];

        private float _runSpeed;
        private float _sprintSpeed;
        private float _walkSpeed;
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
            _eventHandler = GetComponent<EventHandler>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _runSpeed = _standingSpeed.x;
            _sprintSpeed = _standingSpeed.y;
            _walkSpeed = _standingSpeed.z;
            _rotationSharpness = _standingRotationSharpness;
            _hitBox.HitResponder = this;

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

            //_animator.applyRootMotion = false;
            _eventHandler.Event.AddListener(OnEvent);
        }

        private void Update()
        {
            if (_proning) return;

            Vector3 moveInputVector = new Vector3(_inputs.MoveAxisRight, 0, _inputs.MoveAxisForward);
            Vector3 cameraPlanarDirection = _cameraController._cameraPlanarDirection;
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection);

            Vector3 moveInputVectorOrientation = cameraPlanarRotation * moveInputVector.normalized;

            if(_strafing)
            {
                _sprinting = _inputs.Sprint.PressedDown() && (moveInputVector != Vector3.zero);
                _strafing = !_inputs.Aim.PressedDown() && !_sprinting;
            }
            else
            {
                _sprinting = _inputs.Sprint.Pressed() && (moveInputVector != Vector3.zero);
                _strafing = _inputs.Aim.PressedDown();
            }

            if (_sprinting)
                _targetSpeed = moveInputVector != Vector3.zero ? _sprintSpeed : 0;
            else if (_strafing)
                _targetSpeed = moveInputVector != Vector3.zero ? _walkSpeed : 0; 
            else
                _targetSpeed = moveInputVector != Vector3.zero ? _runSpeed : 0;

            _newSpeed = Mathf.Lerp(_newSpeed, _targetSpeed, Time.deltaTime * _moveSharpness);

            if (_inAnimation)
                _newVelocity = _animatorVelocity;
            else
                _newVelocity = moveInputVectorOrientation * _targetSpeed;
            transform.Translate(_newVelocity * Time.deltaTime, Space.World);

            if (_inAnimation)
            {
                transform.rotation *= _animatorDeltaRotation;
            }

            if(_strafing)
            {
                _targetRotation = Quaternion.LookRotation(cameraPlanarDirection);
                _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
                transform.rotation = _newRotation;
            }
            else if (_targetSpeed != 0)
            {
                _targetRotation = Quaternion.LookRotation(moveInputVectorOrientation);
                _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
                transform.rotation = _newRotation;
            }

            if (_strafing)
            {
                _strafeParameter = Mathf.Clamp01(_strafeParameter + Time.deltaTime * 4);
                _strafeParameterXZ = Vector3.Lerp(_strafeParameterXZ, moveInputVector * _newSpeed, _moveSharpness * Time.deltaTime);
            }
            else
            {
                _strafeParameter = Mathf.Clamp01(_strafeParameter - Time.deltaTime * 4);
                _strafeParameterXZ = Vector3.Lerp(_strafeParameterXZ, Vector3.forward * _newSpeed, _moveSharpness * Time.deltaTime);
            }

            _animator.SetFloat("Strifing", _strafeParameter);
            _animator.SetFloat("StrifingX", Mathf.Round(_strafeParameterXZ.x * 100f) / 100f);
            _animator.SetFloat("StrifingZ", Mathf.Round(_strafeParameterXZ.z * 100f) / 100f);

            if(!_inAnimation)
            {
                if(_inputs.Attack.PressedDown())
                {
                    _inAnimation = true;
                    _animator.CrossFadeInFixedTime(_meleeAtack, 0.1f, 0, 0);
                }
            }

            if(_hitting)
            {
                _hitBox.CheckHit();
            }
        }

        private void OnAnimatorMove()
        {
            if(_inAnimation)
            {
                _animatorVelocity = _animator.velocity;
                _animatorDeltaRotation = _animator.deltaRotation;
            }
        }

        private void LateUpdate()
        {
            if (_proning) return;

            switch (_stance)
            {
                case CharacterStance.STANDING:
                    if (_inputs.Crouch.PressedDown()) { RequestStanceChange(CharacterStance.CROUCHING); }
                    break;

                case CharacterStance.CROUCHING:
                    if (_inputs.Crouch.PressedDown()) { RequestStanceChange(CharacterStance.STANDING); }
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
                case "HitStart":
                    _objectsHit.Clear();
                    _hitting = true;
                    break;

                case "HitEnd":
                    _hitting = false;
                    break;

                case "AnimationEnd":
                    _inAnimation = false;
                    break;
            }
        }

        int IHitResponder.Damage { get => _damage; }

        bool IHitResponder.CheckHit(HitInteraction data)
        {
            if (data.HurtBox.Owner == gameObject)
                return false;

            else if(_objectsHit.Contains(data.HurtBox.Owner))
                return false;
            else
                return true;
        }

        void IHitResponder.Response(HitInteraction data)
        {
            _objectsHit.Add(data.HurtBox.Owner);
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
