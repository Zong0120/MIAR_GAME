using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AirFishLab.ScrollingList.Demo
{
    public class BagPageManager : MonoBehaviour
    {
        public static BagPageManager Instance { get; private set; }
        [Header("Page")]
        [SerializeField] private BagPage[] _pages = new BagPage[4];
        [SerializeField]private ListEventDemo _listEventDemo;
        
        public int topageCheld = 0;
        public bool shouldChangePage = true;
        
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }
        void Start()
        {
            _listEventDemo.OnButtonClick(0);
        }
        public void ChangePage()
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                if (_pages[i] == null) continue;
                if (topageCheld == i + 1)
                {
                    _pages[i].OpenPage();
                }
                else
                {
                    if (_pages[i]._page.gameObject.activeSelf)
                    {
                        _pages[i].ClosePage();
                    }
                }
            }
        }

        public void ToPage(int page)
        {
            if (shouldChangePage)
            {
                _listEventDemo.OnButtonClick(page);
            }
        }
    }
}

[System.Serializable]
public class BagPage
{
    [SerializeField]private string _pageName;
    public GameObject _page;
    public Button _button;
    [SerializeField]private Sprite _normalImage;
    [SerializeField]private Sprite _changeImage;

    public void OpenPage()
    {
        _page.SetActive(true);
        _button.image.sprite = _changeImage;
    }
    public void ClosePage()
    {
        _page.SetActive(false);
        _button.image.sprite = _normalImage;
    }
}
