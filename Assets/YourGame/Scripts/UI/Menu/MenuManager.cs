using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	[SerializeField] Menu[] menus;
	[SerializeField] Slider	loadingProgressBar;
	[SerializeField]private StoryProgressData storyProgressData;
	[SerializeField]private InheritanceInventoryItem weaponInventoryItem,propInventoryItem;

	void Awake()
	{
		Instance = this;
	}

	public void OpenMenu(string menuName)
	{
		for (int i = 0; i < menus.Length; i++)
		{
			if (menus[i].menuName == menuName)
			{
				menus[i].Open();
			}
			else if (menus[i].open)
			{
				CloseMenu(menus[i]);
			}
		}
	}

	public void OpenMenu(Menu menu)
	{
		for (int i = 0; i < menus.Length; i++)
		{
			if (menus[i].open)
			{
				CloseMenu(menus[i]);
			}
		}
		menu.Open();
		Debug.Log("Open Menu: " + menu.menuName);
	}

	public void CloseMenu(Menu menu)
	{
		menu.Close();
	}
	
	public void ReSartGame()
	{
		// 重置進度
		storyProgressData.ResetProgress();
		weaponInventoryItem.ResetProgress();
		propInventoryItem.ResetProgress();

		PlayGame();
	}

	public void PlayGame()
	{
		// 切換到 Loading Menu
		OpenMenu("loading");

		// 開始異步加載場景
		StartCoroutine(LoadGameSceneAsync());
	}

    private IEnumerator LoadGameSceneAsync()
	{
		yield return new WaitForSeconds(3f); // 等待一段時間，讓 Loading 畫面顯示
		// 確保進度條從 0 開始
		if (loadingProgressBar != null)
		{
			loadingProgressBar.value = 0f;
		}
	
		// 開始加載場景
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
		asyncLoad.allowSceneActivation = false; // 暫時不自動切換場景
	
		// 更新進度條
		while (!asyncLoad.isDone)
		{
			// 計算進度 (0 到 1)
			float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
	
			// 更新進度條
			if (loadingProgressBar != null)
			{
				loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, progress, Time.deltaTime * 5f); // 平滑過渡
			}
	
			// 當進度達到 90% 時，允許切換場景
			if (asyncLoad.progress >= 0.9f)
			{
				if (loadingProgressBar != null)
				{
					loadingProgressBar.value = 1f; // 確保進度條填滿
				}
	
				// 等待一段時間，確保玩家看到進度條完成
				yield return new WaitForSeconds(3f);
	
				asyncLoad.allowSceneActivation = true; // 切換場景
			}
	
			yield return null; // 等待下一幀，避免卡頓
		}
	}
}