using AirFishLab.ScrollingList.Demo;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ListPassword
{
    [SerializeField]private ListEventDemoLock _list;
    [SerializeField]private RectTransform _listPos;
    public int Password => 10-_list._password;

    public void InitList()
    {
        _listPos.anchoredPosition = new Vector2(_listPos.anchoredPosition.x, 202f);
        _list._InitList();
    }

    public void ListButtonClick(int index)
    {
        _list.OnButtonClick(9-index);
    }
}

namespace AirFishLab.ScrollingList.Demo
{
    public class LockManager : MonoBehaviour
    {
        public static LockManager Instance { get; private set; }
        [SerializeField]private GameObject LockUIRoot;
        [SerializeField]private Animator imageAni;
        [SerializeField]private Canvas Locked;
        [SerializeField] private Image question;
        [Header("List")]
        [SerializeField]private ListPassword[] _List = new ListPassword[4];

        private string UnlockedAnimator = "unlocked";
        private int _inputIndex=0;

        private string stringPassword;
        private string BingoPassword;
        private DoorLock _doorLock;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OpenLock(Sprite questionPicture,string questionAns,DoorLock doorLock)
        {
            _doorLock = doorLock;
            Locked.enabled = true;
            BingoPassword = questionAns;
            question.sprite = questionPicture;
            _inputIndex = 0;
            imageAni.SetBool("unclocked",false);
            LockUIRoot.SetActive(true);
        }
        public void CloseLock()
        {
            LockUIRoot.SetActive(false);
            BingoPassword = "";
            _doorLock = null;
            for (int i = 0; i < _List.Length; i++)
            {
                _List[i].InitList();
            }
        }

        private void Update()
        {
            if(Locked.enabled == true)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GetComponent<LockManager>().enabled = false;
                    return;
                }

                //input 4digits 0-9
                for (int i = 0; i <= 9; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    {
                        _List[_inputIndex].ListButtonClick(i);
                        _inputIndex = _inputIndex == 3 ? 0 : _inputIndex + 1;
                    }
                }
            }
        }

        public void OnMovementEnd()
        {
            CheckPassword();
        }

         private void CheckPassword()
        {
            stringPassword = _List[0].Password.ToString() + _List[1].Password.ToString() +_List[2].Password.ToString() +_List[3].Password.ToString();

            if (stringPassword == BingoPassword)
            {
                Debug.Log("Unlocked");
                imageAni.SetBool(UnlockedAnimator,true);
                _doorLock.UnlockDoor();
                CloseLock();
            }
            Debug.Log("Password: " + stringPassword);
        }

        public bool LockToBingo()
        {
            if(stringPassword == "")return false;
            
            _List[0].ListButtonClick(int.Parse(stringPassword[0].ToString()));
            _List[1].ListButtonClick(int.Parse(stringPassword[1].ToString()));
            _List[2].ListButtonClick(int.Parse(stringPassword[2].ToString()));
            _List[3].ListButtonClick(int.Parse(stringPassword[3].ToString()));

            return true;
        }
    }
}
