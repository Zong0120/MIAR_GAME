using UnityEngine;

public enum TutorialStep
{
    None,
    OpenedBackpack,
    EquippedItem,
    UsedItem,
    SwitchedItem
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private TutorialStep currentStep = TutorialStep.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        LoadTutorialProgress(); // 在遊戲啟動時載入進度
        RegisterEventsForStep(currentStep); // 註冊當前步驟的事件
    }
    private void LoadTutorialProgress()
    {
        if (PlayerPrefs.HasKey("currentStep"))
        {
            currentStep = (TutorialStep)PlayerPrefs.GetInt("currentStep", 0);
            RegisterEventsForStep((TutorialStep)(currentStep + 1));
        }
        else
        {
            RegisterEventsForStep(TutorialStep.OpenedBackpack);
        }
    }

    public void AdvanceTutorial(TutorialStep step)
    {
        //Debug.Log("Advancing tutorial to step: " + step);
        if ((int)step > (int)currentStep)
        {
            currentStep = step;
            GuidanceSystem.Instance.TriggerNode(step.ToString());

            UnregisterEventsForStep(step); // 取消註冊當前步驟的事件
            // 根據當前教學步驟註冊對應事件
            RegisterEventsForStep((TutorialStep)(currentStep + 1));
            // 儲存進度
            PlayerPrefs.SetInt("currentStep", (int)currentStep);
            PlayerPrefs.Save();
        }
    }


    void RegisterEventsForStep(TutorialStep step)
    {
        //Debug.Log("Registering events for step: " + step);
        switch (step)
        {
            case TutorialStep.OpenedBackpack:
                InventoryItemManager.Instance.HintBackpackOpen += HandleBackpackOpen;
                break;

            case TutorialStep.EquippedItem:
                InventoryItemManager.Instance.HintItemEquip += HandleItemEquip;
                break;

            case TutorialStep.UsedItem:
                EquipManager.Instance.HintItemUse += HandleItemUse;
                break;

            case TutorialStep.SwitchedItem:
                EquipManager.Instance.HintItemSwitch += HandleItemSwitch;
                break;

        }
    }

    void UnregisterEventsForStep(TutorialStep step)
    {
        switch (step)
        {

            case TutorialStep.OpenedBackpack:
                InventoryItemManager.Instance.HintBackpackOpen -= HandleBackpackOpen;
                break;

            case TutorialStep.EquippedItem:
                InventoryItemManager.Instance.HintItemEquip -= HandleItemEquip;
                break;

            case TutorialStep.UsedItem:
                EquipManager.Instance.HintItemUse -= HandleItemUse;
                break;
            case TutorialStep.SwitchedItem:
                EquipManager.Instance.HintItemSwitch -= HandleItemSwitch;
                break;

                // 依此類推...
        }
    }

    void HandleBackpackOpen()
    {
        AdvanceTutorial(TutorialStep.OpenedBackpack);
    }

    void HandleItemEquip()
    {
        AdvanceTutorial(TutorialStep.EquippedItem);
    }

    void HandleItemUse()
    {
        AdvanceTutorial(TutorialStep.UsedItem);
    }
    void HandleItemSwitch()
    {
        AdvanceTutorial(TutorialStep.SwitchedItem);
    }
}
