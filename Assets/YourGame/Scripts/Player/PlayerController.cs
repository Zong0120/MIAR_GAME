using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerInputAction
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour,IDamageable
    {
        //public static PlayerController Instance;
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 15.0f;

        [Space(10)]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public List<GameObject> cinemachines;

        private Rigidbody2D _playerRigidbody => GetComponent<Rigidbody2D>();
        private PlayerInput _playerInput => GetComponent<PlayerInput>();
        private Animator _animator => GetComponent<Animator>();
        private PlayerInputStatus _input => GetComponent<PlayerInputStatus>();
        private GameObject _mainCamera;
        Vector2 moveDirection;

        private string _currentAnimation = ""; 
        private bool isFreezed = false;
        private bool FacingRight = true;
        public bool CanvasCanOpen = true;
        private bool CanCloseCanvas = false;
        private bool _isDead = false;
        private InheritanceSceneBox _inheritanceSceneBox =null;
        private HealthManager _healthManager => GetComponent<HealthManager>();

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        void Update()
        {
            CheckAnimation();
            OpenCanvas();
            Move();
            UseItem();
        }
            
        private void Move()
        {
            if (isFreezed) return;
            // Calculate movement direction
            moveDirection = new Vector2(_input.move.x, _input.move.y).normalized;
            // Move the player
            _playerRigidbody.MovePosition(_playerRigidbody.position + moveDirection * (MoveSpeed * Time.fixedDeltaTime));
        }

        public void CheckAnimation()
        {
            if(_currentAnimation == "Death_Right" ||_currentAnimation == "Death_Left"||_currentAnimation == "CloseBox")return;

            if(_currentAnimation == "OpenBox")
            {
                if(_input.interactive && CanCloseCanvas)
                {
                    ChangeAnimation("CloseBox");
                    _input.interactive = false;
                    CanvasCanOpen = true;
                    CanCloseCanvas = false;
                }
                return;
            }


            if(moveDirection.x > 0)
            {
                ChangeAnimation("Run_right");
                FacingRight = true;
            }
            else if(moveDirection.x < 0)
            {
                ChangeAnimation("Run_left");
                FacingRight = false;
            }
            else if(moveDirection.y != 0)
            {
                if(FacingRight)
                    ChangeAnimation("Run_right");
                else
                    ChangeAnimation("Run_left");
            }
            else
            {
                if(FacingRight)
                    ChangeAnimation("Idle_right");
                else
                    ChangeAnimation("Idle_left");
            }
        }

        public void ChangeAnimation(string animationName,float crossFadeTime = 0.2f,float time = 0)
        {
            if(time > 0)StartCoroutine(WaitForAnimation(time));
            else Validate();

            IEnumerator WaitForAnimation(float time)
            {
                yield return new WaitForSeconds(time - crossFadeTime);
                Validate();
            }

            void Validate()
            {
                if (_currentAnimation != animationName)
                {
                    _currentAnimation = animationName;
                    if(_currentAnimation == "")
                        CheckAnimation();
                    else 
                        _animator.CrossFade(animationName, crossFadeTime);
                }
            }
        }


        public void SetDirection(Vector2 vector2)
        {
            moveDirection = vector2;
        }

        public void UseItem()
        {
            if(_input.useEquip)
            {
                EquipManager.Instance.UseEquip();
            }

            if(_input.equip1)
            {
                EquipManager.Instance.SwitchEquipIndex(0);
                _input.equip1 = false;
            }
            if(_input.equip2)
            {
                EquipManager.Instance.SwitchEquipIndex(1);
                _input.equip2 = false;
            }
        }

        public void OpenCanvas()
        {
            if (_input.bag)
            {
                _input.bag = false;
                if(!CanvasCanOpen)return;
                if(InventoryItemManager.Instance.BagIsOpen())
                    InventoryItemManager.Instance.CloseBag();
                else
                    InventoryItemManager.Instance.OpenBag();
            }
            if (_input.interactive)
            {
                _input.interactive = false;
                if(!CanvasCanOpen)return;
                if(_inheritanceSceneBox != null)
                {
                    CanvasCanOpen = false;
                    isFreezed = true;
                    transform.position = _inheritanceSceneBox.GetPlayerPositionTarget();
                    ChangeAnimation("OpenBox");
                }
            }
        }

        public void OnBoxAnimationStart()
        {
            _inheritanceSceneBox.OpenBox();
            CanCloseCanvas = true;
        }
        public void OnBoxAnimationEnd()
        {
            _inheritanceSceneBox.CloseBox();
            isFreezed = false;
        }

        public void SetInheritanceSceneBox(InheritanceSceneBox box)
        {
            _inheritanceSceneBox = box;
        }
        public void ClearInheritanceSceneBox()
        {
            _inheritanceSceneBox = null;
        }

        public void TakeDamage(int damage,Transform hitTransform)
        {
            _healthManager.TakeDamage(damage,hitTransform);
            if(_healthManager.IsDead)
            {
                _isDead = true;
                isFreezed = true;
                PlayerDeath();
            }
        }

        public void PlayerDeath()
        {
            if(_input.restart)
            {
                _input.restart = false;
                if(_isDead)
                {
                    LoadScene();
                }
            }

        }

        private void LoadScene()
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }

        public void FullLoadScene()
        {
            //Terminal_Canvas.Instance.FullyInitialize();
            //Player.Instance.gameObject.SetActive(false);
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }

    }
}
