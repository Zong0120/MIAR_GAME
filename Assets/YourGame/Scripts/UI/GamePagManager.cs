using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePageManager : MonoBehaviour
{
    public static GamePageManager Instance { get; private set; }
    [Header("Page")]
    [SerializeField] private GamePage[] _pages;

    public string _currentPageName = "gameing";

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void OpenPage(string pageName)
    {
        foreach (var page in _pages)
        {
            if (page._pageName == pageName)
            {
                page.OpenPage();
                _currentPageName = pageName;
            }
            else
            {
                page.ClosePage();
            }
        }
    }
}

[System.Serializable]
public class GamePage
{
    [SerializeField]public string _pageName;
    [SerializeField]private Canvas _page;

    public void OpenPage()
    {
        _page.enabled = true;
    }

    public void ClosePage()
    {
        _page.enabled = false;
    }
}
