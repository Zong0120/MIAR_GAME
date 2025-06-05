using UnityEngine;
using PlayerInputAction;

public class ComputerColl : MonoBehaviour
{
    [SerializeField] private GameObject computerUI;
    bool ishinted= false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.canOpenComputer = true;
            if (!ishinted)
            {
                GuidanceSystem.Instance.TriggerNode("HintComputer");
                ishinted = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.canOpenComputer = false;
            computerUI.SetActive(false);
        }
    }
}

