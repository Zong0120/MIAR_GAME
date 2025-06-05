using UnityEngine;

public class ClearenceDetection : MonoBehaviour
{
    [SerializeField]private CollToStart collToStart;
    [SerializeField]private GameObject clearenceUI;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryItemManager.Instance.BagIsEmpty())
            {
                collToStart.isStart = false;
                clearenceUI.SetActive(true);
            }
            else
            {
                if(!PlayerPrefs.HasKey("Clearence"))
                {
                    GuidanceSystem.Instance.TriggerNode("HintClearence");
                }
                else
                {
                    PlayerPrefs.SetInt("Clearence", 1);
                }
                collToStart.isStart = true;
                clearenceUI.SetActive(false);
            }
        }
    }
}
