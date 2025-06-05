using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private GameObject level1, level2;
    [SerializeField] private GameObject position1, position2;
    [SerializeField] private Transform player;
    void OnEnable()
    {
        level1.SetActive(true);
        level2.SetActive(false);
        if (player.position.y > -100)
        {
            position1.SetActive(true);
            position2.SetActive(false);
        }
        else
        {
            position1.SetActive(false);
            position2.SetActive(true);
        }
    }

    public void ToggleComputer()
    {
        if (level1.activeSelf)
        {
            level1.SetActive(false);
            level2.SetActive(true);
        }
        else
        {
            level1.SetActive(true);
            level2.SetActive(false);
        }
    }
}
