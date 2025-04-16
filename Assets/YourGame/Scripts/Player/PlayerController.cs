using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
namespace PlayerInputAction
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour,IDamageable
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 15.0f;

        [Header("Player Health")]
        [SerializeField] private const int maxHealth = 3;
        int currentHealth=maxHealth;

        [Space(10)]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public List<GameObject> cinemachines;

        private Rigidbody2D _playerRigidbody;
        private PlayerInput _playerInput;
        private Animator _animator;
        private PlayerInputStatus _input;
        private GameObject _mainCamera;
        Vector2 moveDirection;
        private bool isFreezed = false;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        void Start()
        {
            _input = GetComponent<PlayerInputStatus>();
            _playerInput = GetComponent<PlayerInput>();
            _playerRigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            Move();
            UseItem();
            OpenBag();
        }
            
        private void Move()
        {
            if (isFreezed)
            {
                return;
            }
            // Calculate movement direction
            moveDirection = new Vector2(_input.move.x, _input.move.y).normalized;
            // Move the player
            _playerRigidbody.MovePosition(_playerRigidbody.position + moveDirection * (MoveSpeed * Time.fixedDeltaTime));
            // Rotate the player to face the movement direction
            if (moveDirection != Vector2.zero)
            {
                _animator.SetFloat("horizontal", moveDirection.x);
                _animator.SetFloat("vertical", moveDirection.y);
            }
            _animator.SetFloat("magnitude", moveDirection.sqrMagnitude);
            // Update animator parameters
        }

        public void SetDirection(Vector2 vector2)
        {
            moveDirection = vector2;
        }

        public void UseItem()
        {
            if(_input.useEquip)
            {
                EquipManager.Instance.UseEuip();
            }
        }

        public void OpenBag()
        {
            if (_input.bag)
            {
                _input.bag = false;
                if(GamePageManager.Instance._currentPageName == "gameing")
                    GamePageManager.Instance.OpenPage("backpack");
            }
        }
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Debug.Log("Player is dead");
            }
        }
    }
}
