using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AirFishLab.ScrollingList.Demo;
using PlayerInputAction;

public class BagPageManager : MonoBehaviour
{
    public static BagPageManager Instance { get; private set; }
    [Header("Page")]
    [SerializeField] private BagPage[] _pages = new BagPage[4];
    [SerializeField]private ListEventDemo _listEventDemo;
    [Header("Setting")]
    [SerializeField]private GameObject _settingPage;

    [Header("State Display")]
    [SerializeField]private TextMeshProUGUI _bloodText;
    [SerializeField]private TextMeshProUGUI _speedText;
    [SerializeField]private TextMeshProUGUI _interactionText;
    [SerializeField]private TextMeshProUGUI _timeText;
    
    public int topageCheld { get; set; } = 0;
    private string InitMoveSpeed,InitInteractionRadius,   InitBloodValue;
    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < _pages.Length; i++)
        {
            if (_pages[i] == null) continue;
            _pages[i]._button.onClick.AddListener(ChangePage);
        }
        _listEventDemo.OnButtonClick(0);

        InitMoveSpeed = PlayerController.Instance.InitMoveSpeed.ToString();
        InitInteractionRadius = InteractionScope.Instance.InitRadius.ToString();
        InitBloodValue = HealthManager.Instance.maxHealthvalue().ToString();
    }
    private void OnEnable()
    {
        UpdateStateDisplay();
    }
    private void UpdateStateDisplay()
    {
        _bloodText.text = HealthManager.Instance.currentHealthvalue() + "/" + HealthManager.Instance.maxHealthvalue();
        _speedText.text = PlayerController.Instance.MoveSpeed.ToString();
        _interactionText.text = InteractionScope.Instance.newRadius.ToString() + "/" + InteractionScope.Instance.InitRadius.ToString();
        _timeText.text = TimerManager.Instance.time.text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToPage(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToPage(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToPage(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToPage(3);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_settingPage.activeSelf)
                _settingPage.SetActive(false);
            else
                _settingPage.SetActive(true);
        }
    }
    public void OpenSettingPage()
    {
        _settingPage.SetActive(true);
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
        _listEventDemo.OnButtonClick(page);
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
