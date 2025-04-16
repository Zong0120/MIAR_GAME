using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AirFishLab.ScrollingList.Demo
{
    public class BagPageManager : MonoBehaviour
    {
        [Header("Page")]
        [SerializeField] private BagPage[] _pages = new BagPage[4];
        
        public int topageCheld = 0;
        public bool shouldChangePage = true;
        void Start()
        {
            ChangePage();
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

    }
}

[System.Serializable]
public class BagPage
{
    [SerializeField]private string _pageName;
    public GameObject _page;
    [SerializeField]private Button _button;
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
