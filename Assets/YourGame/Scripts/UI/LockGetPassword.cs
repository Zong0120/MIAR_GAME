using AirFishLab.ScrollingList.Demo;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ListPassword
{
    [SerializeField]private ListEventDemoLock _list;
    [SerializeField]private RectTransform _listPos;
    public int Password => 10-_list._password;

    public void InitList()
    {
        _listPos.anchoredPosition = new Vector2(_listPos.anchoredPosition.x, 202f);
        _list.InitList();
    }
}

namespace AirFishLab.ScrollingList.Demo
{
    public class LockGetPassword : MonoBehaviour
    {
        
        [Header("List")]
        [SerializeField]private ListPassword[] _List = new ListPassword[4];
        [SerializeField]private GameObject imageAni;
        private Canvas parentcanvas;
        [SerializeField]private Canvas Locked;
        [SerializeField]private Canvas topic;
        [SerializeField] private Image question;

        public string UnlockedAnimator = "unlocked";
         private void Start()
        {
            initlocked();
            parentcanvas = GetComponent<Canvas>();
        }

        public void initlocked()
        {
            parentcanvas.enabled = false;
            Locked.enabled = false;
            topic.enabled = true;
            imageAni.SetActive(false);
            for (int i = 0; i < _List.Length; i++)
            {
                _List[i].InitList();
            }
        }


        public string stringPassword ;
        public void OnMovementEnd()
        {
            CheckPassword();
        }

         private void CheckPassword()
        {
            stringPassword = _List[0].Password.ToString() + _List[1].Password.ToString() +_List[2].Password.ToString() +_List[3].Password.ToString();
            Debug.Log("Password: " + stringPassword);
        }

        private void OpenCanvas()
        {
            parentcanvas.enabled = true;
            imageAni.SetActive(true);
            imageAni.GetComponent<Animator>().SetBool("unclocked",false);
        }

        public void Unlocked(){
            imageAni.GetComponent<Animator>().SetBool(UnlockedAnimator,true);
            Locked.enabled = false;
            topic.enabled = false;
        }
        public void canvasclose(){
            parentcanvas.enabled = false;
        }
        public void SetQuestion(Sprite questionPicture)
        {
            question.sprite = questionPicture;
        }
    }
}
