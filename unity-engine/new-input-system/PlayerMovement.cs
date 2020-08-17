using JetBrains.Annotations;
using Assets.Scripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Camera _mainCamera;

        [Header("Physics")] public Rigidbody PlayerRigidbody;

        [Header("Animation")] public Animator PlayerAnimator;

        private Vector3 _inputDirection;
        private Vector2 _movementInput;
        private bool _currentInput = false;

        [Header("Movement Settings")] public float MovementSpeed = 3;
        public float SmoothingSpeed = 1;
        private Vector3 _currentDirection;
        private Vector3 _rawDirection;
        private Vector3 _smoothDirection;
        private Vector3 _movement;

        void Start()
        {
            _mainCamera = Camera.main;
        }


        void Update()
        {
            CalculateMovementInput();
        }

        void FixedUpdate()
        {
            CalculateDesiredDirection();
            ConvertDirectionFromRawToSmooth();
            MoveThePlayer();
            TurnThePlayer();
        }


        private void CalculateMovementInput()
        {
            if (_inputDirection == Vector3.zero)
            {
                _currentInput = false;
            }
            else if (_inputDirection != Vector3.zero)
            {
                _currentInput = true;
            }
        }


        private void CalculateDesiredDirection()
        {
            //Camera Direction
            var cameraForward = _mainCamera.transform.forward;
            var cameraRight = _mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            _rawDirection = cameraForward * _inputDirection.z + cameraRight * _inputDirection.x;
        }

        private void ConvertDirectionFromRawToSmooth()
        {
            switch (_currentInput)
            {
                case true:
                    _smoothDirection = Vector3.Lerp(_smoothDirection, _rawDirection, Time.deltaTime * SmoothingSpeed);
                    break;
                case false:
                    _smoothDirection = Vector3.zero;
                    break;
            }
        }

        private void MoveThePlayer()
        {
            if (_currentInput)
            {
                PlayerAnimator.SetBool("isWalking", true);
                _movement.Set(_smoothDirection.x, 0f, _smoothDirection.z);
                _movement = _movement.normalized * MovementSpeed * Time.deltaTime;
                PlayerRigidbody.MovePosition(transform.position + _movement);
            }
            else
            {
                PlayerAnimator.SetBool("isWalking", false);
            }
        }

        private void TurnThePlayer()
        {
            if (_currentInput != true) return;
            Quaternion newRotation = Quaternion.LookRotation(_smoothDirection);
            PlayerRigidbody.MoveRotation(newRotation);
        }


        private void OnMovement(InputValue value)
        {
            Vector2 inputMovement = value.Get<Vector2>();
            _inputDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        private void OnInteract(InputValue value)
        {
            PlayerAnimator.SetTrigger("isJumping");
        }
    }
}