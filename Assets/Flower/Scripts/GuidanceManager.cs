using UnityEngine;
using Flower;

public class GuidanceManager : MonoBehaviour
{
    FlowerSystem flowerSys;

    void Start()
    {
        flowerSys = FlowerManager.Instance.CreateFlowerSystem("GuidanceBox",false);
    }
}
