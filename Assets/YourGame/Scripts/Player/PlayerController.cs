using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerInputAction
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 15.0f;

        [Space(10)]
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public List<GameObject> cinemachines;

        [Space(10)]
        [Header("Player Death")]
        [Tooltip("The prefab of the player death VFX")]
        public GameObject playerDeathVFXPrefab;

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
        private string _currentChapterId = "";
        private InheritanceSceneBox _inheritanceSceneBox =null;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        void Start()
        {
            GuidanceSystem.Instance.SetCurrentNode("GameStart");
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
            if(_currentAnimation == "Death_Right" ||_currentAnimation == "Death_Left"||_currentAnimation == "CloseBox"||_currentAnimation=="ReadFile_Right"||_currentAnimation=="ReadFile_Left")return;

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
            if(_input.map)
            {
                _input.map = false;
                if(!CanvasCanOpen)return;

                if(InventoryItemManager.Instance.BagIsOpen())
                    InventoryItemManager.Instance.CloseBag();
                else
                    InventoryItemManager.Instance.OpenMap();
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
            if(_input.target)
            {
                _input.target = false;
                if(!CanvasCanOpen)return;
                TargetManager.Instance.OpenTarget();
                SoundManager.PlaySoundItemAudio(SoundType.UI, "UI_Button");
            }
        }

        public void Accelerate_Player(float addspeed,float durationtime)
        {
            
            StartCoroutine(Accelerate(addspeed, durationtime));
        }
        IEnumerator Accelerate(float addspeed,float durationtime)
        {
            MoveSpeed += addspeed;
            yield return new WaitForSeconds(durationtime);
            MoveSpeed -= addspeed;
        }

        public void ReadChapter(string storyID)
        {
            if (storyID == null) return;
            isFreezed = true;
            ChangeAnimation("ReadFile_" + ((FacingRight == true) ? "Right" : "Left"));
            _currentChapterId = storyID;
        }

        public void OnReadEnd()
        {
            if (_currentChapterId == null) return;
            StoryManager.Instance.UnlockNextChapter(_currentChapterId);
            _currentChapterId = null;
            isFreezed = false;
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
        public void FreezePlayer()
        {
            isFreezed = true;
            ChangeAnimation("Idle_" + ((FacingRight == true) ? "Right" : "Left"));
        }
        public void UnFreezePlayer()
        {
            isFreezed = false;
        }

        public void PlayerDeath()
        {
            isFreezed = true;
            ChangeAnimation("Death_" + ((FacingRight == true) ? "Right" : "Left"));
            StartCoroutine(WaitReStart());
        }
        IEnumerator WaitReStart()
        {
            PlayerPrefs.SetInt("DeathCount", PlayerPrefs.GetInt("DeathCount", 0) + 1);
            GuidanceSystem.Instance.ShowRandomDeathMessage();
            while(true)
            {
                if(_input.restart)
                {
                    _input.restart = false;
                    break;
                }
                yield return null;
            }
            _input.restart = true;
            isFreezed = false;
            LoadScene();
            Instantiate(playerDeathVFXPrefab, transform.position, Quaternion.identity);
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
